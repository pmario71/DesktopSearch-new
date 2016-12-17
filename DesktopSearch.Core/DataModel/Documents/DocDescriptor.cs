using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopSearch.Core.DataModel.Documents
{
    [ElasticsearchType(Name = "docdescriptor")]
    public class DocDescriptor
    {
        [Text(Name = "title", Boost = 1.5)]
        public string Title { get; set; }

        [Text(Name = "author")]
        public string Author { get; set; }

        [Text(Name = "path")]
        public string Path { get; set; }

        [Date(Name = "lastmodified")]
        public DateTime LastModified { get; set; }

        //public string Content { get; set; }

        [Text(Name = "keywords", Boost = 1.3)]
        public string Keywords { get; set; }

        [Text(Name = "contenttype", Boost = 1.3)]
        public string ContentType { get; set; }
        
        [Number(Name = "rating", Boost = 1.1)]
        public int Rating { get; set; }

        [Attachment(Name = "attachment", Store = false)]
        public Nest.Attachment Attachment { get; set; }

        [Text(Store = false)]
        public string Content { get; set; }

        public TimeSpan ExtractionDuration { get; set; }

        public override string ToString()
        {
            return $"{Path} -- {LastModified} --  {Keywords}";
        }
    }
}
