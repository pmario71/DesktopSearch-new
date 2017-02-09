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
    }


    public class CodeIndex : ICodeIndexer, IDisposable
    {
        private readonly PerFieldAnalyzerWrapper _analyzer;
        private readonly IndexWriter _indexWriter;
        private readonly SearcherManager _searcherManager;
        private readonly QueryParser _queryParser;

        public CodeIndex(IConfigAccess configuration)
            : this(FromConfig(configuration))
        { }

        public CodeIndex(Directory indexDirectory)
        {
            _analyzer = new PerFieldAnalyzerWrapper(new StandardAnalyzer(LuceneVersion.LUCENE_48),
                new Dictionary<string, Analyzer>
                {
                    {"comment", new StandardAnalyzer(LuceneVersion.LUCENE_48)},
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

        public Task IndexAsync(IEnumerable<TypeDescriptor> extractedTypes)
        {
            return Task.Run(() =>
            {
                foreach (var item in extractedTypes)
                {
                    var doc = new Document
                {
                    new StringField("name", item.Name, Field.Store.YES),
                    new StringField("namespace", item.Namespace, Field.Store.YES),
                    new StringField("filepath", item.FilePath, Field.Store.YES),
                    new StringField("elementtype", Enum.GetName(typeof(ElementType), item.ElementType), Field.Store.YES),
                    new IntField("linenr", item.LineNr, Field.Store.YES),
                    new TextField("comment", item.Comment, Field.Store.YES),
                    new StringField("apidefinition", Enum.GetName(typeof(API), item.APIDefinition), Field.Store.YES),
                };
                    _indexWriter.AddDocument(doc);
                }
            });
        }

        private static Directory FromConfig(IConfigAccess configuration)
        {
            var cfg = configuration.Get();
            return FSDirectory.Open(new System.IO.DirectoryInfo(cfg.IndexDirectory));
        }
    }


}
