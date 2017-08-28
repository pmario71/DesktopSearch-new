using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DesktopSearch.Core.Tika;
using System.Diagnostics;
using DesktopSearch.Core.Tests;
using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.Services;
using DesktopSearch.Core.Tests.Utils;

namespace DesktopSearch.Core.Extractors.Tika
{
    [TestFixture, Explicit(TestDefinitions.Requires_running_Tika_service_instance)]
    public class TikaServerExtractorTests
    {
        private static TikaServer _server;
        private static DockerServiceManager _dockerServiceManager;

        [OneTimeSetUp]
        public static void SetupFixture()
        {
            _server = new TikaServer();
            _server.Start();
            //_dockerService = new DockerService();
            //_dockerService.EnsureTikaStarted().Wait(TimeSpan.FromSeconds(5));
        }

        [SetUp]
        public void Setup()
        {
            //_server = new TikaServer();
            //_server.Start();
        }

        [OneTimeTearDown]
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
            string fullTestFilename = $"{TestContext.CurrentContext.TestDirectory}\\{testFile}";

            //var context = new ParserContext();
            using (var target = new TikaServerExtractor(CfgMocks.GetTikaConfigMock()))
            {
                var ctx = new ParserContext();
                var sw = Stopwatch.StartNew();

                var document = await target.ExtractAsync(ctx, new FileInfo(fullTestFilename));

                sw.Stop();
                Console.WriteLine("Extraction took: {}");

                Assert.NotNull(document);
                Assert.AreEqual(fullTestFilename, document.Path);

                Assert.AreEqual("Zen of results", document.Title);
                Assert.AreEqual("J. D. Meier", document.Author);
                Assert.AreEqual("getting things done", document.Keywords[0]);
            }
        }

        

        [Test]
        public async Task DetectLanguageTests()
        {
            
            using (var target = new TikaServerExtractor(CfgMocks.GetTikaConfigMock()))
            {
                
                var sw = Stopwatch.StartNew();

                var txt = "\"Dieselgate hätte vermieden werden können, wenn die EU-Kommission und die Mitgliedstaaten einfach nur EU-Recht " +
                          "eingehalten hätten.\" So fasst der liberale niederländische EU-Parlamentarier Gerben-Jan Gerbrandy das Ergebnis " +
                          "eines Untersuchungsausschusses zur Abgasaffäre zusammen. Die Mitglieder hatten seit April 2016 untersucht, wie es " +
                          "zu dem Skandal um manipulierte Dieselmotoren kommen konnte.";

                var language = await target.DetectLanguageAsync(txt);

                sw.Stop();
                Console.WriteLine($"Extraction took: {sw.ElapsedMilliseconds} [ms]");

                Assert.AreEqual("de", language);
            }
        }
    }
}
