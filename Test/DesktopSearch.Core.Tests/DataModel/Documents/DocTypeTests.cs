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
    public class DocTypeTests
    {

        [TestCase("", "c:\\temp")]
        [TestCase("rewq|fdas", "c:\\temp")]
        [TestCase("Buecher", "\\temp")]
        public void CreateTests(string name, string folder)
        {
            Assert.Throws<ArgumentException> (() => DocType.Create(name, folder));
        }

    }
}
