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

        [JsonIgnore]
        public Uri ElasticSearchUri
        {
            get
            {
                if (_elasticSearchUri == null)
                {
                    return new Uri("http://localhost:9200");
                }
                return _elasticSearchUri;
            }
            set
            {
                if (value == null)
                    throw new ArgumentException("Null value not cannot be specified as ElasticSearchUri!");
                if (value.Port <= 0)
                    throw new ArgumentException($"No valid port specified: {value}!");

                _elasticSearchUri = value;
            }
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
