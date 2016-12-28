using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.FileSystem;
using Nest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DesktopSearch.Core.Extractors.Tika;

namespace DesktopSearch.Core.Processors
{
    public class DocumentFolderProcessor : IFolderProcessor
    {
        private readonly IElasticClient _client;
        private readonly ElasticSearchConfig _configuration;
        DesktopSearch.Core.Extractors.Tika.TikaServerExtractor _extractor;
        //private readonly ILogger<DocumentFolderProcessor> _logging;

        public DocumentFolderProcessor(IElasticClient client, ElasticSearchConfig config/*, ILogger logging*/)
        {
            _client = client;
            _configuration = config;
            _extractor = new TikaServerExtractor();
            //_logging = logging;
        }

        public Task Process(Folder folder)
        {
            return Process(folder, null);
        }

        public Task Process(string file, string indexingTypeName)
        {
            return ExtractFilesAsync(new[] { file }, indexingTypeName, null);
        }

        public async Task Process(DocDescriptor document)
        {
            var result = await _client.IndexAsync(document, (indexSelector) => indexSelector.Index(_configuration.DocumentSearchIndexName));
            if (!result.IsValid)
            {
                throw new Exception(result.DebugInformation);
            }
        }

        public Task Process(Folder folder, IProgress<int> progress)
        {
            var extensionFilter = new ExcludeFileByExtensionFilter(".bin", ".lnk");

            var filesToProcess = Directory.GetFiles(folder.Path, "*", SearchOption.AllDirectories)
                                          .Where(f => extensionFilter.FilterByExtension(f));

            return ExtractFilesAsync(filesToProcess, folder.IndexingType, progress);
        }

        private async Task ExtractFilesAsync(IEnumerable<string> filesToParse, string indexingTypeName, IProgress<int> progress=null)
        {
            int current = 0;
            var maxFiles = filesToParse.Count();

            foreach (var filePath in filesToParse)
            {
                var stopWatch = Stopwatch.StartNew();

                Extractors.ParserContext context = new Extractors.ParserContext();
                var docDesc = await _extractor.ExtractAsync(context, new FileInfo(filePath));

                docDesc.ContentType = indexingTypeName;

                var result = await _client.IndexAsync(docDesc, (indexSelector) => indexSelector.Index(_configuration.DocumentSearchIndexName));

                if (!result.IsValid)
                {
                    //_logging.LogWarning($"Failed to index document: {filePath}!", result.OriginalException);
                    Console.WriteLine($"Failed to index document: {filePath}!\r\n{result.OriginalException}");
                }
                else
                {
                    stopWatch.Stop();
                    //_logging.LogInformation($"Added '{Path.GetFileName(filePath)}' to index  took: {stopWatch.Elapsed.TotalSeconds} [s]");
                }

                ++current;
                if (progress != null)
                {
                    progress.Report((int)(current * 100 / (double)maxFiles));
                }
            }
        }
    }
}
