using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Utils.Async;

namespace DesktopSearch.Core.Services
{
    internal class DocTypeRepository : IDocTypeRepository
    {
        //private readonly List<DocType> _doctypes;
        private readonly IDocTypePersistence _store;

        Task<List<IDocType>> _docTypesCache;

        public DocTypeRepository(IDocTypePersistence persistenceStore)
        {
            _store = persistenceStore;

            _docTypesCache = Task.Run(() =>
            {
                 return new List<IDocType>(_store.LoadAsync().Result);
            });
        }

        public void AddDocType(IDocType docType)
        {
            CheckIfAnyLinkedFolderIsInUse(docType);

            _docTypesCache.Result.Add(docType);
            _store.StoreOrUpdateAsync(docType).Wait();
        }

        public IDocType GetDocTypeByName(string name)
        {
            return _docTypesCache.Result.Single(p => StringComparer.OrdinalIgnoreCase.Compare(p.Name, name) == 0);
        }
        
        public bool TryGetDocTypeForPath(FileInfo file, out IDocType docType)
        {
            if (file == null || !Path.IsPathRooted(file.FullName))
                throw new ArgumentException("file");

            foreach (var dt in _docTypesCache.Result)
            {
                if (dt.Folders.SingleOrDefault(i => file.FullName.StartsWith(i.Path, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    docType = dt;
                    return true;
                }
            }

            docType = null;
            return false;
        }

        #region Internals
        private void CheckIfAnyLinkedFolderIsInUse(IDocType docType)
        {
            var paths = _docTypesCache.Result
                .SelectMany(dt => dt.Folders);

            if (docType.Folders.Intersect(paths).Count() > 0)
            {
                throw new FolderRootPathException($"A path '' already assigned to DocType!");
            }
        }
        #endregion
    }

    internal interface IDocTypePersistence
    {
        Task<IEnumerable<IDocType>> LoadAsync();

        Task StoreOrUpdateAsync(IDocType docType);
    }
}
