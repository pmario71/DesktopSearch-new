using DesktopSearch.Core.DataModel.Documents;
using Nest;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Services
{
    public interface ISearchService
    {
        //Task IndexCodeFileAsync(string codefilePath);

        Task<IEnumerable<string>> GetKeywordSuggestionsAsync(string filter=null);

        Task<IEnumerable<IHit<DocDescriptor>>> SearchDocumentAsync(string querystring);
    }
}