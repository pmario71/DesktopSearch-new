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
        private ICodeSearch _codeSearch;

        public LuceneSearchService(ICodeSearch codeSearch)
        {
            _codeSearch = codeSearch;
        }

        public Task<IEnumerable<string>> GetKeywordSuggestionsAsync(string filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DocDescriptor>> SearchDocumentAsync(string querystring)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TypeDescriptor>> SearchCodeAsync(string querystring, ElementType? elementType=null)
        {
            return Task.Run(() => Search(querystring, elementType));
        }

        private IEnumerable<TypeDescriptor> Search(string queryString, ElementType? elementType = null)
        {
            //Console.WriteLine($"Number of docs stored: {_indexWriter.NumDocs()}");

            var query = _codeSearch.Parser.Parse(queryString);
            if (elementType != null)
            {
                var bq = new BooleanQuery();
                bq.Add(new BooleanClause(query, BooleanClause.Occur.MUST));
                var term = TermHelper.FromInt((int)elementType.Value);
                bq.Add(new BooleanClause(new TermQuery(term), BooleanClause.Occur.MUST));
                query = bq;
            }
            //var query = new TermQuery(new Term(queryString));

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
                    l.Add(new TypeDescriptor(
                        (ElementType)doc.GetField("elementtype").NumericValue,
                        doc.GetField("name")?.StringValue,
                        (Visibility)doc.GetField("visibility").NumericValue,
                        doc.GetField("namespace").StringValue,
                        doc.GetField("filepath").StringValue,
                        (int)doc.GetField("linenr").NumericValue,
                        doc.GetField("comment")?.StringValue));
                }
            }, exception => { throw exception; });

            return l;
        }

    }
}
