﻿using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.ElasticSearch;
using DesktopSearch.Core.Processors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.Contracts;

namespace DesktopSearch.Core.Services
{
    internal class IndexingService : IIndexingService
    {
        private readonly Dictionary<IndexingStrategy, IFolderProcessor> _map;
        private readonly IDocumentCollectionRepository                  _documentCollectionRepository;
        private readonly IServiceManager                                _serviceManager;

        public IndexingService(
            IDocumentCollectionRepository documentCollectionRepository,
            DocumentFolderProcessor docFolderProcessor,
            CodeFolderProcessor codeFolderProcessor,
            IServiceManager serviceManager)
        {
            _map = new Dictionary<IndexingStrategy, IFolderProcessor>()
            {
                { IndexingStrategy.Code     , codeFolderProcessor },
                { IndexingStrategy.Documents, docFolderProcessor  },
            };

            _documentCollectionRepository = documentCollectionRepository;
            _serviceManager = serviceManager;
        }

        public async Task IndexRepositoryAsync(IDocumentCollection documentCollection, IProgress<int> progress = null)
        {
            IFolder folder;
            if (!_documentCollectionRepository.TryGetConfiguredLocalFolder(documentCollection, out folder))
            {
                throw new ArgumentException($"The provided IndexedCollection '{documentCollection.Name}' is not hosted locally. Updating the index not possible!");
            }
            if (documentCollection.IndexingStrategy == IndexingStrategy.Documents)
            {
                await _serviceManager.EnsureTikaStarted();
            }

            var processor = _map[documentCollection.IndexingStrategy];
            await processor.ProcessAsync(folder, progress);
        }

        public async Task IndexRepositoryAsync(IFolder folder, IProgress<int> progress = null)
        {
            if (folder.DocumentCollection.IndexingStrategy == IndexingStrategy.Documents)
            {
                await _serviceManager.EnsureTikaStarted();
            }

            var processor = _map[folder.DocumentCollection.IndexingStrategy];
            await processor.ProcessAsync(folder, progress);
        }

        public async Task IndexDocumentAsync(string documentPath)
        {
            IFolder folder;
            if (!_documentCollectionRepository.TryGetFolderForPath(new FileInfo(documentPath), out folder))
            {
                throw new ArgumentException($"File '{documentPath}' is not part of any DocumentCollection!");
            }

            await IndexRepositoryAsync(folder);
        }

        public async Task IndexDocumentAsync(DocDescriptor documentDescriptor)
        {
            IFolder folder;
            if (!_documentCollectionRepository.TryGetFolderForPath(new FileInfo(documentDescriptor.Path), out folder))
            {
                throw new ArgumentException($"Location of document '{documentDescriptor.Path}' is not part of any DocumentCollection!", nameof(documentDescriptor));
            }

            if (folder.DocumentCollection.IndexingStrategy != IndexingStrategy.Documents)
            {
                throw new ArgumentException($"documentDescriptor is referencing code and not documents!");
            }
            await IndexRepositoryAsync(folder);

            var processor = _map[IndexingStrategy.Documents] as DocumentFolderProcessor;
            await processor.ProcessAsync(documentDescriptor, folder.DocumentCollection.Name);
        }
    }
}
