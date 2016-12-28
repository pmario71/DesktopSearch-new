using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Configuration
{
    public class ElasticSearchConfig
    {
        public string Uri { get; set; } = "http://localhost:9200";

        public string DocumentSearchIndexName { get; set; } = "docsearch";

        public string CodeSearchIndexName { get; set; } = "codesearch";
    }
}
