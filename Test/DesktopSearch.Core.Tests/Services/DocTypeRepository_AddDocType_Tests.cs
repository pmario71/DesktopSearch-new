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
    public class DocTypeRepository_AddDocType_Tests
    {

        [Test]
        public void Can_add_DocType_to_repo()
        {

            var sut = new DocTypeRepository(new NullMockStore());

            var docType = DocType.Create("uniquename", Path.GetTempPath());
            sut.AddDocType(docType);
        }

        [Test]
        public void Adding_same_folder_twice_throws()
        {
            var sut = new DocTypeRepository(new NullMockStore());

            var docType = DocType.Create("uniquename", Path.GetTempPath());
            var docType2 = DocType.Create("uniquename2", Path.GetTempPath());

            sut.AddDocType(docType);

            Assert.Throws<FolderRootPathException>(() => sut.AddDocType(docType2));
        }
    }
}
