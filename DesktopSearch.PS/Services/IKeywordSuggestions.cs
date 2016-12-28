using DesktopSearch.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace DesktopSearch.PS.Services
{
    public interface IKeywordSuggestions
    {
        Task<IEnumerable<string>> GetSuggestedKeywordsAsync();
    }

    [Export(typeof(IKeywordSuggestions))]
    public class KeywordSuggestionService : IKeywordSuggestions
    {
        private readonly ISearchService _searchService;

        [ImportingConstructor]
        public KeywordSuggestionService(ISearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<IEnumerable<string>> GetSuggestedKeywordsAsync()
        {
            IEnumerable<string> suggestions;
            try
            {
                suggestions = await _searchService.GetKeywordSuggestionsAsync();
            }
            catch(Exception ex)
            {
                suggestions = new[] { "NO_CONNECTION_ELASTIC" };
            }
            return suggestions;
        }
    }

    public class MockKeywordSuggestionService : IKeywordSuggestions
    {
        public Task<IEnumerable<string>> GetSuggestedKeywordsAsync()
        {
            return Task.FromResult<IEnumerable<string>>(new[] { "SQL", "Test" });
        }
    }

}