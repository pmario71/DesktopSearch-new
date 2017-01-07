using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;
using Moq;
using NUnit.Framework;
using System.IO;

namespace DesktopSearch.Core.Tests.Services
{

    [TestFixture]
    public class DocumentCollectionRepository_Persistence_Tests
    {
        [Test]
        public void Load_called_to_initialize_cache()
        {
            var mockStore = new Mock<IDocumentCollectionPersistence>();

            var sut = new DocumentCollectionRepository(mockStore.Object);

            mockStore.Verify(m => m.LoadAsync(), Times.Once);
        }

        [Test]
        public void StoreOrUpdate_called_when_item_added()
        {
            var mockStore = new Mock<IDocumentCollectionPersistence>();

            var sut = new DocumentCollectionRepository(mockStore.Object);

            const string name = "uniquename";

            var collection = DocumentCollection.Create(name, Path.GetTempPath());
            sut.AddDocumentCollection(collection);

            IDocumentCollection returnedType;
            sut.TryGetDocumentCollectionByName(name, out returnedType);

            mockStore.Verify(m => m.StoreOrUpdateAsync(collection), Times.Once);
        }
    }
}
