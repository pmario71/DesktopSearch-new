using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Code;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Extractors.Roslyn;
using DesktopSearch.Core.FileSystem;
using DesktopSearch.Core.Lucene;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.Utils;
using Microsoft.Extensions.Logging;

namespace DesktopSearch.Core.Processors
{
    internal class CodeFolderProcessor : IFolderProcessor
    {
        private readonly ICodeIndexer _codeIndexer;
        private readonly IPerformance _performance;
        private ILogger<CodeFolderProcessor> _logger;

        public CodeFolderProcessor(ICodeIndexer codeIndexer, Utils.IPerformance performance)
        {
            _codeIndexer = codeIndexer;
            _performance = performance;
            _logger = Logging.GetLogger<CodeFolderProcessor>();
        }

        public ILogger<CodeFolderProcessor> OverrideLogger
        {
            get { return _logger; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                _logger = value;
            }
        }

        public Task ProcessAsync(IFolder folder)
        {
            return ProcessAsync(folder, null);
        }

        public Task ProcessAsync(string file, string indexingTypeName)
        {
            throw new NotImplementedException();
        }

        public Task ProcessAsync(IFolder folder, IProgress<int> progress)
        {
            var extensionFilter = new IncludeFileByExtensionFilter(".cs", ".xaml");

            var filesToProcess = Directory.GetFiles(folder.Path, "*", SearchOption.AllDirectories)
                                          .Where(f => extensionFilter.FilterByExtension(f));

            return ExtractFilesAsync(filesToProcess, progress);
        }

        private async Task ExtractFilesAsync(IEnumerable<string> filesToParse, IProgress<int> progress)
        {
            var parser = new RoslynParser();
            var stopWatch = Stopwatch.StartNew();
            var sourceFiles = new BlockingCollection<SourceFile>(10);

            int current = 0;
            long typesExtracted = 0;
            var maxFiles = filesToParse.Count();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            var reader = Task.Run(() =>
            {
                var filesReadCounter = _performance.GetCounter(IndexingCounters.FilesRead);
                filesReadCounter.ReadOnly = false;

                Parallel.ForEach(filesToParse,
                                 new ParallelOptions { MaxDegreeOfParallelism = 10 },
                                 file =>
                {
                    var readAllText = File.ReadAllText(file);
                    var sourceFile = new SourceFile(file, readAllText);
                    sourceFiles.Add(sourceFile);
                    filesReadCounter.Increment();
                });
                filesReadCounter.Dispose();
                sourceFiles.CompleteAdding();
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            var filesParsedCounter = _performance.GetCounter(IndexingCounters.FilesParsed);
            filesParsedCounter.ReadOnly = false;
            var filesIndexedCounter = _performance.GetCounter(IndexingCounters.FilesIndexed);
            filesIndexedCounter.ReadOnly = false;

            foreach (var sourceFile in sourceFiles.GetConsumingEnumerable())
            {
                IEnumerable<TypeDescriptor> extractedTypes = null;

                try
                {
                    extractedTypes = parser.ExtractTypes(sourceFile.Content, sourceFile.Path);
                    filesParsedCounter.Increment();

                    await _codeIndexer.IndexAsync(extractedTypes);
                    filesIndexedCounter.Increment();

                    typesExtracted += extractedTypes.Count();
                }
                catch (Exception ex)
                {
                    string msg = $"Error parsing file: {sourceFile.Path}";
                    _logger.LogWarning(new EventId(LoggedIds.ErrorParsingFile), ex, msg);
                }

                ++current;
                if (progress != null)
                {
                    progress.Report((int)(current * 100 / (double)maxFiles));
                }
            }

            filesIndexedCounter.Dispose();
            filesParsedCounter.Dispose();

            System.Diagnostics.Debug.Assert(reader.IsCompleted);
        }

        internal struct SourceFile
        {
            public readonly string Path;
            public readonly string Content;

            public SourceFile(string path, string content)
            {
                Path = path;
                Content = content;
            }
        }

        static async Task<string> ReadAllFileAsync(string filename)
        {
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                byte[] buff = new byte[file.Length];
                await file.ReadAsync(buff, 0, (int)file.Length);

                return Encoding.UTF8.GetString(buff);
            }
            
        }
    }
}
