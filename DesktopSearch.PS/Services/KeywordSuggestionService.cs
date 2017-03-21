using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using DesktopSearch.Core.Services;
using DesktopSearch.Core.Utils;
using DesktopSearch.PS.Utils;
using Microsoft.Extensions.Logging;

namespace DesktopSearch.PS.Services
{
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
                suggestions = new string[0];

                var logger = Logging.GetLogger<KeywordSuggestionService>();
                logger.LogWarning(new EventId(PSLoggedIds.FailedToGetKeywordSuggestions), 
                    "Failed to get keyword suggestions!", 
                    ex);
            }
            return suggestions;
        }
    }
}