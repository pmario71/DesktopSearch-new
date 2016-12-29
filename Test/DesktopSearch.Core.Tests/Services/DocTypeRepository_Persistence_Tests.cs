using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;
using Moq;
using NUnit.Framework;
using System.IO;

namespace DesktopSearch.Core.Tests.Services
{

    [TestFixture]
    public class DocTypeRepository_Persistence_Tests
    {
        [Test]
        public void Load_called_to_initialize_cache()
        {
            var mockStore = new Mock<IDocTypePersistence>();

            var sut = new DocTypeRepository(mockStore.Object);

            mockStore.Verify(m => m.LoadAsync(), Times.Once);
        }

        [Test]
        public void StoreOrUpdate_called_when_item_added()
        {
            var mockStore = new Mock<IDocTypePersistence>();

            var sut = new DocTypeRepository(mockStore.Object);

            const string name = "uniquename";

            var docType = DocType.Create(name, Path.GetTempPath());
            sut.AddDocType(docType);

            var returnedType = sut.GetDocTypeByName(name);

            mockStore.Verify(m => m.StoreOrUpdateAsync(docType), Times.Once);
        }
    }
}
