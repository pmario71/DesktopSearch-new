using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;
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
    public class DocumentCollectionRepository_AddDocumentCollection_Tests
    {

        [Test]
        public void Can_add_DocumentCollection_to_repo()
        {

            var sut = new DocumentCollectionRepository(new NullMockStore());

            var collection = DocumentCollection.Create("uniquename", Path.GetTempPath());
            sut.AddDocumentCollection(collection);
        }

        [Test]
        public void Adding_same_folder_twice_throws()
        {
            var sut = new DocumentCollectionRepository(new NullMockStore());

            var col1 = DocumentCollection.Create("uniquename", Path.GetTempPath());
            var col2 = DocumentCollection.Create("uniquename2", Path.GetTempPath());

            sut.AddDocumentCollection(col1);

            Assert.Throws<FolderRootPathException>(() => sut.AddDocumentCollection(col2));
        }
    }

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
            var mock = new NullMockStore();
            var col = DocumentCollection.Create("uniquename", BuildTempPathAndCreateFolder("F1\\"));
            mock.DocumentCollections = new IDocumentCollection[] { col };

            var sut = new DocumentCollectionRepository(mock);
                        
            sut.AddFolderToDocumentCollection(col, BuildTempPathAndCreateFolder("someotherPath\\"));

            Assert.AreEqual(2, col.Folders.Count);
        }

        [Test]
        public void Cannot_add_same_folder_twice()
        {
            var mock = new NullMockStore();
            string thePath = BuildTempPathAndCreateFolder("samePath\\");

            var col = DocumentCollection.Create("uniquename", thePath);
            mock.DocumentCollections = new IDocumentCollection[] { col };

            var sut = new DocumentCollectionRepository(mock);

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

    class Cleaner : IDisposable
    {
        List<FileSystemInfo> _paths = new List<FileSystemInfo>();

        public void Add(string path)
        {
            var di = new DirectoryInfo(path);
            if (di.Exists)
            {
                _paths.Add(di);
                return;
            }
            FileInfo fi = new FileInfo(path);

            if (!fi.Exists)
            {
                throw new ArgumentException("Directory or File need to exist!", nameof(path));
            }

            _paths.Add(fi);
        }

        public void Add(DirectoryInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            if (!info.Exists)
                throw new ArgumentException("Directory must exist!", nameof(info));

            _paths.Add(info);
        }

        public void Dispose()
        {
            foreach (var item in _paths.ToArray())
            {
                item.Delete();
            }
            _paths.Clear();
        }
    }
}
