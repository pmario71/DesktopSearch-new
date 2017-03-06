using DesktopSearch.Core.DataModel.Documents;
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

        Task EnsureInitialized = Task.CompletedTask;

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

        public async Task IndexRepositoryAsync(IDocumentCollection documentCollection, IProgress<int> progress = null)
        {
            await EnsureInitialized;

            var processor = _map[documentCollection.IndexingStrategy];

            IFolder folder;
            if (!_documentCollectionRepository.TryGetConfiguredLocalFolder(documentCollection, out folder))
            {
                throw new ArgumentException($"The provided IndexedCollection '{documentCollection.Name}' is not hosted locally. Updating the index not possible!");
            }

            await processor.ProcessAsync(folder, progress);
        }

        public async Task IndexRepositoryAsync(IFolder folder, IProgress<int> progress = null)
        {
            await EnsureInitialized;

            var processor = _map[folder.DocumentCollection.IndexingStrategy];
            await processor.ProcessAsync(folder, progress);
        }

        public async Task IndexDocumentAsync(string documentPath)
        {
            await EnsureInitialized;

            IFolder folder;
            if (!_documentCollectionRepository.TryGetFolderForPath(new FileInfo(documentPath), out folder))
            {
                throw new ArgumentException($"File '{documentPath}' is not part of any DocumentCollection!");
            }

            var processor = _map[folder.DocumentCollection.IndexingStrategy];

            await processor.ProcessAsync(documentPath, folder.DocumentCollection.Name);
        }

        public Task IndexDocumentAsync(DocDescriptor document)
        {
            IFolder folder;
            if (!_documentCollectionRepository.TryGetFolderForPath(new FileInfo(document.Path), out folder))
            {
                throw new ArgumentException($"Location of document '{document.Path}' is not part of any DocumentCollection!", nameof(document));
            }

            var processor = _map[folder.DocumentCollection.IndexingStrategy] as DocumentFolderProcessor;
            return processor.ProcessAsync(document, folder.DocumentCollection.Name);
        }
    }
}
