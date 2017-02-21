using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DesktopSearch.Core.Tests.Utils
{
    [TestFixture]
    public class EnumExtTests
    {
        [Test]
        public void Name_of_enum_is_stringified()
        {
            string result = MyEnum.Test1.ToString();

            Assert.AreEqual("Test1", result);
        }
    }

    enum MyEnum
    {
        Test1,
        Test2
    }
}
