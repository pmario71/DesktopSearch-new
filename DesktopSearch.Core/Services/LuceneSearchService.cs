using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Lucene;
using DesktopSearch.Core.DataModel.Code;
using Lucene.Net.Search;
using Lucene.Net.Index;
using DesktopSearch.Core.Lucene.Utils;

namespace DesktopSearch.Core.Services
{
    internal class LuceneSearchService : ISearchService
    {
        private readonly ICodeSearch     _codeSearch;
        private readonly IDocumentSearch _docSearch;

        public LuceneSearchService(ICodeSearch codeSearch, IDocumentSearch docSearch)
        {
            _codeSearch = codeSearch;
            _docSearch = docSearch;
        }

        public Task<IEnumerable<string>> GetKeywordSuggestionsAsync(string filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DocDescriptor>> SearchDocumentAsync(string queryString)
        {
            return Task.Run(() =>
            {
                var query = _docSearch.Parser.Parse(queryString);
                var totalHits = 0;
                var l = new List<DocDescriptor>();

                // Execute the search with a fresh indexSearcher
                _docSearch.SearchManager.MaybeRefreshBlocking();
                _docSearch.SearchManager.ExecuteSearch(searcher =>
                {
                    var topDocs = searcher.Search(query, 10);
                    totalHits = topDocs.TotalHits;
                    foreach (var result in topDocs.ScoreDocs)
                    {
                        var doc = searcher.Doc(result.Doc);
                        l.Add(doc.ToDocDescriptor());
                    }
                }, exception => { throw exception; });

                return (IEnumerable<DocDescriptor>)l;
            });
        }

        public Task<IEnumerable<TypeDescriptor>> SearchCodeAsync(IDocumentCollection col, string querystring, ElementType? elementType=null)
        {
            return Task.Run(() => Search(col, querystring, elementType));
        }

        private IEnumerable<TypeDescriptor> Search(IDocumentCollection col, string queryString, ElementType? elementType = null)
        {
            var bq = new BooleanQuery();

            var term = new Term("doccollection", col.Name);
            bq.Add(new BooleanClause(new TermQuery(term), BooleanClause.Occur.MUST));

            var query = _codeSearch.Parser.Parse(queryString);
            bq.Add(new BooleanClause(query, BooleanClause.Occur.MUST));

            if (elementType != null)
            {
                
                term = TermHelper.FromInt((int)elementType.Value);
                bq.Add(new BooleanClause(new TermQuery(term), BooleanClause.Occur.MUST));
                query = bq;
            }

            var totalHits = 0;
            var l = new List<TypeDescriptor>();

            // Execute the search with a fresh indexSearcher
            _codeSearch.SearchManager.MaybeRefreshBlocking();
            _codeSearch.SearchManager.ExecuteSearch(searcher =>
            {
                var topDocs = searcher.Search(query, 10);
                totalHits = topDocs.TotalHits;
                foreach (var result in topDocs.ScoreDocs)
                {
                    var doc = searcher.Doc(result.Doc);
                    l.Add(doc.ToTypeDescriptor());
                }
            }, exception => { throw exception; });

            return l;
        }

    }
}
