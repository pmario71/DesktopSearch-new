﻿using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Services
{
    public class IndexingService : IIndexingService
    {
        private Dictionary<IndexingStrategy, IFolderProcessor> _map;
        private IDocumentCollectionRepository _documentCollectionRepository;

        public IndexingService(
            IDocumentCollectionRepository documentCollectionRepository,
            DocumentFolderProcessor docFolderProcessor,
            CodeFolderProcessor codeFolderProcessor)
        {
            _map = new Dictionary<IndexingStrategy, IFolderProcessor>()
            {
                { IndexingStrategy.Code     , codeFolderProcessor },
                { IndexingStrategy.Documents, docFolderProcessor  },
            };

            _documentCollectionRepository = documentCollectionRepository;
        }

        public Task IndexRepositoryAsync(IDocumentCollection documentCollection, IProgress<int> progress = null)
        {
            var processor = _map[documentCollection.IndexingStrategy];

            IFolder folder;
            if (!_documentCollectionRepository.TryGetConfiguredLocalFolder(documentCollection, out folder))
            {
                throw new ArgumentException($"The provided IndexedCollection '{documentCollection.Name}' is not hosted locally. Updating the index not possible!");
            }

            return processor.ProcessAsync(folder, progress);
        }

        public Task IndexRepositoryAsync(IFolder folder, IProgress<int> progress = null)
        {
            var processor = _map[folder.DocumentCollection.IndexingStrategy];
            return processor.ProcessAsync(folder, progress);
        }
    }

    public interface IIndexingService
    {
        Task IndexRepositoryAsync(IDocumentCollection documentCollection, IProgress<int> progress = null);

        Task IndexRepositoryAsync(IFolder folder, IProgress<int> progress = null);
    }
}