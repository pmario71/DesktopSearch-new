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
using DesktopSearch.Core.Contracts;

namespace DesktopSearch.Core.Lucene
{
    public interface ICodeIndexer
    {
        Task IndexAsync(IEnumerable<TypeDescriptor> extractedTypes);

        Task GetStatistics(IndexStatistics stat);

        IEnumerable<TypeDescriptor> GetIndexedTypes();
    }

    public interface ICodeSearch
    {
        QueryParser Parser { get; }
        SearcherManager SearchManager { get; }
    }


    public class CodeIndex : ICodeIndexer, ICodeSearch, IDisposable
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

        #region ICodeSearch implementation

        public QueryParser Parser => _queryParser;

        public SearcherManager SearchManager => _searcherManager;

        #endregion

        public Task IndexAsync(IEnumerable<TypeDescriptor> extractedTypes)
        {
            return Task.Run(() =>
            {
                foreach (var item in extractedTypes)
                {
                    var doc = item.From();
                    _indexWriter.UpdateDocument(new Term("id", item.GetTypeID()), doc);
                }

                _indexWriter.Flush(true, true);
                _indexWriter.Commit();
            });
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
                    l.Add(doc.ToTypeDescriptor());
                }
            }, exception => { Console.WriteLine(exception.ToString()); });

            return l;
        }

        public Task GetStatistics(IndexStatistics stat)
        {
            return Task.Run(() =>
            {
                var cs = new CodeStatistics();

                int[] typeHistogram = new int[Enum.GetNames(typeof(ElementType)).Length];
                long apis = 0;

                IndexReader reader = _indexWriter.GetReader(true);
                for (int i = 0; i < reader.MaxDoc; i++)
                {
                    Document doc = reader.Document(i);
                    var td = doc.ToTypeDescriptor();

                    typeHistogram[(int)td.ElementType]++;

                    if (td.APIDefinition == API.Yes)
                        apis++;
                }
                cs.Classes    = typeHistogram[(int)ElementType.Class];
                cs.Interfaces = typeHistogram[(int)ElementType.Interface];
                cs.Enums      = typeHistogram[(int)ElementType.Enum];
                cs.Structs    = typeHistogram[(int)ElementType.Struct];
                cs.Activities = typeHistogram[(int)ElementType.Activity];

                cs.Types = typeHistogram.Sum();
                cs.APIs  = apis;
                stat.Code = cs;
            });
        }
    }


}
