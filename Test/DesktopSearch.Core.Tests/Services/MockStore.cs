using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;

namespace DesktopSearch.Core.Tests.Services
{

    class NullMockStore : IDocTypePersistence
    {
        public Task<IEnumerable<DocType>> LoadAsync()
        {
            return Task.FromResult<IEnumerable<DocType>>(new DocType[] { });
        }

        public Task StoreOrUpdateAsync(DocType docType)
        {
            return Task.CompletedTask;
        }
    }
}
