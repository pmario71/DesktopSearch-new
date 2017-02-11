using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Code;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Extractors.Roslyn;
using DesktopSearch.Core.FileSystem;
using DesktopSearch.Core.Lucene;
using Nest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Processors
{
    public class CodeFolderProcessor : IFolderProcessor
    {
        private ICodeIndexer _codeIndexer;

        public CodeFolderProcessor(ICodeIndexer codeIndexer)
        {
            _codeIndexer = codeIndexer;
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
            var sourceFiles = new BlockingCollection<SourceFile>(100);

            int current = 0;
            long typesExtracted = 0;
            var maxFiles = filesToParse.Count();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            var reader = Task.Run(() =>
            {
                Parallel.ForEach(filesToParse, file =>
                {
                    //TODO: async io goes here
                    var sourceFile = new SourceFile(file, File.ReadAllText(file));
                    sourceFiles.Add(sourceFile);
                });
                sourceFiles.CompleteAdding();
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            foreach (var sourceFile in sourceFiles.GetConsumingEnumerable())
            {
                IEnumerable<TypeDescriptor> extractedTypes = null;

                try
                {
                    extractedTypes = parser.ExtractTypes(sourceFile.Content, sourceFile.Path);

                    await _codeIndexer.IndexAsync(extractedTypes);

                    typesExtracted += extractedTypes.Count();
                }
                catch (Exception ex)
                {
                    {
                        //WriteError(new ErrorRecord(innerException, "idx", ErrorCategory.NotSpecified, this));
                        //TODO: Log errors
                    }
                }

                ++current;
                if (progress != null)
                {
                    progress.Report((int)(current * 100 / (double)maxFiles));
                }
            }
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
    }
}
