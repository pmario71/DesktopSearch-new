using DesktopSearch.Core.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Tests.Utils
{
    [TestFixture]
    public class CollectionExtensionsTests
    {

        [Test]
        public void DesiredResult()
        {
            var coll = new[] { "aaa", "bbb", "ccc" };
            var coll2 = new[] { "ccc" };

            var result = CollectionExtensions.FilterBySequence(coll, coll2);

            Assert.AreEqual(2, result.Count());
        }
    }
}
