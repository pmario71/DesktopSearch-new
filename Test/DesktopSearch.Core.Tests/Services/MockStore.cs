using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;

namespace DesktopSearch.Core.Tests.Services
{

    class NullMockStore : IDocTypePersistence
    {
        public Task<IEnumerable<IDocType>> LoadAsync()
        {
            return Task.FromResult<IEnumerable<IDocType>>(new IDocType[] { });
        }

        public Task StoreOrUpdateAsync(IDocType docType)
        {
            return Task.CompletedTask;
        }
    }
}
