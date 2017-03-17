using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopSearch.Core.DataModel.Documents
{

    public class DocDescriptor
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public string Path { get; set; }

        public DateTime LastModified { get; set; }

        public string[] Keywords { get; set; }

        /// <summary>
        /// Buch, Artikel, Rechnung, Unterlagen
        /// </summary>
        public string DocumentCollection { get; set; }

        /// <summary>
        /// mime-type returned from Tika
        /// </summary>
        public string ContentType { get; set; }

        public string LanguageID { get; set; }

        public int Rating { get; set; }

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
