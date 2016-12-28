using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Services
{
    public interface ISearchService
    {
        Task IndexDocumentAsync(string documentPath,string indexingTypeName);
        Task IndexCodeFileAsync(string codefilePath);

        Task<IEnumerable<string>> GetKeywordSuggestionsAsync(string filter=null);
    }
}