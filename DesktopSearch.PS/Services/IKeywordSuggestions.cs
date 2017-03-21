using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopSearch.PS.Services
{
    public interface IKeywordSuggestions
    {
        Task<IEnumerable<string>> GetSuggestedKeywordsAsync();
    }

    public class MockKeywordSuggestionService : IKeywordSuggestions
    {
        public Task<IEnumerable<string>> GetSuggestedKeywordsAsync()
        {
            return Task.FromResult<IEnumerable<string>>(new[] { "SQL", "Test" });
        }
    }

}