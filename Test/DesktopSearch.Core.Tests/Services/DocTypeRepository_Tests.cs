using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;
using NUnit.Framework;
using System.IO;

namespace DesktopSearch.Core.Tests.Services
{

    [TestFixture]
    public class DocTypeRepository_Tests
    {
        [Test]
        public void Can_get_back_added_DocType_by_name_case_insensitive()
        {
            var sut = new DocTypeRepository(new NullMockStore());

            const string name = "uniquename";

            var docType = DocType.Create(name, Path.GetTempPath());
            sut.AddDocType(docType);

            IDocType returnedType;
            sut.TryGetDocTypeByName(name.ToUpper(), out returnedType);

            Assert.IsNotNull(returnedType);
            Assert.AreEqual(docType, returnedType);
        }

        [Test]
        public void Can_get_back_added_DocType_by_folder_path_case_insensitive()
        {
            var sut = new DocTypeRepository(new NullMockStore());

            const string name = "uniquename";

            var docType = DocType.Create(name, Path.GetTempPath());
            sut.AddDocType(docType);

            string path = $"{Path.GetTempPath()}\\Folder\\filename.pdf".ToUpper();

            IDocType dc;
            Assert.True( sut.TryGetDocTypeForPath(new FileInfo(path), out dc));
                        
            Assert.AreEqual(docType, dc);
        }

        [Test]
        public void TryGetDocTypeForPath_returns_false_if_DocType_was_not_found()
        {
            var sut = new DocTypeRepository(new NullMockStore());

            const string name = "uniquename";

            var docType = DocType.Create(name, Path.GetTempPath());
            sut.AddDocType(docType);

            string path = $"c:\\unknown_path\\unknown_file.pdf".ToUpper();

            IDocType dc;
            Assert.False(sut.TryGetDocTypeForPath(new FileInfo(path), out dc));
            Assert.Null(dc);
        }
    }
}
