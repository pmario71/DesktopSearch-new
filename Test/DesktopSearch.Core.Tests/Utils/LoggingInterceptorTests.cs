using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace DesktopSearch.Core.Tests.Utils
{
    [TestFixture]
    public class LoggingInterceptorTests
    {
        [TestCase(LogLevel.Trace, ExpectedResult = true)]
        [TestCase(LogLevel.Information, ExpectedResult = true)]
        [TestCase(LogLevel.Error, ExpectedResult = false)]
        public bool IsInfoOrTraceTests(LogLevel level)
        {
            return level <= LogLevel.Information;
        }
    }
}
