using DesktopSearch.Core.Services;
using DesktopSearch.Core.Tests.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Tests.Services
{
    [TestFixture]
    public class CodeIndexing_IntegrationTests
    {

        [Test]
        public void Index_own_source_code()
        {
            var bs = new Bootstrapper();
            bs.RegisterServicesFinal = c =>
            {
                c.Register<Core.Lucene.IIndexProvider, Core.Lucene.InMemoryIndexProvider>();
                c.Register<Core.Configuration.ICurrentDirectoryProvider, TestDirectoryProvider >();
            };

            var container = bs.Initialize();
            var indexingSvc = container.GetService<IIndexingService>();
        }

    }
}
