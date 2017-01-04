using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Documents;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.PS.Tests.Configuration
{
    [TestFixture]
    public class ConfigAccessTests
    {

        [Test]
        public void NullValueHandling_for_ElasticSearchURI_Test()
        {
            throw new NotImplementedException("Refactor!");

            //Settings settings = new Settings
            //{
            //    FoldersToIndex = new FoldersToIndex
            //    {
            //        Folders = new[]
            //        {
            //            Folder.Create(Path.GetTempPath(), indexingType:"Code")
            //        }.ToList()
            //    }
            //};

            //var strm = new MemoryStreamEx();
            //var sut = new ConfigAccess(new TestFactory(strm));

            //sut.SaveChanges(settings);

            //strm.Position = 0;

            //var sr = new StreamReader(strm);
            //var s = sr.ReadToEnd();

            //// check that stream does not contain serialized 
            //CollectionAssert.DoesNotContain(s, "localhost");

            //strm.Position = 0;

            //var result = sut.Get();

            //Assert.AreEqual(settings.FoldersToIndex.Folders[0].Path, result.FoldersToIndex.Folders[0].Path);
        }

        [Test]
        public void SerializeDeserializeTest()
        {
            throw new NotImplementedException("Refactor!");
            //Settings settings = new Settings
            //{
            //    ElasticSearchUri = new Uri("http://test.com:1234"),
            //    FoldersToIndex = new FoldersToIndex
            //    {
            //        Folders = new[]
            //        {
            //            Folder.Create(Path.GetTempPath(), indexingType:"Code")
            //        }.ToList()
            //    }
            //};

            //var strm = new MemoryStreamEx();
            //var sut = new ConfigAccess(new TestFactory(strm));

            //sut.SaveChanges(settings);

            //strm.Position = 0;

            //var sr = new StreamReader(strm);
            //var s = sr.ReadToEnd();

            //strm.Position = 0;

            //var result = sut.Get();

            //Assert.AreEqual(settings.FoldersToIndex.Folders[0].Path, result.FoldersToIndex.Folders[0].Path);
        }

    }

    internal class TestFactory : IStreamFactory
    {
        private MemoryStream strm;

        public TestFactory(MemoryStream strm)
        {
            this.strm = strm;
        }

        public Stream GetReadableStream()
        {
            return strm;
        }

        public Stream GetWritableStream()
        {
            return strm;
        }
    }
}
