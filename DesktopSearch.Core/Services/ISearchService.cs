using DesktopSearch.Core.DataModel.Code;
using DesktopSearch.Core.DataModel.Documents;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Services
{
    public interface ISearchService
    {
        //Task IndexCodeFileAsync(string codefilePath);

        Task<IEnumerable<string>> GetKeywordSuggestionsAsync(string filter=null);

        Task<IEnumerable<DocDescriptor>> SearchDocumentAsync(string querystring);

        Task<IEnumerable<TypeDescriptor>> SearchCodeAsync(IDocumentCollection collection, string querystring, ElementType? elementType = null);
    }
}