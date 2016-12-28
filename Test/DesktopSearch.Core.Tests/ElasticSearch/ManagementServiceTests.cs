using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Processors;
using DesktopSearch.Core.Services;
using Nest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DesktopSearch.Core.Tests.ElasticSearch
{
    [TestFixture]
    public class ManagementServiceTests
    {
        private const string testDataPath = @"D:\Projects\GitHub\DesktopSearch\test\DesktopSearch.Core.Tests\TestData\ElasticSearch\";

        [Test, Explicit("Requires elasticsearch running")]
        public async Task Setup_and_populate_Index()
        {
            var cfg = new ElasticSearchConfig();
            cfg.DocumentSearchIndexName += "_test";

            var esClient = ElasticClientFactory.Create(cfg);

            var sut = new Core.ElasticSearch.ManagementService(esClient, cfg);

            await sut.EnsureIndicesCreated();

            var docFolderProcessoer = new DocumentFolderProcessor(esClient, cfg);

            await docFolderProcessoer.Process(testDataPath + "zen-of-results.pdf", Core.Configuration.DocumentSearch.ContentType.Artikel);

            //if (!result.IsValid)
            //{
            //    Console.WriteLine(result.DebugInformation);
            //    Assert.True(result.IsValid, "Indexing test document failed!");
            //}
        }

        [Test, Explicit]
        public async Task Search_for_documents()
        {
            var cfg = new ElasticSearchConfig();
            cfg.DocumentSearchIndexName += "_test";

            var esClient = ElasticClientFactory.Create(cfg);
            var mgtmSvc = new Core.ElasticSearch.ManagementService(esClient, cfg);
            var docProc = new DocumentFolderProcessor(esClient, cfg);

            var searchSvs = new SearchService(esClient, cfg, mgtmSvc, docProc);

            var results = await searchSvs.SearchDocumentAsync("another");

            Console.WriteLine("Returned results:");
            Console.WriteLine("=================");

            foreach (var r in results)
            {
                Console.WriteLine($"{r.Score}  -  {r.Source.Title}");
            }
        }
    }


    class ElasticClientFactory
    {
        public static IElasticClient Create(ElasticSearchConfig config)
        {
            var settings = new ConnectionSettings(new Uri(config.Uri));
            settings
                .MapDefaultTypeIndices(m => m
                    .Add(typeof(DocDescriptor), config.DocumentSearchIndexName))
                .DisableDirectStreaming()
                    .OnRequestCompleted(details =>
                    {
                        Console.WriteLine("### ES REQEUST ###");
                        if (details.RequestBodyInBytes != null) Console.WriteLine(Encoding.UTF8.GetString(details.RequestBodyInBytes));
                        Console.WriteLine("### ES RESPONSE ###");
                        if (details.ResponseBodyInBytes != null) Console.WriteLine(Encoding.UTF8.GetString(details.ResponseBodyInBytes));
                    })
                    .PrettyJson();

            return new ElasticClient(settings);
        }
    }
}
