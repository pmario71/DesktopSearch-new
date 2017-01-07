using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using Nest;
using DesktopSearch.Core.Configuration;
using Newtonsoft.Json;
using static DesktopSearch.Core.Configuration.ConfigAccess;
using System.IO;

namespace DesktopSearch.Core.Services
{

    internal class DocumentCollectionElasticStore : IDocumentCollectionPersistence
    {
        private readonly ElasticSearchConfig _cfg;
        private readonly IElasticClient _client;

        private static JsonSerializerSettings _formatSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ContractResolver = new PrivateFieldResolver()
        };

        public DocumentCollectionElasticStore(IElasticClient elasticClient, ElasticSearchConfig config)
        {
            _client = elasticClient;
            _cfg = config;
        }

        public async Task<IEnumerable<IDocumentCollection>> LoadAsync()
        {
            var result = await _client.SearchAsync<IDocumentCollectionConfigurationElement>(t => t.Index(_cfg.DocumentSearchIndexName).Query(q => q.MatchAll()));

            var collections = new List<IDocumentCollection>();
            foreach (var item in result.Hits.Select(s => s.Source))
            {
                collections.Add(JsonConvert.DeserializeObject<DocumentCollection>(item.Content, _formatSettings));
            }

            return collections;
        }

        public async Task StoreOrUpdateAsync(IDocumentCollection docCollection)
        {
            var serialized = JsonConvert.SerializeObject(docCollection, _formatSettings);

            var myJson = new IDocumentCollectionConfigurationElement
            {
                Id = docCollection.Name,
                Content = serialized
            };
            var result = await _client.IndexAsync<IDocumentCollectionConfigurationElement>(myJson, t => t
                                    .Index(_cfg.DocumentSearchIndexName));

            if (!result.IsValid)
            {
                throw new Exception($"Error storing/updating: {docCollection.Name}\r\n{result.DebugInformation}");
            }
        }
    }

    [ElasticsearchType(Name = "documentcollectionconfigurationelement", IdProperty ="Id")]
    public class IDocumentCollectionConfigurationElement
    {
        [Keyword]
        public string Id { get; set; }

        [Keyword(Index =false)]
        public string Content { get; set; }
    }

    static class StreamEx
    {
        public static byte[] ReadAll(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
