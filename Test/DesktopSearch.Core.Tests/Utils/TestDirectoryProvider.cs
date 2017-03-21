using DesktopSearch.Core.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Tests.Utils
{
    public class TestDirectoryProvider : ICurrentDirectoryProvider
    {
        public string GetCurrentDirectory()
        {
            return TestContext.CurrentContext.WorkDirectory;
        }
    }
}
