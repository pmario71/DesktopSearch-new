using Lucene.Net.Analysis;
using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Analysis.Util;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using NUnit.Framework;
using DesktopSearch.Core.Lucene.Analyzers;

namespace DesktopSearch.Core.Tests.Lucene
{
    public class CamelCaseTokenizingTests
    {

        [TestCase("Test", new[] {"test"})]
        [TestCase("TestCase", new[] {"test", "case"})]
        [TestCase("testCase", new[] {"test", "case"})]
        public void TokenizeTests(string input, string[] expectedOutput)
        {
            var sut = new CamelCaseTokenizer(LuceneVersion.LUCENE_48, new StringReader(input));

            int idx = 0;
            sut.Reset();

            while (sut.IncrementToken())
            {
                string term = sut.GetAttribute<ICharTermAttribute>().ToString();
                Assert.AreEqual(expectedOutput[idx++], term);
            }

            sut.End();
        }

        [Test]
        public void Index_using_CamelCaseAnalyzer()
        {
            var indexDirectory = new RAMDirectory();
            var sut = new CamelCaseAnalyzer();
            IndexWriterConfig indexWriterConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, sut);

            var indexWriter = new IndexWriter(indexDirectory, indexWriterConfig);

            var doc = new Document { new TextField("name", "TestCaseAttribute", Field.Store.YES) };
            indexWriter.AddDocument(doc);

            doc = new Document { new TextField("name", "fieldRoadNonsense", Field.Store.YES) };
            indexWriter.AddDocument(doc);

            indexWriter.Flush(true, true);
            indexWriter.Commit();

            Console.WriteLine("--- index written ---");

            var qp = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, new[] { "name", "elementtype", "comment" },
                sut);
            var searcherManager = new SearcherManager(indexWriter, applyAllDeletes: true);

            var result = LucenePrototypingTests.Query(searcherManager, qp.Parse("case"));
            Assert.AreEqual(1, result);

            result = LucenePrototypingTests.Query(searcherManager, qp.Parse("field"));
            Assert.AreEqual(1, result);

            result = LucenePrototypingTests.Query(searcherManager, qp.Parse("nonsense"));
            Assert.AreEqual(1, result);
        }

        

        //public sealed class CamelCaseFilter : TokenFilter
        //{
        //    private readonly ICharTermAttribute _termAtt;

        //    public CamelCaseFilter(TokenStream input) : base(input)
        //    {
        //        this._termAtt = AddAttribute<ICharTermAttribute>();
        //    }

        //    public override bool IncrementToken()
        //    {
        //        if (!input.IncrementToken())
        //            return false;

        //        ICharTermAttribute a = this.GetAttribute<ICharTermAttribute>();

        //        IEnumerable<string> splittedString = Tokenize(a.ToString());

        //        _termAtt.SetEmpty();
        //        foreach (var token in splittedString)
        //        {
        //            _termAtt.Append(token);
        //        }

        //        return true;
        //    }

        //    internal static IEnumerable<string> Tokenize(string stringToTokenize)
        //    {
        //        int start = 0;
        //        int current = 0;
        //        foreach (char c in stringToTokenize)
        //        {
        //            if (Char.IsUpper(c))
        //            {
        //                if ((current - start) > 0)
        //                {
        //                    int tStart = start;
        //                    start = current;
        //                    yield return stringToTokenize.Substring(tStart, current);
        //                }
        //            }
        //            else
        //            {
        //                if (current == stringToTokenize.Length - 1)
        //                {
        //                    yield return stringToTokenize.Substring(start, current-start+1);
        //                }
        //            }
        //            current++;
        //        }
        //    }
        //}
    }
}
