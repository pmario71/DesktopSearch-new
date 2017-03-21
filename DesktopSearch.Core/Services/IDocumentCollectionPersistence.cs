using System.Collections.Generic;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;

namespace DesktopSearch.Core.Services
{
    public interface IDocumentCollectionPersistence
    {
        Task<IEnumerable<IDocumentCollection>> LoadAsync();

        Task StoreOrUpdateAsync(IDocumentCollection documentCollection);
        void Remove(IEnumerable<string> enumerable);
    }
}
