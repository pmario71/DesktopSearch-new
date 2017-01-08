using DesktopSearch.Core.DataModel.Documents;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Tests.DataModel.Documents
{
    [TestFixture]
    public class DocumentCollectionTests
    {

        [TestCase("")]
        [TestCase("rewq|fdas")]
        public void CreateTests(string name)
        {
            Assert.Throws<ArgumentException> (() => DocumentCollection.Create(name));
        }

    }
}
