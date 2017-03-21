using DesktopSearch.Core.Extractors.Tika;
using NUnit.Framework;

namespace DesktopSearch.Core.Tests.Tika
{

    [TestFixture]
    public class KeywordParserTests
    {
        [TestCase("Test", ExpectedResult = new string[] { "Test" })]
        [TestCase(" Test ", ExpectedResult = new string[] { "Test" })]
        [TestCase(" Test;SQL ", ExpectedResult = new string[] { "Test", "SQL" })]
        [TestCase(" Test,SQL ", ExpectedResult = new string[] { "Test", "SQL" })]
        public string[] TokenizingTests(string keywordStream)
		{
            string[] result = KeywordParser.Parse(keywordStream);
            return result;
		}
    }
}