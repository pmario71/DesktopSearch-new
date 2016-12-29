using System;

namespace DesktopSearch.Core.Services
{


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
}
