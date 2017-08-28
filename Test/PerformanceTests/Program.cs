using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace PerformanceTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(new[] {
                typeof(ITextSharpExtractionTests),
                typeof(TikaServerExtractor_ExtractAsyncT_Tests),
                typeof(FileIOPerformanceTests)
            });
            switcher.Run(args);

            Console.ReadLine();
        }
    }
}
