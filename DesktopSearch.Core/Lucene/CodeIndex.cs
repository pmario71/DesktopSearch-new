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

namespace DesktopSearch.Core.Lucene
{
    public class CodeIndex : IDisposable
    {
        private readonly PerFieldAnalyzerWrapper _analyzer;
        private readonly IndexWriter _indexWriter;
        private readonly SearcherManager _searcherManager;
        private readonly QueryParser _queryParser;

        public CodeIndex(Directory indexDirectory)
        {
            _analyzer = new PerFieldAnalyzerWrapper(new StandardAnalyzer(LuceneVersion.LUCENE_48),
                new Dictionary<string, Analyzer>
                {
                    {"comment", new RepositoryNamesAnalyzer()},
                });

            _queryParser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48,
                new[] { "name", "elementtype", "comment" }, _analyzer);

            _indexWriter = new IndexWriter(indexDirectory, new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer));
            _searcherManager = new SearcherManager(_indexWriter, applyAllDeletes:true);
        }


        public void Dispose()
        {
            _indexWriter?.Dispose();
            _searcherManager?.Dispose();
        }
    }
}
