using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.FileSystem;
using DesktopSearch.Core.Lucene;
using DesktopSearch.Core.Processors;
using DesktopSearch.Core.Tests.DependencyInjection;
using DesktopSearch.Core.Tests.Utils;
using DesktopSearch.Core.Utils;
using NUnit.Framework;

namespace DesktopSearch.Core.Tests.Lucene
{
    [TestFixture]
    public class CodeFolderProcessorTests
    {
        [Explicit]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(20)]
        [TestCase(40)]
        public void FileIO_Test(int degreeOfParallelism)
        {
            const string folder = "c:\\Projects";
            var extensionFilter = new IncludeFileByExtensionFilter(".cs", ".xaml");

            var filesToProcess = Directory.GetFiles(folder, "*", SearchOption.AllDirectories)
                                          .Where(f => extensionFilter.FilterByExtension(f)).ToList();

            var results = new Result[filesToProcess.Count];
            int idx = -1;

            var sw = Stopwatch.StartNew();

            ParallelOptions ps = new ParallelOptions {MaxDegreeOfParallelism = degreeOfParallelism };
            Parallel.ForEach(filesToProcess, ps, file =>
            //foreach (var file in filesToProcess)
            {
                var innerSw = Stopwatch.StartNew();

                //string text = null;
                //using (var strm = new FileStream(file,FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.Asynchronous))
                //{
                //    var rd = new StreamReader(strm);
                //    text = await rd.ReadToEndAsync();
                //}

                var text = File.ReadAllText(file);

                var i = Interlocked.Increment(ref idx);
                results[i] = new Result(){ TimeToRead = sw.Elapsed, Size = text.Length };
            });

            sw.Stop();

            Console.WriteLine($"Total time to read {idx} files: {sw.Elapsed.TotalSeconds} [s]");
            foreach (var result in results.OrderBy(s => s.Size))
            {
                Console.WriteLine($"{result.Size};{result.TimeToRead}");
            }
        }

        [Test]
        public async Task Parsing_files_shall_result_in_no_errors()
        {
            //TODO: add files as Content
            var fname = @"c:\Projects\Scratch\WPFToolkit\WpfToolkit\DataVisualization";

            var sut = new CodeFolderProcessor(new CodeIndexer(new InMemoryIndexProvider()), new Performance());

            var logger = new LoggingInterceptor<CodeFolderProcessor>(ignoreInfoLevel:true);
            sut.OverrideLogger = logger;

            IFolder folder = new FolderMock() { Path = fname };
            await sut.ProcessAsync(folder);

            Assert.IsFalse(logger.LoggedEvents.Any());
        }

        struct Result
        {
            public long Size;
            public TimeSpan TimeToRead;
        }

        [Test,Explicit]
        public async Task PerformanceTests()
        {
            var sut = new CodeFolderProcessor(new CodeIndexer(new InMemoryIndexProvider()), new Performance());

            var logger = new LoggingInterceptor<CodeFolderProcessor>();
            sut.OverrideLogger = logger;

            IFolder folder = new FolderMock(){ Path = "c:\\Projects"};
            await sut.ProcessAsync(folder);

            Assert.IsFalse(logger.LoggedEvents.Any());
        }

        class FolderMock : IFolder
        {
            public string Path { get; set; }
            public IDocumentCollection DocumentCollection { get; }
        }
    }
}
