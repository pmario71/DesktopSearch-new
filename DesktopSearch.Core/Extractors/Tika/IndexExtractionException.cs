using System;
using System.Runtime.Serialization;

namespace DesktopSearch.Core.Extractors.Tika
{

    [Serializable]
    public class IndexExtractionException : Exception
    {
        private string fullName;
        private object tikaReturnedNoResults;
        private string v;

        public IndexExtractionException()
        {
        }

        public IndexExtractionException(string message) : base(message)
        {
        }

        public IndexExtractionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public IndexExtractionException(string v, string fullName, object tikaReturnedNoResults)
        {
            this.v = v;
            this.fullName = fullName;
            this.tikaReturnedNoResults = tikaReturnedNoResults;
        }

        protected IndexExtractionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}