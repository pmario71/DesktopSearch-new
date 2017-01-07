using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;

namespace DesktopSearch.Core.Tests.Services
{

    class NullMockStore : IDocumentCollectionPersistence
    {
        IDocumentCollection[] _documentCollections;

        public IDocumentCollection[] DocumentCollections
        {
            get
            {
                if (_documentCollections == null)
                {
                    _documentCollections = new IDocumentCollection[] { };
                }
                return _documentCollections;
            }
            set => _documentCollections = value;
        }


        public Task<IEnumerable<IDocumentCollection>> LoadAsync()
        {
            return Task.FromResult<IEnumerable<IDocumentCollection>>(this.DocumentCollections);
        }

        public Task StoreOrUpdateAsync(IDocumentCollection collection)
        {
            return Task.CompletedTask;
        }
    }
}
