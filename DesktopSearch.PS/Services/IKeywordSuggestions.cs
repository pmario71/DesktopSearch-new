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
        public Task<IEnumerable<string>> GetSuggestedKeywordsAsync()
        {
            return Task.FromResult<IEnumerable<string>>(new[] { "SQL", "Test" });
        }
    }
}