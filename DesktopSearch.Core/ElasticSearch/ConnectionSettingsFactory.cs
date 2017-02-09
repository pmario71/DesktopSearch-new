using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Documents;
using Microsoft.Extensions.Options;
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
        public static ConnectionSettings Create(IOptions<ElasticSearchConfig> config)
        {
            var settings = new ConnectionSettings(new Uri(config.Value.Uri));
            settings
                .MapDefaultTypeIndices(m => m
                    .Add(typeof(DocDescriptor), config.Value.DocumentSearchIndexName)
                    .Add(typeof(DocumentCollection), config.Value.DocumentSearchIndexName));

            // PrivateFieldResolver   Newtonsoft.Json.Serialization.DefaultContractResolver

            return settings;
        }
    }
}
