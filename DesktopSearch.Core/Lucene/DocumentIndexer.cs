using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace DesktopSearch.Core.Lucene
{
    public interface IDocumentIndexer
    {
        Task IndexAsync(IEnumerable<DocDescriptor> extractedDocuments);

        //Task DeleteAllEntries();

        //Task GetStatistics(IndexStatistics stat);

        //IEnumerable<TypeDescriptor> GetIndexedTypes();
        IEnumerable<string> GetIndexedDocuments();
    }

    internal sealed class DocumentIndexer : IDocumentIndexer, IDisposable
    {
        private readonly PerFieldAnalyzerWrapper _analyzer;
        private readonly IndexWriter             _indexWriter;
        private readonly SearcherManager         _searcherManager;
        private readonly QueryParser             _queryParser;


        public DocumentIndexer(IIndexProvider indexProvider)
        {
            var indexDirectory = indexProvider.GetIndexDirectory(IndexType.Document);

            _analyzer = new PerFieldAnalyzerWrapper(new StandardAnalyzer(LuceneVersion.LUCENE_48),
                new Dictionary<string, Analyzer>
                {
                    //{"name", new Analyzers.CamelCaseAnalyzer()},
                    //{"comment", new StandardAnalyzer(LuceneVersion.LUCENE_48)},
                });

            _queryParser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48,
                new[] { "name", "elementtype", "comment" }, _analyzer);

            if (IndexWriter.IsLocked(indexDirectory))
            {
                IndexWriter.Unlock(indexDirectory);
            }

            _indexWriter = new IndexWriter(indexDirectory, new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer));
            _searcherManager = new SearcherManager(_indexWriter, applyAllDeletes: true);
        }

        public void Dispose()
        {
            _indexWriter?.Dispose();
            _searcherManager?.Dispose();
        }

        public Task IndexAsync(IEnumerable<DocDescriptor> extractedDocuments)
        {
            return Task.Run(() =>
            {
                RemoveDeletedFiles();

                foreach (var item in extractedDocuments)
                {
                    var doc = item.FromDocDescriptor();
                    _indexWriter.UpdateDocument(new Term("path", item.Path), doc);
                }

                _indexWriter.Flush(true, true);
                _indexWriter.Commit();
            });
        }


        private void RemoveDeletedFiles()
        {
            IndexReader reader = _indexWriter.GetReader(true);
            for (int i = 0; i < reader.MaxDoc; i++)
            {
                Document doc = reader.Document(i);

                var file = doc.GetField("filepath").StringValue;
                if (!System.IO.File.Exists(file))
                {
                    _indexWriter.DeleteDocuments(new Term("path", doc.GetField("path").StringValue));
                }
            }
        }

        public IEnumerable<string> GetIndexedDocuments()
        {
            var resultDocs = new List<string>();

            _searcherManager.MaybeRefreshBlocking();
            _searcherManager.ExecuteSearch(searcher =>
            {
                var topDocs = searcher.Search(new MatchAllDocsQuery(), 1000);

                foreach (var result in topDocs.ScoreDocs)
                {
                    var doc = searcher.Doc(result.Doc);
                    resultDocs.Add(doc.GetField("title").StringValue);
                }
            }, exception => { Console.WriteLine(exception.ToString()); });

            return resultDocs;
        }
    }
}
