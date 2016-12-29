using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;

namespace DesktopSearch.Core.Services
{
    public class DocTypeRepository : IDocTypeRepository
    {
        private readonly PersistenceStore _store;

        public DocTypeRepository()
        {
            _store = new PersistenceStore();
        }

        public void AddDocType(DocType docType)
        {
            CheckIfAnyLinkedFolderIsInUse(docType);

            _store.Add(docType);
        }

        private void CheckIfAnyLinkedFolderIsInUse(DocType docType)
        {
            var paths = _store.DocTypes
                .SelectMany(dt => dt.Folders);

            if (docType.Folders.Intersect(paths).Count() > 0)
            {
                throw new FolderRootPathException($"A path '' already assigned to DocType!");
            }
        }

        public DocType GetDocTypeByName(string name)
        {
            throw new NotImplementedException();
        }

        internal void AddDocType(object docType)
        {
            throw new NotImplementedException();
        }

        public DocType GetDocTypeForPath(FileInfo file)
        {
            throw new NotImplementedException();
        }
    }


    [Serializable]
    public class FolderRootPathException : Exception
    {
        public FolderRootPathException() { }
        public FolderRootPathException(string message) : base(message) { }
        public FolderRootPathException(string message, Exception inner) : base(message, inner) { }

        protected FolderRootPathException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    internal class PersistenceStore
    {
        private readonly List<DocType> _doctypes;

        public PersistenceStore()
        {
            _doctypes = new List<DocType>();
        }

        public IEnumerable<DocType> DocTypes { get { return _doctypes; } }

        internal void Add(DocType docType)
        {
            _doctypes.Add(docType);
        }
    }

    [Serializable]
    public class DocTypeNotRegisteredException : Exception
    {
        public DocTypeNotRegisteredException() { }
        public DocTypeNotRegisteredException(string message) : base(message) { }
        public DocTypeNotRegisteredException(string message, Exception inner) : base(message, inner) { }

        protected DocTypeNotRegisteredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
