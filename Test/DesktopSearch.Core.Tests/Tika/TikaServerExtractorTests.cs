using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DesktopSearch.Core.Tika;

namespace DesktopSearch.Core.Extractors.Tika
{
    [TestFixture]
    public class TikaServerExtractorTests
    {
        private TikaServer _server;

        [SetUp]
        public void Setup()
        {
            //_server = new TikaServer();
            //_server.Start();
        }

        [TearDown]
        public void Dispose()
        {
            if (_server != null)
            {
                _server.Stop();
                _server = null;
            }
        }

        [TestCase("TestData\\Tika\\zen-of-results.docx")]
        [TestCase("TestData\\Tika\\zen-of-results.pdf")]
        //[TestCase(@"D:\Dokumente\Bücher\Agile\xx\Wrox.Code.Leader.Using.People.Tools.and.Processes.to.Build.Successful.Software.May.2008.pdf"), Explicit]
        public async Task ParseFileTests(string testFile)
        {
            string fullTestFilename = $"{TestContext.CurrentContext.WorkDirectory}\\{testFile}";

            //var context = new ParserContext();
            using (var target = new TikaServerExtractor())
            {
                var ctx = new ParserContext();
                var document = await target.ExtractAsync(ctx, new FileInfo(fullTestFilename));

                Assert.NotNull(document);
                Assert.AreEqual(fullTestFilename, document.Path);

                Assert.AreEqual("Zen of results", document.Title);
                Assert.AreEqual("J. D. Meier", document.Author);
                Assert.AreEqual("getting things done;", document.Keywords);
            }
        }
    }
}
