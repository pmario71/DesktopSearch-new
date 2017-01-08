﻿using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        public void Adding_same_folder_twice_to_different_collections_throws()
        {
            var sut = new DocumentCollectionRepository(new NullMockStore());

            var col1 = DocumentCollection.Create("uniquename", Path.GetTempPath());
            var col2 = DocumentCollection.Create("uniquename2", Path.GetTempPath());

            sut.AddDocumentCollection(col1);

            Assert.Throws<FolderRootPathException>(() => sut.AddDocumentCollection(col2));
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