using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Processors;
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

        [Test, Ignore("Requires elasticsearch running")]
        public async Task SetupIndex()
        {
            var esClient = ElasticClientFactory.Create();

            var sut = new Core.ElasticSearch.ManagementService(esClient, true);

            await sut.EnsureIndicesCreated();

            var result = await IndexingHelper.IndexDocumentAsync(esClient, testDataPath + "zen-of-results.pdf");

            if (!result.IsValid)
            {
                Console.WriteLine(result.DebugInformation);
                Assert.True(result.IsValid, "Indexing test document failed!");
            }
        }

    }


    class ElasticClientFactory
    {
        public static IElasticClient Create()
        {
            var settings = new ConnectionSettings(new Uri(Configuration.ElasticSearchUri));
            settings
                .MapDefaultTypeIndices(m => m
                    .Add(typeof(DocDescriptor), Core.Configuration.DocumentSearch.IndexName + "_test"))
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

    public class Configuration
    {
        public const string ElasticSearchUri = "http://localhost:9200";
    }
}
