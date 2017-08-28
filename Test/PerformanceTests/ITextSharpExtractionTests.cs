using System;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PerformanceTests
{
    public class ITextSharpExtractionTests
    {
        [Benchmark]
        public void ExtractAsync()
        {
            var res = ExtractText(TestFiles.PDFFile);
        }

        private static string ExtractText(string fullTestFilename)
        {
            if (fullTestFilename == null) throw new ArgumentNullException(nameof(fullTestFilename));

            StringBuilder text = new StringBuilder();
            using (PdfReader reader = new PdfReader(fullTestFilename))
            {
                ITextExtractionStrategy strategy = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    var page = PdfTextExtractor.GetTextFromPage(reader, i, strategy);
                    text.Append(page);
                }
            }

            return text.ToString();
        }
    }
}