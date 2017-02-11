using DesktopSearch.Core.Lucene.Analyzers;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Code;
using Lucene.Net.Documents;
using DesktopSearch.Core.Configuration;

namespace DesktopSearch.Core.Lucene
{
    public interface ICodeIndexer
    {
        Task IndexAsync(IEnumerable<TypeDescriptor> extractedTypes);

        IEnumerable<TypeDescriptor> GetIndexedTypes();
    }


    public class CodeIndex : ICodeIndexer, IDisposable
    {
        private readonly PerFieldAnalyzerWrapper _analyzer;
        private readonly IndexWriter _indexWriter;
        private readonly SearcherManager _searcherManager;
        private readonly QueryParser _queryParser;


        public CodeIndex(IIndexProvider indexProvider)
        {
            var indexDirectory = indexProvider.GetIndexDirectory();

            _analyzer = new PerFieldAnalyzerWrapper(new StandardAnalyzer(LuceneVersion.LUCENE_48),
                new Dictionary<string, Analyzer>
                {
                    {"comment", new StandardAnalyzer(LuceneVersion.LUCENE_48)},
                });

            _queryParser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48,
                new[] { "name", "elementtype", "comment" }, _analyzer);

            if (IndexWriter.IsLocked(indexDirectory))
            {
                IndexWriter.Unlock(indexDirectory);
            }

            _indexWriter = new IndexWriter(indexDirectory, new IndexWriterConfig(LuceneVersion.LUCENE_48, new StandardAnalyzer(LuceneVersion.LUCENE_48)));
            _searcherManager = new SearcherManager(_indexWriter, applyAllDeletes:true);
        }


        public void Dispose()
        {
            _indexWriter?.Dispose();
            _searcherManager?.Dispose();
        }

        public Task IndexAsync(IEnumerable<TypeDescriptor> extractedTypes)
        {
            return Task.Run(() =>
            {
                foreach (var item in extractedTypes)
                {
                    var doc = DocumentConverter.From(item);
                    _indexWriter.UpdateDocument(new Term("id", DocumentConverter.GetTypeID(item)), doc);
                }

                _indexWriter.Flush(true, true);
                _indexWriter.Commit();
            });
        }

        public IEnumerable<TypeDescriptor> Search(string queryString)
        {
            Console.WriteLine($"Number of docs stored: {_indexWriter.NumDocs()}");

            var query = _queryParser.Parse(queryString);
            //var query = new TermQuery(new Term(queryString));

            var totalHits = 0;
            var l = new List<TypeDescriptor>();

            // Execute the search with a fresh indexSearcher
            _searcherManager.MaybeRefreshBlocking();
            _searcherManager.ExecuteSearch(searcher =>
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
            }, exception => { Console.WriteLine(exception.ToString()); });
                        
            return l;
        }

        private static Directory FromConfig(IConfigAccess configuration)
        {
            var cfg = configuration.Get();
            return FSDirectory.Open(new System.IO.DirectoryInfo(cfg.IndexDirectory));
        }

        public IEnumerable<TypeDescriptor> GetIndexedTypes()
        {
            var l = new List<TypeDescriptor>();

            _searcherManager.MaybeRefreshBlocking();
            _searcherManager.ExecuteSearch(searcher =>
            {
                var topDocs = searcher.Search(new MatchAllDocsQuery(), 1000);

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
            }, exception => { Console.WriteLine(exception.ToString()); });

            return l;
        }
    }


}
