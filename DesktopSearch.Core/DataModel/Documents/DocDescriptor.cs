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
        [String(Name = "path")]
        public string Path { get; set; }

        [Date(Name = "lastmodified")]
        public DateTime LastModified { get; set; }

        //public string Content { get; set; }

        [String(Name = "keywords")]
        public string Keywords { get; set; }

        [Attachment(Name = "attachment", Store = false)]
        public Nest.Attachment Attachment { get; set; }

        public override string ToString()
        {
            return $"{Path} -- {LastModified} --  {Keywords}";
        }
    }
}
