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
        private readonly IDocker                                        _docker;

        public IndexingService(
            IDocumentCollectionRepository documentCollectionRepository,
            DocumentFolderProcessor docFolderProcessor,
            CodeFolderProcessor codeFolderProcessor,
            IDocker docker)
        {
            _map = new Dictionary<IndexingStrategy, IFolderProcessor>()
            {
                { IndexingStrategy.Code     , codeFolderProcessor },
                { IndexingStrategy.Documents, docFolderProcessor  },
            };

            _documentCollectionRepository = documentCollectionRepository;
            _docker = docker;
        }

        public async Task IndexRepositoryAsync(IDocumentCollection documentCollection, IProgress<int> progress = null)
        {
            await _docker.EnsureTikaStarted();

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
            await _docker.EnsureTikaStarted();

            var processor = _map[folder.DocumentCollection.IndexingStrategy];
            await processor.ProcessAsync(folder, progress);
        }

        public async Task IndexDocumentAsync(string documentPath)
        {
            await _docker.EnsureTikaStarted();

            IFolder folder;
            if (!_documentCollectionRepository.TryGetFolderForPath(new FileInfo(documentPath), out folder))
            {
                throw new ArgumentException($"File '{documentPath}' is not part of any DocumentCollection!");
            }

            var processor = _map[folder.DocumentCollection.IndexingStrategy];

            await processor.ProcessAsync(documentPath, folder.DocumentCollection.Name);
        }

        public async Task IndexDocumentAsync(DocDescriptor documentDescriptor)
        {
            await _docker.EnsureTikaStarted();

            IFolder folder;
            if (!_documentCollectionRepository.TryGetFolderForPath(new FileInfo(documentDescriptor.Path), out folder))
            {
                throw new ArgumentException($"Location of document '{documentDescriptor.Path}' is not part of any DocumentCollection!", nameof(documentDescriptor));
            }

            if (folder.DocumentCollection.IndexingStrategy != IndexingStrategy.Documents)
            {
                throw new ArgumentException($"documentDescriptor is referencing code and not documents!");
            }

            var processor = _map[IndexingStrategy.Documents] as DocumentFolderProcessor;
            await processor.ProcessAsync(documentDescriptor, folder.DocumentCollection.Name);
        }
    }
}
