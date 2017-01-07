using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Documents;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.ElasticSearch
{
    public class ConnectionSettingsFactory
    {
        public static ConnectionSettings Create(ElasticSearchConfig config)
        {
            var settings = new ConnectionSettings(new Uri(config.Uri));
            settings
                .MapDefaultTypeIndices(m => m
                    .Add(typeof(DocDescriptor), config.DocumentSearchIndexName)
                    .Add(typeof(DocumentCollection), config.DocumentSearchIndexName));

            // PrivateFieldResolver   Newtonsoft.Json.Serialization.DefaultContractResolver

            return settings;
        }
    }
}
