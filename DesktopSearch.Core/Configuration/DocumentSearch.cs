using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Configuration
{
    public class DocumentSearch
    {
        //public const string IndexName = "docsearch";

        //public static ContentType ContentType { get; }

        public class ContentType
        {
            //public const string Dokument = "Dokument";
            public const string Buch = "Buch";
            public const string Artikel = "Artikel";
            public const string Unterlagen = "Unterlagen";
            public const string Rechnungen = "Rechnungen";

            public static string[] ToArray()
            {
                return new[] { Buch, Artikel, Unterlagen, Rechnungen };
            }
        }
    }

    
}
