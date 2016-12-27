using DesktopSearch.Core.Contracts;
using DesktopSearch.Core.Tagging;
using DesktopSearch.PS.Services;
using DesktopSearch.PS.UI;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.PS.Tests.UI
{
    [TestFixture]
    public class KeywordViewModelTests
    {
        [Test]
        public void Added_Keywords_are_stored_in_Keywords_collection()
        {
            var td = CreateTagDescriptor();

            var sut = new KeywordViewModel(td, new SuggestionServiceMock());

            const string someKeyword = "fjdkl";
            sut.KeywordToAdd = someKeyword;
            sut.AddKeywordCommand.Execute(null);

            Assert.IsTrue(sut.Keywords.Any(t => t.Text == someKeyword));
        }

        [Test]
        public void Keywords_are_not_added_multiple_times_to_Keywords_collection()
        {
            string someKeyword = "something";

            var td = CreateTagDescriptor();

            var sm =new SuggestionServiceMock();
            sm.Suggestions.Add(someKeyword);

            var sut = new KeywordViewModel(td, sm);

            sut.KeywordToAdd = someKeyword;
            sut.AddKeywordCommand.Execute(null);

            sut.KeywordToAdd = someKeyword;
            sut.AddKeywordCommand.Execute(null);

            Assert.AreEqual(1, sut.Keywords.Count(k => k.Text == someKeyword));
        }

        [Test]
        public void Suggestions_pulled_from_service_are_exposed_form_SuggestedKeywordsList()
        {
            string someKeyword = "something";

            var td = CreateTagDescriptor();

            var sm = new SuggestionServiceMock();
            sm.Suggestions.Add(someKeyword);

            var sut = new KeywordViewModel(td, sm);

            Assert.IsTrue(sut.SuggestedKeywordsList.Contains(someKeyword));
        }

        [Test]
        public void Keywords_that_are_already_used_are_not_suggested_again()
        {
            var td = CreateTagDescriptor();

            var sut = new KeywordViewModel(td, new SuggestionServiceMock());

            const string someKeyword = "fjdkl";
            sut.KeywordToAdd = someKeyword;
            sut.AddKeywordCommand.Execute(null);

            Assert.IsFalse(sut.SuggestedKeywordsList.Contains(someKeyword));
        }

        private static TagDescriptor CreateTagDescriptor()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add(MetaTypes.Title, "The Title");
            map.Add(MetaTypes.Author, "The Author");
            var tagDesc = new TagDescriptor(map);

            return tagDesc;
        }

        class SuggestionServiceMock : IKeywordSuggestions
        {
            public SuggestionServiceMock()
            {
                this.Suggestions = new List<string>();
            }

            public List<string> Suggestions { get; private set; }

            public Task<IEnumerable<string>> GetSuggestedKeywordsAsync()
            {
                return Task.FromResult<IEnumerable<string>>(this.Suggestions);
            }
        }
    }
}
