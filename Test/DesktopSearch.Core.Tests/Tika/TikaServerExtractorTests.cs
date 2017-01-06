﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DesktopSearch.Core.Tika;
using System.Diagnostics;
using DesktopSearch.Core.Tests;

namespace DesktopSearch.Core.Extractors.Tika
{
    [TestFixture, Explicit(TestDefinitions.Requires_running_Tika_service_instance)]
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
                var sw = Stopwatch.StartNew();

                var document = await target.ExtractAsync(ctx, new FileInfo(fullTestFilename));

                sw.Stop();
                Console.WriteLine("Extraction took: {}");

                Assert.NotNull(document);
                Assert.AreEqual(fullTestFilename, document.Path);

                Assert.AreEqual("Zen of results", document.Title);
                Assert.AreEqual("J. D. Meier", document.Author);
                Assert.AreEqual("getting things done;", document.Keywords);
            }
        }

        [Test, Explicit(TestDefinitions.Requires_running_Tika_service_instance)]
        public async Task Parse_Performance_Test()
        {
            const int testcycles = 1;
            bool pretty = false;

            string formatString = (pretty) ? "{0,10:#,##0}" : "{0};";
                //"{0,5} - {1,10:#,##0} {2,10:#,##0}" : "{0};{1};{2}";

            //var files = new[] 
            //{
            //    $"{TestContext.CurrentContext.WorkDirectory}\\TestData\\Tika\\zen-of-results.docx",
            //    $"{TestContext.CurrentContext.WorkDirectory}\\TestData\\Tika\\zen-of-results.pdf"
            //};
            var files = Directory.GetFiles(@"Z:\Buecher\Programming\Database", "*.pdf");

            Console.WriteLine($"running {testcycles} testcycles (values in [ms])");
            Console.WriteLine("==================================================");

            using (var target = new TikaServerExtractor())
            {
                var durations = new TimeSpan[files.Length];
                var ctx = new ParserContext();
                for (int i = 0; i < testcycles; i++)
                {
                    for (int j = 0; j < files.Length; j++)
                    {
                        if (pretty)
                            Console.Write("{0,5} ");

                        var sw = Stopwatch.StartNew();

                        var document = await target.ExtractAsync(ctx, new FileInfo(files[j]));

                        sw.Stop();
                        durations[j] = sw.Elapsed;
                        Console.Write(formatString, sw.ElapsedMilliseconds);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("Testcycle duration [s]: {0,10}", durations.Sum(v => v.TotalSeconds));
            }
        }
    }
}
