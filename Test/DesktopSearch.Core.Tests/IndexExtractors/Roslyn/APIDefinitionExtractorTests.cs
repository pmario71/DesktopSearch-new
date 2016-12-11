using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DesktopSearch.Core.DataModel;
using DesktopSearch.Core.Extractors.Roslyn;
using DesktopSearch.Core.DataModel.Code;
using NUnit.Framework;

namespace CodeSearchTests.Indexing.Roslyn
{
    public class APIDefinitionExtractorTests
    {

        [TestCase("API:NO", API.No)]
        [TestCase("API :  NO", API.No)]
        [TestCase("API:YES", API.Yes)]
        [TestCase("API :  YES", API.Yes)]
        [TestCase("Some other text API test.", API.Undefined)]
        [TestCase("Some other text API: test.", API.Undefined)]
        public void ParseTests(string comment, API expectedResult)
        {
            Assert.AreEqual(expectedResult, APIDefinitionExtractor.Parse(comment));
        }

    }
}
