using DesktopSearch.Core.DataModel.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Configuration
{
    public class LuceneConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private Uri _elasticSearchUri;

        [JsonProperty]
        Tools _tools;

        [JsonProperty]
        private DocumentIndexing _documentIndexing;

        public LuceneConfig()
        {
            this.IndexDirectory = "Indices\\";
            this.DocumentCollections = new DocumentCollection[] { };
        }

        [JsonProperty]
        public string IndexDirectory { get; set; }

        [JsonProperty]
        public DocumentCollection[] DocumentCollections { get; set; }

        public Tools Tools
        {
            get
            {
                if (_tools == null)
                {
                    _tools = new Tools();
                }
                return _tools;
            }
        }

        public DocumentIndexing DocumentIndexing
        {
            get
            {
                if (_documentIndexing==null)
                {
                    _documentIndexing=new DocumentIndexing();
                }
                return _documentIndexing;
            }
        }
    }
}
