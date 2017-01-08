using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;
using NUnit.Framework;
using System.IO;

namespace DesktopSearch.Core.Tests.Services
{

    [TestFixture]
    public class DocumentCollectionRepository_Tests
    {
        [Test]
        public void Can_get_back_added_DocumentCollection_by_name_case_insensitive()
        {
            var sut = new DocumentCollectionRepository(new NullMockStore());

            const string name = "uniquename";

            var collection = DocumentCollection.Create(name);
            sut.AddDocumentCollection(collection);

            IDocumentCollection returnedType;
            sut.TryGetDocumentCollectionByName(name.ToUpper(), out returnedType);

            Assert.IsNotNull(returnedType);
            Assert.AreEqual(collection, returnedType);
        }

        [Test]
        public void Can_get_back_added_DocumentCollection_by_folder_path_case_insensitive()
        {
            var sut = new DocumentCollectionRepository(new NullMockStore());

            const string name = "uniquename";

            var collection = DocumentCollection.Create(name);
            sut.AddDocumentCollection(collection);
            sut.AddFolderToDocumentCollection(collection, Path.GetTempPath());

            string path = $"{Path.GetTempPath()}\\Folder\\filename.pdf".ToUpper();

            IDocumentCollection dc;
            Assert.True( sut.TryGetDocumentCollectionForPath(new FileInfo(path), out dc));
                        
            Assert.AreEqual(collection, dc);
        }

        [Test]
        public void TryGetDocumentCollectionForPath_returns_false_if_DocumentCollection_was_not_found()
        {
            var sut = new DocumentCollectionRepository(new NullMockStore());

            const string name = "uniquename";

            var collection = DocumentCollection.Create(name);
            sut.AddDocumentCollection(collection);

            string path = $"c:\\unknown_path\\unknown_file.pdf".ToUpper();

            IDocumentCollection dc;
            Assert.False(sut.TryGetDocumentCollectionForPath(new FileInfo(path), out dc));
            Assert.Null(dc);
        }
    }
}
