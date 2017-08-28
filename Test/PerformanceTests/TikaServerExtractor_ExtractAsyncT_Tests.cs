using System.Diagnostics;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using DesktopSearch.Core.Extractors;
using DesktopSearch.Core.Extractors.Tika;
using DesktopSearch.Core.Tests.Utils;

namespace PerformanceTests
{
    [RPlotExporter]
    public class TikaServerExtractor_ExtractAsyncT_Tests
    {
        private TikaServerExtractor _sut;

        [GlobalSetup]
        public void Setup()
        {
            //_files = Directory.GetFiles(@"D:\Dokumente\Bücher\Database", "*.pdf").Take(3).ToArray();

            _sut = new TikaServerExtractor(CfgMocks.GetTikaConfigMock());
        }


        [GlobalCleanup]
        public void Cleanup()
        {
            _sut.Dispose();
        }

        [Benchmark]
        public void ExtractAsync()
        {
            var ctx = new ParserContext();

            //foreach (string file in _files)
            {
                var res = _sut.ExtractAsync(ctx, new FileInfo(TestFiles.PDFFile)).Result;
            }
        }
    }
}