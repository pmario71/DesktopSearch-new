using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Tests.Services
{

    [TestFixture]
    public class DocumentCollectionRepository_AddFolderToDocumentCollection_Tests
    {
        Cleaner _cleaner;

        [SetUp]
        public void Setup()
        {
            _cleaner = new Cleaner();
        }

        [TearDown]
        public void Dispose()
        {
            _cleaner.Dispose();
        }

        [Test]
        public void Can_add_folder_to_DocumentCollection()
        {
            var mock2 = new Mock<IDocumentCollectionPersistence>();
            
            var col = DocumentCollection.Create("uniquename");

            IEnumerable<IDocumentCollection> dcs = new IDocumentCollection[] { col };
            mock2.Setup(m => m.LoadAsync()).Returns(Task.FromResult(dcs));

            var sut = new DocumentCollectionRepository(mock2.Object);
            
            sut.AddFolderToDocumentCollection(col, BuildTempPathAndCreateFolder("F1\\"));
            sut.AddFolderToDocumentCollection(col, BuildTempPathAndCreateFolder("someotherPath\\"));

            Assert.AreEqual(2, col.Folders.Count);
            mock2.Verify(m => m.StoreOrUpdateAsync(It.IsAny<IDocumentCollection>())); // actual store happened
        }

        [Test]
        public void Cannot_add_same_folder_twice()
        {
            var mock = new NullMockStore();
            string thePath = BuildTempPathAndCreateFolder("samePath\\");

            var col = DocumentCollection.Create("uniquename");
            mock.DocumentCollections = new IDocumentCollection[] { col };

            var sut = new DocumentCollectionRepository(mock);
            sut.AddFolderToDocumentCollection(col, thePath);

            Assert.Throws<FolderRootPathException>(() => sut.AddFolderToDocumentCollection(col, thePath));
        }

        [Test]
        public void Cannot_add_child_folder()
        {
            var mock = new NullMockStore();
            string thePath = BuildTempPathAndCreateFolder("samePath\\child\\");

            //setup folders
            var folders = new List<IFolder>();
            var folderMoq = new Moq.Mock<IFolder>();
            folderMoq.Setup(f => f.Path).Returns(Path.GetDirectoryName(thePath));
            folders.Add(folderMoq.Object);

            var dtMoq = new Moq.Mock<IDocumentCollection>();
            dtMoq.Setup(m => m.Name).Returns("DocumentCollectionName");
            dtMoq.Setup(m => m.Folders).Returns(folders);
            folderMoq.Setup(f => f.DocumentCollection).Returns(dtMoq.Object);
            mock.DocumentCollections = new IDocumentCollection[] { dtMoq.Object };

            var sut = new DocumentCollectionRepository(mock);

            Assert.Throws<FolderRootPathException>(() =>
            {
                try
                {
                    sut.AddFolderToDocumentCollection(dtMoq.Object, thePath);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            });
        }

        [Test]
        public void Cannot_add_parent_folder()
        {
            var mock = new NullMockStore();
            string thePath = BuildTempPathAndCreateFolder("samePath\\child\\");

            //setup folders
            var folders = new List<IFolder>();
            var folderMoq = new Moq.Mock<IFolder>();
            folderMoq.Setup(f => f.Path).Returns($"{thePath}AnotherChild\\");
            folders.Add(folderMoq.Object);

            var dtMoq = new Moq.Mock<IDocumentCollection>();
            dtMoq.Setup(m => m.Name).Returns("DocumentCollectionName");
            dtMoq.Setup(m => m.Folders).Returns(folders);
            folderMoq.Setup(f => f.DocumentCollection).Returns(dtMoq.Object);
            mock.DocumentCollections = new IDocumentCollection[] { dtMoq.Object };

            var sut = new DocumentCollectionRepository(mock);

            Assert.Throws<FolderRootPathException>(() =>
            {
                try
                {
                    sut.AddFolderToDocumentCollection(dtMoq.Object, thePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            });
        }

        private string BuildTempPathAndCreateFolder(string relativePath)
        {
            string p = Path.GetTempPath() + relativePath;
            _cleaner.Add(Directory.CreateDirectory(p));

            return p;
        }
    }
}
