using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using NUnit.Framework;

namespace DesktopSearch.Core.Tests.IndexExtractors.itextsharp
{
    [TestFixture]
    public class TextFromPdfTests
    {

        //[TestCase("TestData\\Tika\\zen-of-results.pdf")]
        [Test]
        public void TextExtractionSimple()//(string testFile)
        {
            string fullTestFilename = @"D:\Dokumente\Bücher\Allgemein\Foto\Photoshop for Photographers CS4.pdf"; //$"{TestContext.CurrentContext.TestDirectory}\\{testFile}";

            var sw = Stopwatch.StartNew();
            var text = ExtractText(fullTestFilename);

            Console.WriteLine($"Text extraction {fullTestFilename} took: {sw.ElapsedMilliseconds} [ms]");

            //Console.WriteLine(text);
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
