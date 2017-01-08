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
using DesktopSearch.Core.Services;

namespace DesktopSearch.Core.Processors
{
    public class DocumentFolderProcessor : IFolderProcessor
    {
        private readonly IElasticClient       _client;
        private readonly ElasticSearchConfig  _configuration;
        private TikaServerExtractor           _extractor;
        private IDocumentCollectionRepository _documentCollectionRepository;

        //private readonly ILogger<DocumentFolderProcessor> _logging;

        public DocumentFolderProcessor(
            IDocumentCollectionRepository documentCollectionRepository,
            IElasticClient client, 
            ElasticSearchConfig config/*, ILogger logging*/)
        {
            _documentCollectionRepository = documentCollectionRepository;
            _client                       = client;
            _configuration                = config;
            _extractor                    = new TikaServerExtractor();
            //_logging                    = logging;
        }

        public Task ProcessAsync(IFolder folder)
        {
            return ProcessAsync(folder, null);
        }

        public Task ProcessAsync(IFolder folder, IProgress<int> progress)
        {
            var extensionFilter = new ExcludeFileByExtensionFilter(".bin", ".lnk");

            var filesToProcess = Directory.GetFiles(folder.Path, "*", SearchOption.AllDirectories)
                                          .Where(f => extensionFilter.FilterByExtension(f));

            return ExtractFilesAsync(filesToProcess, folder.DocumentCollection.Name, progress);
        }

        public Task ProcessAsync(string file, string documentCollectionName)
        {
            return ExtractFilesAsync(new[] { file }, documentCollectionName, null);
        }

        public async Task ProcessAsync(DocDescriptor document, string documentCollectionName)
        {
            var result = await ProcessAsyncInt(document, documentCollectionName);
            if (!result.IsValid)
            {
                throw new Exception(result.DebugInformation);
            }
        }

        private async Task ExtractFilesAsync(IEnumerable<string> filesToParse, string documentCollectionName, IProgress<int> progress=null)
        {
            int current = 0;
            var maxFiles = filesToParse.Count();

            foreach (var filePath in filesToParse)
            {
                var stopWatch = Stopwatch.StartNew();

                Extractors.ParserContext context = new Extractors.ParserContext();
                var docDesc = await _extractor.ExtractAsync(context, new FileInfo(filePath));

                var result = await ProcessAsyncInt(docDesc, documentCollectionName);
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

        private async Task<IIndexResponse> ProcessAsyncInt(DocDescriptor docDesc, string documentCollectionName)
        {
            docDesc.DocumentCollection = documentCollectionName;

            var result = await _client.IndexAsync(docDesc, 
                                  (indexSelector) => indexSelector.Index(_configuration.DocumentSearchIndexName));
            return result;
        }
    }
}
