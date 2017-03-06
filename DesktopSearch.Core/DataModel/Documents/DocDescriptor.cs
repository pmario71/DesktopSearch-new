using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopSearch.Core.DataModel.Documents
{
    [ElasticsearchType(Name = "docdescriptor", IdProperty ="path")]
    public class DocDescriptor
    {
        [Text(Name = "title", Store = true, Boost = 1.5)]
        public string Title { get; set; }

        [Text(Name = "author", Store = true)]
        public string Author { get; set; }

        [Keyword(Name = "path", Store = true)]
        public string Path { get; set; }

        [Date(Name = "lastmodified", Store = true)]
        public DateTime LastModified { get; set; }

        [Keyword(Name = "keywords", Store = true, Boost = 1.3)]
        public string[] Keywords { get; set; }

        /// <summary>
        /// Buch, Artikel, Rechnung, Unterlagen
        /// </summary>
        [Keyword(Name = "documentcollection")]
        public string DocumentCollection { get; set; }

        /// <summary>
        /// mime-type returned from Tika
        /// </summary>
        [Keyword(Name = "contenttype")]
        public string ContentType { get; set; }

        public string LanguageID { get; set; }

        [Number(Name = "rating", Boost = 1.1)]
        public int Rating { get; set; }

        [Text(Store = false)]
        public string Content { get; set; }

        public TimeSpan ExtractionDuration { get; set; }

        public ErrorState Error { get; private set; }

        public override string ToString()
        {
            return $"{Path} -- {LastModified} --  {Keywords}";
        }

        public static DocDescriptor UnsupportedFileType(string filePath)
        {
            var doc = new DocDescriptor();
            doc.Path = filePath;
            doc.Error = ErrorState.UnsupportedFileType;
            return doc;
        }
    }

    public enum ErrorState
    {
        None,
        UnsupportedFileType,
    }
}
