using DesktopSearch.Core.DataModel.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Configuration
{
    public class LuceneConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private Uri _elasticSearchUri;

        [JsonProperty]
        Tools _Tools;

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
                if (_Tools == null)
                {
                    _Tools = new Tools();
                }
                return _Tools;
            }
        }
    }

    public class Tools
    {
        [JsonProperty]
        private string _luke;

        public string Luke
        {
            get => _luke;
            set
            {
                if (!File.Exists(value))
                {
                    throw new ArgumentException($"File does not exist: {value}!");
                }
                if (Path.GetExtension(value).ToLower() != ".jar" || !Path.GetFileNameWithoutExtension(value).ToLower().Contains("luke"))
                {
                    throw new ArgumentException($"'{Path.GetFileName(value)}' does not seem to be a valid Luke version!");
                }
                _luke = value;
            }
        }
    }
}
