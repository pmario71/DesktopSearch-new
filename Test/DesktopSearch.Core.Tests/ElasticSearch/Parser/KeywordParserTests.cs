using DesktopSearch.Core.Extractors.Tika;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Tests.ElasticSearch.Parser
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