using DesktopSearch.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using System.IO;

namespace DesktopSearch.Core.Lucene
{
    class DocumentCollectionLuceneStore : IDocumentCollectionPersistence
    {
        public Task<IEnumerable<IDocumentCollection>> LoadAsync()
        {
            throw new NotImplementedException();
        }

        public Task StoreOrUpdateAsync(IDocumentCollection documentCollection)
        {
            throw new NotImplementedException();
        }
    }
}
