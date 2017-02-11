using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.ElasticSearch;
using DesktopSearch.Core.Services;
using DesktopSearch.Core.Tests.ElasticSearch;
using DesktopSearch.Core.Tests.Utils;
using Nest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Tests.Services
{
    [TestFixture]
    public class DocumentCollectionElasticStoreTests
    {
        [Test, Explicit(TestDefinitions.Requires_running_ES_service_instance)]
        public async Task Persist_DocumentCollection_and_read_them_back_in()
        {
            var client = ElasticTestClientFactory.Create();

            var mgmSvc = new ManagementService(
                client, 
                CfgMocks.GetElasticSearchConfigMock());

            await mgmSvc.EnsureIndicesCreated();

            var sut = new DocumentCollectionElasticStore(client, CfgMocks.GetElasticSearchConfigMock());

            var documentCollection = DocumentCollection.Create("Buecher", IndexingStrategy.Code);
            var folder = Folder.Create(Path.GetTempPath());
            folder.DocumentCollection = documentCollection;
            ((DocumentCollection)documentCollection).Folders.Add(folder);
            
            await sut.StoreOrUpdateAsync(documentCollection);

            client.Refresh(ElasticTestClientFactory.Config.DocumentSearchIndexName);

            var result = await sut.LoadAsync();

            Assert.AreEqual(documentCollection.Name, result.First().Name);
            Assert.AreEqual(documentCollection.IndexingStrategy, result.First().IndexingStrategy);

            Assert.AreEqual(1, result.First().Folders.Count());
            Assert.AreEqual(documentCollection.Folders.First().Path, result.First().Folders.First().Path);
        }


        [Test, Explicit(TestDefinitions.Requires_running_ES_service_instance)]
        public async Task Persist_DocumentCollection_and_read_them_back_in_lowlevel()
        {
            var client = ElasticTestClientFactory.Create();

            string idxName = ElasticTestClientFactory.Config.DocumentSearchIndexName;
            if (!client.IndexExists(idxName).Exists)
            {
                var docIndex = new CreateIndexDescriptor(idxName);

                docIndex.Mappings(mp => mp
                                            .Map<DocType2>(m => m.AutoMap())
                                            .Map<Folder2>(m => m.AutoMap()));
                var task = await client.CreateIndexAsync(idxName, i => docIndex);
            }

            var docType = new DocType2()
            {
                Name = "Buecher",
                IncludedExtensions = new[] { "pdf" },
            };
            docType.Folders.Add(new Folder2()
            {
                Path = Path.GetTempPath(),
                IndexingType = "fdjas"
            });


            await client.IndexAsync(docType, t => t.Index(idxName));

            client.Refresh(idxName);

            var documentCollections = await client.SearchAsync<DocType2>(t => t.Index(idxName).Query(q => q.MatchAll()));
            var result = documentCollections.Hits.Select(s => s.Source);

            Assert.AreEqual(docType.Name, result.First().Name);
            Assert.AreEqual(docType.Folders.First().Path, result.First().Folders.First().Path);
        }
    }

    [ElasticsearchType(Name = "doctype2", IdProperty = "Name")]
    public class DocType2
    {
        private List<Folder2> _folders;

        public DocType2()
        {
            _folders = new List<Folder2>();
        }

        public string Name { get; set; }
        public string TrueCryptContainerPath { get; set; }
        public List<Folder2> Folders { get { return _folders; } }
        public string[] ExcludedExtensions { get; set; }
        public string[] IncludedExtensions { get; set; }
    }

    [ElasticsearchType(Name = "folder2", IdProperty = "Path")]
    public class Folder2
    {
        public string Path { get; set; }
        public string IndexingType { get; set; }
        public string Machinename { get; internal set; }
    }
}
