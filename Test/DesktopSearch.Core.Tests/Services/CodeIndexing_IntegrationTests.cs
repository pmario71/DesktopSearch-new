using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Lucene;
using DesktopSearch.Core.Services;
using DesktopSearch.Core.Tests.Utils;
using NUnit.Framework;
using SimpleInjector;
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
        public async Task Index_own_source_code()
        {
            var bs = new Bootstrapper();
            bs.RegisterServicesFinal = c =>
            {
                c.Register<Core.Lucene.IIndexProvider, Core.Lucene.InMemoryIndexProvider>(Lifestyle.Singleton);
                c.Register<Core.Configuration.ICurrentDirectoryProvider, TestDirectoryProvider >(Lifestyle.Singleton);
            };

            var container = bs.Initialize();
            var indexingSvc = container.GetService<IIndexingService>();

            var docColl = DocumentCollection.Create("Code", IndexingStrategy.Code);
            var folder = Folder.Create(@"C:\Projects\Tools\DesktopSearch\DesktopSearch.Core\");
            docColl.AddFolder(folder);

            await indexingSvc.IndexRepositoryAsync(docColl);

            var codeIndexer = container.GetService<ICodeIndexer>();

            var types = codeIndexer.GetIndexedTypes();
            foreach (var t in types)
            {
                Console.WriteLine($"{t.Name} - {t.Namespace}");
            }
        }

    }
}
