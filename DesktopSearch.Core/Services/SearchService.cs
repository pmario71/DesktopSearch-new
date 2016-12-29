using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel;
using DesktopSearch.Core.DataModel.Code;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.ElasticSearch;
using DesktopSearch.Core.Extractors.Roslyn;
using DesktopSearch.Core.Processors;
using DesktopSearch.Core.Utils;
using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Services
{
    public class SearchService : ISearchService
    {
        private readonly IElasticClient          _elastic;
        private readonly ElasticSearchConfig     _config;
        private readonly RoslynParser            _roslynParser = new RoslynParser();
        private readonly DocumentFolderProcessor _docFolderProcessor;

        Task EnsureInitialized = null;

        #region CTOR
        public SearchService(IElasticClient elastic,
                             ElasticSearchConfig config,
                             ManagementService mgtSvc,
                             DocumentFolderProcessor docFolderProcessor)
        {
            //TODO: replace ElasticSearchConfig with original ConnectionSettings
            //var settings = new ConnectionSettings(new Uri(_eleasticSearchConfig.Uri));
            //var elastic = new ElasticClient(settings);

            EnsureInitialized = mgtSvc.EnsureIndicesCreated();

            _elastic = elastic;
            _config = config;
            _docFolderProcessor = docFolderProcessor;
        }
        #endregion

        #region Initialization
        //private async Task<ElasticClient> InitializeAsync()
        //{
        //    var settings = new ConnectionSettings(new Uri(_eleasticSearchConfig.Uri));
        //    settings.DefaultIndex(Configuration.CodeSearch.IndexName);

        //    var elastic = new ElasticClient(settings);

        //    var res = await elastic.IndexExistsAsync(Configuration.CodeSearch.IndexName);

        //    if (!res.IsValid && !res.Exists)
        //    {
        //        await EnsureIndicesCreated();
        //    }

        //    return elastic;
        //}

        
        #endregion

        #region API
        public async Task IndexDocumentAsync(string documentPath, string indexingTypeName)
        {
            await EnsureInitialized;

            await _docFolderProcessor.Process(documentPath, indexingTypeName);
        }

        public async Task IndexDocumentAsync(DocDescriptor document)
        {
            await EnsureInitialized;

            await _docFolderProcessor.Process(document);
        }

        public async Task IndexCodeFileAsync(string codefilePath)
        {
            await EnsureInitialized;

            using (StreamReader sr = new StreamReader(new FileStream(codefilePath, FileMode.Open, FileAccess.Read)))
            {
                String fileContent = await sr.ReadToEndAsync();
                var extractedTypes = _roslynParser.ExtractTypes(fileContent);

                var elastic = _elastic;
                var response = await elastic.IndexManyAsync(extractedTypes, CodeSearch.IndexName);

                if (!response.IsValid)
                {
                    throw new Exception($"Failed to index code file: '{codefilePath}'", response.OriginalException);
                }
            }
        }

        public async Task<IEnumerable<string>> GetKeywordSuggestionsAsync(string filter=null)
        {
            await EnsureInitialized;

            var keywords = new[] { "SQL", "Test", "Balloon" };

            if (filter != null)
            {
                return keywords.Where(e => e.StartsWith(filter));
            }
            return keywords;
        }

        public async Task<DocDescriptor> GetDocumentAsync(string id)
        {
            await EnsureInitialized;

            var result = await _elastic.SearchAsync<DocDescriptor>(t => t.Index(_config.DocumentSearchIndexName).Query(q => q.Term(p => p.Path, id)));
            return result.Documents.First();
        }

        public async Task<IEnumerable<IHit<DocDescriptor>>> SearchDocumentAsync(string querystring)
        {
            await EnsureInitialized;

            var result = await _elastic.SearchAsync<DocDescriptor>(t => t.Index(_config.DocumentSearchIndexName)
                                                                         .Query(q => q.QueryString(c => c.Query(querystring))));
            return result.Hits;
        }
        #endregion

        private void xx(ElasticClient elastic)
        {
            var indexSettings = new IndexSettings();

            var customAnalyzer = new CustomAnalyzer()
            {
                Tokenizer = "keyword",
                Filter = new[] { "lowercase" }
            };

            indexSettings.Analysis.Analyzers.Add("custom_lowercase_analyzer", customAnalyzer);

            //var analyzerRes = elastic.CreateIndex("", ci => ci
            //   .Index("my_third_index")
            //   //.InitializeUsing(indexSettings)
            //   .AddMapping<TypeDescriptor>(m => m.MapFromAttributes())
            //   .AddMapping<MethodDescriptor>(m => m.MapFromAttributes()));
        }
    }
}
