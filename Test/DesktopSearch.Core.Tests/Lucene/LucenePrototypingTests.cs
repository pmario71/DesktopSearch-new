using DesktopSearch.Core.DataModel.Code;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Tests.Lucene
{
    [TestFixture]
    public class LucenePrototypingTests
    {
        Analyzer _analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);

        [Test]
        public void Can_index_string()
        {
            string comment = "The quick brown fox jumps over the lazy ...";
            var indexDirectory = new RAMDirectory();
            IndexWriterConfig indexWriterConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer);

            var indexWriter = new IndexWriter(indexDirectory, indexWriterConfig);
            var searcherManager = new SearcherManager(indexWriter, applyAllDeletes: true);

            string name = "SomeTestClass";
            var doc = new Document
                    {
                        new TextField("name", name, Field.Store.YES),
                        new IntField("elementtype", (int)ElementType.Class, Field.Store.YES),
                        new TextField("comment", comment, Field.Store.YES),
                    };
            indexWriter.UpdateDocument(new Term("name", name), doc);

            indexWriter.Flush(true, true);
            indexWriter.Commit();


            var qp = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, new[] { "name", "elementtype", "comment" }, _analyzer);


            // ----------------------------------------------------------------------
            Assert.AreEqual(1, Query(searcherManager, qp.Parse("name:sometestclass")));

            // ----------------------------------------------------------------------
            Assert.AreEqual(1, Query(searcherManager, qp.Parse("name:some*")));

            // ----------------------------------------------------------------------
            Assert.AreEqual(1, Query(searcherManager, qp.Parse("some*")));

            // ----------------------------------------------------------------------
            BytesRef bytes = new BytesRef(NumericUtils.BUF_SIZE_INT);
            NumericUtils.IntToPrefixCoded(0, 0, bytes);
            Term term = new Term("elementtype", bytes);
            Assert.AreEqual(1, Query(searcherManager, new TermQuery(term)));  // not working

            // ----------------------------------------------------------------------
            Assert.AreEqual(1, Query(searcherManager, NumericRangeQuery.NewIntRange("elementtype", 0, 0, true, true)));
        }

        private int Query(SearcherManager _searcherManager, Query query)
        {
            int nResults = 0;
            _searcherManager.MaybeRefreshBlocking();
            _searcherManager.ExecuteSearch(searcher =>
            {
                var topDocs = searcher.Search(query, 10);
                nResults = topDocs.TotalHits;

                foreach (var result in topDocs.ScoreDocs)
                {
                    var doc = searcher.Doc(result.Doc);
                    //Console.WriteLine($"{result.Score} - {doc.GetField("name")?.StringValue} - {doc.GetField("elementtype")?.NumericValue}");
                }
            }, exception => { Console.WriteLine(exception.ToString()); });
            Console.WriteLine($"{query.ToString()}  - Hits: {nResults}");
            return nResults;
        }
    }
}
