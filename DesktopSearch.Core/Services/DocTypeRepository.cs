using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Utils.Async;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace DesktopSearch.Core.Services
{
    internal class DocTypeRepository : IDocTypeRepository
    {
        //private readonly List<DocType> _doctypes;
        private readonly IDocTypePersistence _store;

        Task<ValidatingCollection<IDocType>> _docTypesCache;

        public DocTypeRepository(IDocTypePersistence persistenceStore)
        {
            _store = persistenceStore;

            _docTypesCache = Task.Run(() =>
            {
                var list = new ValidatingCollection<IDocType>(_store.LoadAsync().Result, this.Validate);
                return list;
            });
        }

        public void AddDocType(IDocType docType)
        {
            CheckIfAnyLinkedFolderIsInUse(docType);

            _docTypesCache.Result.Add(docType);
            _store.StoreOrUpdateAsync(docType).Wait();
        }

        public bool TryGetDocTypeByName(string name, out IDocType docType)
        {
            docType = _docTypesCache.Result.SingleOrDefault(p => StringComparer.OrdinalIgnoreCase.Compare(p.Name, name) == 0);
            return docType != null;
        }
        
        public bool TryGetDocTypeForPath(FileInfo file, out IDocType docType)
        {
            if (file == null || !Path.IsPathRooted(file.FullName))
                throw new ArgumentException(nameof(file));

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

        public bool TryGetConfiguredLocalFolder(IDocType docType, out IFolder localFolder)
        {
            string machinename = Environment.MachineName;
            localFolder = ((DocType)docType).Folders.SingleOrDefault(f => f.Machinename == machinename);
            return localFolder != null;
        }

        public void AddFolderToDocType(IDocType docType, string path)
        {
            if (docType == null)
                throw new ArgumentNullException(nameof(docType));

            var folder = Folder.Create(path);
            ValidateBeforeAdding(folder);
            ((DocType)docType).Folders.Add(folder);
        }

        public IEnumerable<IDocType> GetIndexedCollections()
        {
            return this._docTypesCache.Result;
        }

        private void Validate(IDocType docType)
        {
            CheckIfAnyLinkedFolderIsInUse(docType);
        }

        private void ValidateBeforeAdding(IFolder folder)
        {
            var paths = _docTypesCache.Result
                .SelectMany(dt => dt.Folders);

            // none of the already existing folders must be a parent or a child folder of the folder to be added

            var childOrParent = paths.FirstOrDefault(p => 
            (
                folder.Path.Contains(p.Path) || p.Path.Contains(folder.Path)
            ));
            if (childOrParent != null)
            {
                string msg = $@"Folder {folder.Path} cannot be added!\r\nIt would conflict with a folder belonging to DocType 
'{childOrParent.DocType.Name}'. New folders neither can be a child or a parent of an already existing folder!
";
                throw new FolderRootPathException(msg);
            }
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

    class ValidatingCollection<T> : ObservableCollection<T>
    {
        private readonly Action<T> _validator;

        public ValidatingCollection(IEnumerable<T> items, Action<T> validator) : base(items)
        {
            _validator = validator;
        }

        protected override void InsertItem(int index, T item)
        {
            _validator(item);
            base.InsertItem(index, item);
        }
    }
}
