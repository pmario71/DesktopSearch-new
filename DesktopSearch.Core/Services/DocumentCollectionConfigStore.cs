using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Configuration;

namespace DesktopSearch.Core.Services
{
    internal class DocumentCollectionConfigStore : IDocumentCollectionPersistence
    {
        private IConfigAccess<LuceneConfig> _config;

        public DocumentCollectionConfigStore(IConfigAccess<LuceneConfig> conf)
        {
            _config = conf;
        }
        public Task<IEnumerable<IDocumentCollection>> LoadAsync()
        {
            var cfg = _config.Get();
            return Task.FromResult<IEnumerable<IDocumentCollection>>(cfg.DocumentCollections);
        }

        public Task StoreOrUpdateAsync(IDocumentCollection documentCollection)
        {
            var cfg = _config.Get();

            var collections = cfg.DocumentCollections.ToList();
            var idx = collections.FindIndex(dc => (dc.Name == documentCollection.Name));
            if (idx >= 0)
            {
                collections.RemoveAt(idx);
            }
            collections.Add(documentCollection);

            cfg.DocumentCollections = collections.ToArray();
            _config.Save(cfg);

            return Task.CompletedTask;
        }
    }


}
