﻿using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.FileSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DesktopSearch.Core.Extractors.Tika;
using DesktopSearch.Core.Lucene;
using DesktopSearch.Core.Services;
using Microsoft.Extensions.Options;

namespace DesktopSearch.Core.Processors
{
    public class DocumentFolderProcessor : IFolderProcessor
    {
        private readonly IDocumentIndexer              _client;
        private readonly ElasticSearchConfig           _configuration;
        private readonly ITikaServerExtractor          _extractor;
        private IDocumentCollectionRepository          _documentCollectionRepository;

        private ILogger<DocumentFolderProcessor> _logger;

        public DocumentFolderProcessor(
            IDocumentCollectionRepository documentCollectionRepository,
            IDocumentIndexer client, 
            IConfigAccess<ElasticSearchConfig> config,
            ITikaServerExtractor extractor)
        {
            _documentCollectionRepository = documentCollectionRepository;
            _client                       = client;
            _configuration                = config.Get();
            _extractor                    = extractor;
        }

        public ILogger<DocumentFolderProcessor> OverrideLogger
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

        public Task ProcessAsync(DocDescriptor document, string documentCollectionName)
        {
            return ProcessAsyncInt(document, documentCollectionName);
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

                try
                {
                    await ProcessAsyncInt(docDesc, documentCollectionName);

                    stopWatch.Stop();
                    _logger.LogInformation($"Added '{Path.GetFileName(filePath)}' to index  took: {stopWatch.Elapsed.TotalSeconds} [s]");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Failed to index document: {filePath}!", ex);

                }

                ++current;
                if (progress != null)
                {
                    progress.Report((int)(current * 100 / (double)maxFiles));
                }
            }
        }

        private async Task ProcessAsyncInt(DocDescriptor docDesc, string documentCollectionName)
        {
            docDesc.DocumentCollection = documentCollectionName;

            //var result = await _client.IndexAsync(docDesc, 
            //                      (indexSelector) => indexSelector.Index(_configuration.DocumentSearchIndexName));

            await _client.IndexAsync(new[] {docDesc});
        }
    }
}
