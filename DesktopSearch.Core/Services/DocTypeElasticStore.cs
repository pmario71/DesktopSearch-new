using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using Nest;

namespace DesktopSearch.Core.Services
{

    internal class DocTypeElasticStore : IDocTypePersistence
    {
        private IElasticClient _client;

        public DocTypeElasticStore(IElasticClient elasticClient)
        {
            _client = elasticClient;
        }

        public async Task<IEnumerable<DocType>> LoadAsync()
        {
            //_config.DocumentSearchIndexName
            var result = await _client.SearchAsync<DocType>(t => t.Index("")
                .Query(q => q.MatchAll()));

            return result.Hits.Select(s => s.Source);
        }

        public async Task StoreOrUpdateAsync(DocType docType)
        {
            var result = await _client.IndexAsync(docType);
            if (!result.IsValid)
            {
                throw new Exception($"Error storing/updating: {docType.Name}\r\n{result.DebugInformation}");
            }
        }
    }
}
