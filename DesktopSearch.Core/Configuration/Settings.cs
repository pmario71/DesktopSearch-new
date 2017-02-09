using DesktopSearch.Core.DataModel.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Configuration
{
    public class Settings
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private Uri _elasticSearchUri;

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

        //public FoldersToIndex FoldersToIndex { get; set; }
    }

    //public class FoldersToIndex
    //{
    //    public List<Folder> Folders { get; set; }
    //}
}
