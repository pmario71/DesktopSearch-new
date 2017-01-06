using DesktopSearch.Core.DataModel.Documents;
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
        private IDocTypeRepository _docTypeRepository;

        public IndexingService(
            IDocTypeRepository docTypeRepository,
            DocumentFolderProcessor docFolderProcessor,
            CodeFolderProcessor codeFolderProcessor)
        {
            _map = new Dictionary<IndexingStrategy, IFolderProcessor>()
            {
                { IndexingStrategy.Code     , codeFolderProcessor },
                { IndexingStrategy.Documents, docFolderProcessor  },
            };

            _docTypeRepository = docTypeRepository;
        }

        public Task IndexRepositoryAsync(IDocType docType, IProgress<int> progress = null)
        {
            var processor = _map[docType.IndexingStrategy];

            IFolder folder;
            if (!_docTypeRepository.TryGetConfiguredLocalFolder(docType, out folder))
            {
                throw new ArgumentException($"The provided IndexedCollection '{docType.Name}' is not hosted locally. Updating the index not possible!");
            }

            return processor.ProcessAsync(folder, progress);
        }

        public Task IndexRepositoryAsync(IFolder folder, IProgress<int> progress = null)
        {
            var processor = _map[folder.DocType.IndexingStrategy];
            return processor.ProcessAsync(folder, progress);
        }
    }

    public interface IIndexingService
    {
        Task IndexRepositoryAsync(IDocType docType, IProgress<int> progress = null);

        Task IndexRepositoryAsync(IFolder folder, IProgress<int> progress = null);
    }
}
