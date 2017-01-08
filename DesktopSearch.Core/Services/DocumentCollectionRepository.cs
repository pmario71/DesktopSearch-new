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
    public class DocumentCollectionRepository : IDocumentCollectionRepository
    {
        private readonly IDocumentCollectionPersistence _store;

        Task<ValidatingCollection<IDocumentCollection>> _docCollectionCache;

        public DocumentCollectionRepository(IDocumentCollectionPersistence persistenceStore)
        {
            _store = persistenceStore;

            _docCollectionCache = Task.Run(() =>
            {
                var list = new ValidatingCollection<IDocumentCollection>(_store.LoadAsync().Result, this.Validate);
                return list;
            });
        }

        public void AddDocumentCollection(IDocumentCollection documentCollection)
        {
            if (documentCollection == null)
                throw new ArgumentNullException(nameof(documentCollection));

            if (_docCollectionCache.Result.Contains(documentCollection))
                throw new Exception($"A collection with name '{documentCollection.Name}' does already exist!");

            CheckIfAnyLinkedFolderIsInUse(documentCollection);

            _docCollectionCache.Result.Add(documentCollection);
            _store.StoreOrUpdateAsync(documentCollection).Wait();
        }

        public bool TryGetDocumentCollectionByName(string name, out IDocumentCollection documentCollection)
        {
            documentCollection = _docCollectionCache.Result.SingleOrDefault(p => StringComparer.OrdinalIgnoreCase.Compare(p.Name, name) == 0);
            return documentCollection != null;
        }

        public bool TryGetFolderForPath(FileInfo file, out IFolder folder)
        {
            if (file == null || !Path.IsPathRooted(file.FullName))
                throw new ArgumentException(nameof(file));

            foreach (var dt in _docCollectionCache.Result)
            {
                IFolder folder1 = dt.Folders.SingleOrDefault(i => file.FullName.StartsWith(i.Path, StringComparison.OrdinalIgnoreCase));
                if (folder1 != null)
                {
                    folder = folder1;
                    return true;
                }
            }

            folder = null;
            return false;
        }

        public bool TryGetDocumentCollectionForPath(FileInfo file, out IDocumentCollection documentCollection)
        {
            IFolder folder;
            bool result = TryGetFolderForPath(file, out folder);

            documentCollection = (result) ? folder.DocumentCollection : null;
            return result;
        }

        public bool TryGetConfiguredLocalFolder(IDocumentCollection documentCollection, out IFolder localFolder)
        {
            string machinename = Environment.MachineName;
            localFolder = ((DocumentCollection)documentCollection).Folders.SingleOrDefault(f => f.Machinename == machinename);
            return localFolder != null;
        }

        public IFolder AddFolderToDocumentCollection(IDocumentCollection documentCollection, string path)
        {
            if (documentCollection == null)
                throw new ArgumentNullException(nameof(documentCollection));

            var folder = Folder.Create(path);
            ValidateBeforeAdding(folder);
            folder.DocumentCollection = documentCollection;  //TODO: unfortunate, that back link on folder needs to be manually set!
            ((DocumentCollection)documentCollection).Folders.Add(folder);

            _store.StoreOrUpdateAsync(documentCollection).Wait();

            return folder;
        }

        public IEnumerable<IDocumentCollection> GetIndexedCollections()
        {
            return this._docCollectionCache.Result;
        }

        private void Validate(IDocumentCollection documentCollection)
        {
            CheckIfAnyLinkedFolderIsInUse(documentCollection);
        }

        private void ValidateBeforeAdding(IFolder folder)
        {
            var paths = _docCollectionCache.Result
                .SelectMany(dt => dt.Folders);

            // none of the already existing folders must be a parent or a child folder of the folder to be added

            var childOrParent = paths.FirstOrDefault(p => 
            (
                folder.Path.Contains(p.Path) || p.Path.Contains(folder.Path)
            ));
            if (childOrParent != null)
            {
                string msg = $@"Folder {folder.Path} cannot be added!
It would conflict with a folder belonging to DocumentCollection '{childOrParent.DocumentCollection.Name}'. 
New folders neither can be a child or a parent of an already existing folder!
";
                throw new FolderRootPathException(msg);
            }
        }

        #region Internals
        private void CheckIfAnyLinkedFolderIsInUse(IDocumentCollection documentCollection)
        {
            var paths = _docCollectionCache.Result
                .SelectMany(dt => dt.Folders);

            if (documentCollection.Folders.Intersect(paths).Count() > 0)
            {
                throw new FolderRootPathException($"A path '' already assigned to DocumentCollection!");
            }
        }
        #endregion
    }

    public interface IDocumentCollectionPersistence
    {
        Task<IEnumerable<IDocumentCollection>> LoadAsync();

        Task StoreOrUpdateAsync(IDocumentCollection documentCollection);
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
