using System;
using System.Linq;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Lucene;
using DesktopSearch.Core.Services;
using Lucene.Net.Store;
using Moq;
using NUnit.Framework;

namespace DesktopSearch.Core.Tests.Lucene
{
    [TestFixture]
    public class DocumentIndexTests
    {

        [Test]
        public async Task Index_Document_Descriptor()
        {
            var indexProviderMock = new Mock<IIndexProvider>();

            indexProviderMock.Setup(m => m.GetIndexDirectory(IndexType.Document))
                .Returns(new RAMDirectory());

            var codeSearchMock = new Mock<ICodeSearch>();

            var sut = new DocumentIndexer(indexProviderMock.Object);
            var sut2 = new LuceneSearchService(codeSearchMock.Object, sut);

            // index
            var descriptor = new DocDescriptor
            {
                Title        = "The good, the bad and the ugly",
                Author       = "Mario Plendl",
                Content      =
                    "Harold and nor her and lurked did but the shun ofttimes dear lines parasites power wassailers sight will concubines none",
                LastModified = DateTime.Now,
                Path         = System.IO.Path.GetTempFileName(),
                ContentType  = "mime/text",
                Keywords = new []{"Novel"}
            };

            await sut.IndexAsync(new[] { descriptor });

            // search
            var results = await sut2.SearchDocumentAsync("concubines");
            Assert.AreEqual(1, results.Count(), "Content field not searched!");

            results = await sut2.SearchDocumentAsync("ugly");
            Assert.AreEqual(1, results.Count(), "Title field not searched!");

            results = await sut2.SearchDocumentAsync("novel");
            Assert.AreEqual(1, results.Count(), "Keywords field not searched!");

            results = await sut2.SearchDocumentAsync("concubine");
            Assert.AreEqual(1, results.Count(), "stemming not working!");

            //Console.WriteLine("Results");
            //Console.WriteLine("-------");
            //foreach (var result in results)
            //{
            //    Console.WriteLine($"{result.Title}");
            //}

            //// Assert
            //Assert.AreEqual(1, results.Count());
        }
    }
}