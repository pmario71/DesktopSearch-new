using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.ElasticSearch;
using DesktopSearch.Core.Processors;
using DesktopSearch.Core.Services;
using DesktopSearch.Core.Tests.Utils;
using Nest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DesktopSearch.Core.Configuration.DocumentSearch;
using DesktopSearch.Core.Extractors.Tika;

namespace DesktopSearch.Core.Tests.ElasticSearch
{
    [TestFixture]
    public class ManagementServiceTests
    {
        private const string testDataPath = @"D:\Projects\GitHub\DesktopSearch\test\DesktopSearch.Core.Tests\TestData\ElasticSearch\";

        [Test, Explicit(TestDefinitions.Requires_running_ES_service_instance)]
        public async Task Setup_and_populate_Index()
        {
            var docColRepo = new Moq.Mock<IDocumentCollectionRepository>();
            //var esClient = ElasticTestClientFactory.Create();

            var luceneConfigMock = CfgMocks.GetLuceneConfigMock();
            var tikaMock = CfgMocks.GetTikaConfigMock();
            

            //var sut = new Core.ElasticSearch.ManagementService(esClient, elasticMock);

            //await sut.EnsureIndicesCreated();

            //TODO: fix
            var docFolderProcessoer = new DocumentFolderProcessor(
                                                    docColRepo.Object,
                                                    null,
                                                    luceneConfigMock,
                                                    new TikaServerExtractor(tikaMock));

            await docFolderProcessoer.ProcessAsync(testDataPath + "zen-of-results.pdf", Core.Configuration.DocumentSearch.ContentType.Artikel);

            //if (!result.IsValid)
            //{
            //    Console.WriteLine(result.DebugInformation);
            //    Assert.True(result.IsValid, "Indexing test document failed!");
            //}
        }
        
        [Test, Explicit(TestDefinitions.Requires_running_ES_service_instance)]
        public async Task Search_for_documents()
        {
            SearchService searchSvs = CreateSearchService();

            var results = await searchSvs.SearchDocumentAsync("another");

            Console.WriteLine("Returned results:");
            Console.WriteLine("=================");

            foreach (var r in results)
            {
                Console.WriteLine($"{r.Score}  -  {r.Source.Title}");
            }
        }

        [Test, Explicit]
        public async Task Index_Documents()
        {
            IElasticClient client;
            SearchService searchSvc;
            IndexingService indexingSvc = CreateIndexingService(out client, out searchSvc);

            var dd1 = new DocDescriptor()
            {
                Title = "Beginning SQL Queries",
                Author = "Clare Churcher",
                ContentType = "pdf",
                DocumentCollection = ContentType.Buch,
                Keywords = new[] { "SQL", "databases" },
                Path = @"Z:\Buecher\Programming\Database\Beginning SQL Queries - From Novice To Professional.pdf",
                Content = @"
Query Optimizer
In the previous sections, we looked at a couple ways the database system could carry out
a join: with nested loops or with a sort and merge. Which one will occur? Fortunately, we
don’t have to worry about this, as good relational database products have a query optimizer
to figure out the most efficient way.
What Does the Query Optimizer Consider?
The query optimizer will take into account a number of things, such as which indexes are
present, the number of rows in the tables, the length of the rows, and which fields are
required in the output. An optimizer will look at all the possible steps for completing the
task and assign time costs to each. It then comes up with the most efficient plan.
In the previous section, we looked at just a single join, but queries usually involve a
number of steps. Consider finding the tournaments that member 235 has entered. This
will require us to join the Entry and Member tables, to perform a select operation to find the
rows for member 235, and then to project the required columns. In what order should we
do the join and the select operations? Listing 9-5 shows two possibilities.
Listing 9-5. Two Algebra Expressions to Find the Tournaments That Member 235 Has Entered
In the first of the two expressions in Listing 9-5, we first do the complete join of Entry
and Member (the innermost set of parentheses). This involves comparing all the rows from"
            };

            await indexingSvc.IndexDocumentAsync(dd1);

            client.Refresh(ElasticTestClientFactory.Config.DocumentSearchIndexName);

            var results = await searchSvc.SearchDocumentAsync("SQL");

            var hit = results.SingleOrDefault(r => r.Source.Path == dd1.Path);

            Assert.IsNotNull(hit);
        }

        [Test, Explicit]
        public async Task Search_Documents()
        {
            IElasticClient esClient;
            SearchService searchSvc;
            IndexingService indexingSvc = CreateIndexingService(out esClient, out searchSvc);

            // because 
            var result = esClient.Search<DocDescriptor>(s => s
                                    //.Index(cfg.DocumentSearchIndexName)  // sollte eigentlich nicht notwendig sein
                                    .Query(q => q.QueryString(c => c.Query("SQL")))
                                    //.StoredFields(fs => fs
                                    //    .Field(p => p.Title)
                                    //    .Field(p => p.Author)
                                    //    .Field(p => p.Path)
                                    //    .Field(p => p.Keywords))
            );

            var hit = result.Hits.First();
            Assert.AreEqual("Clare Churcher", hit.Source.Author);
            //Assert.AreEqual("Clare Churcher", hit.Fields.ValueOf<DocDescriptor,string>(t => t.Author));

            // alternatively:
            // Assert.AreEqual("Clare Churcher", hit.Fields.Value<string>("author"));
        }

        private SearchService CreateSearchService()
        {
            IElasticClient esClient;
            SearchService searchSvc;
            CreateIndexingService(out esClient, out searchSvc);
            return searchSvc;
        }

        private IndexingService CreateIndexingService()
        {
            IElasticClient esClient;
            SearchService searchSvc;
            return CreateIndexingService(out esClient, out searchSvc);
        }

        private static IndexingService CreateIndexingService(out IElasticClient esClient, out SearchService searchSvc)
        {
            esClient = null; //ElasticTestClientFactory.Create();
            //var mgtmSvc = new Core.ElasticSearch.ManagementService(esClient, config);

            var mock = new Services.NullMockStore();
            var dcr = new DocumentCollectionRepository(mock);

            var docProc = new DocumentFolderProcessor(dcr, 
                                                      null, 
                                                      CfgMocks.GetLuceneConfigMock(), 
                                                      new TikaServerExtractor(CfgMocks.GetTikaConfigMock()));

            var indexingSvc = new IndexingService(dcr, docProc, null);

            var elasticMock = new Moq.Mock<IConfigAccess<ElasticSearchConfig>>();
            elasticMock.Setup(m => m.Get()).Returns(ElasticTestClientFactory.Config);

            searchSvc = new SearchService(null, elasticMock.Object, docProc);

            return indexingSvc;
        }
    }

    class ElasticTestClientFactory
    {
        static Lazy<ElasticSearchConfig> _config = new Lazy<ElasticSearchConfig>(() => 
        {
            var c = new ElasticSearchConfig();
            c.DocumentSearchIndexName += "_test";
            return c;
        });

        public static ElasticSearchConfig Config
        {
            get { return _config.Value; }
        }

        public static IElasticClient Create(ElasticSearchConfig config=null)
        {
            if (config == null)
            {
                config = _config.Value;
            }

            var cfg = ConnectionSettingsFactory.Create(OptionsProvider<ElasticSearchConfig>.Get(ElasticTestClientFactory.Config));
            cfg.DisableDirectStreaming()
               .OnRequestCompleted(details =>
               {
                   Console.WriteLine("### ES REQEUST ###");
                   if (details.RequestBodyInBytes != null)
                       Console.WriteLine(Encoding.UTF8.GetString(details.RequestBodyInBytes));
                   Console.WriteLine("### ES RESPONSE ###");
                   if (details.ResponseBodyInBytes != null)
                       Console.WriteLine(Encoding.UTF8.GetString(details.ResponseBodyInBytes));
               })
               .PrettyJson();

            return new ElasticClient(cfg);
        }
    }
}
