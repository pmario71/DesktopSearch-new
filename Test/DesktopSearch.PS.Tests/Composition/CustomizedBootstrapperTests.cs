using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.Tests.Utils;
using DesktopSearch.PS.Composition;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.PS.Tests.Composition
{
    [TestFixture]
    public class CustomizedBootstrapperTests
    {

        [Test]
        public void Can_initialize()
        {
            var sut = new CustomizedBootstrapper();
            sut.AddTestOverrides = c => 
            {
                c.Register<ICurrentDirectoryProvider, TestDirectoryProvider>();
            };
            sut.Initialize();
        }
    }
}
