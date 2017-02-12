using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Tests.Services
{
    [TestFixture]
    public class DocumentCollectionConfigStoreTests
    {
        [Test]
        public async Task Store_first_DocumentCollection_Tests()
        {
            var cfg = new LuceneConfig();

            var mock = new Moq.Mock<IConfigAccess<LuceneConfig>>();
            mock.Setup(m => m.Get()).Returns(cfg);

            var sut = new DocumentCollectionConfigStore(mock.Object);

            var col = DocumentCollection.Create("Test", IndexingStrategy.Code);
            
            await sut.StoreOrUpdateAsync(col);

            var storedCollections = await sut.LoadAsync();
            
            CollectionAssert.Contains(storedCollections, col);
            Assert.AreEqual(1, storedCollections.Count());
        }

        [Test]
        public async Task Update_existing_collection_Tests()
        {
            var cfg = new LuceneConfig();
            cfg.DocumentCollections = new[] 
            {
                (DocumentCollection)DocumentCollection.Create("Test", IndexingStrategy.Code)
            };

            var mock = new Moq.Mock<IConfigAccess<LuceneConfig>>();
            mock.Setup(m => m.Get()).Returns(cfg);

            var sut = new DocumentCollectionConfigStore(mock.Object);

            var col = DocumentCollection.Create("Test", IndexingStrategy.Documents);

            await sut.StoreOrUpdateAsync(col);

            var storedCollections = await sut.LoadAsync();

            Assert.AreEqual(1, storedCollections.Count());
            Assert.AreEqual(IndexingStrategy.Documents, cfg.DocumentCollections[0].IndexingStrategy);
        }
    }
}
