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
using DesktopSearch.Core.Processors;

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

    [TestFixture]
    public class DocumentIndexing_IntegrationTests
    {

        [Test]
        public async Task Index_some_books()
        {
            var bs = new Bootstrapper();
            bs.RegisterServicesFinal = c =>
            {
                c.Register<Core.Lucene.IIndexProvider, Core.Lucene.InMemoryIndexProvider>(Lifestyle.Singleton);
                c.Register<Core.Configuration.ICurrentDirectoryProvider, TestDirectoryProvider>(Lifestyle.Singleton);
            };

            var container = bs.Initialize();

            var fp = (DocumentFolderProcessor)container.GetService<DocumentFolderProcessor>();
            fp.OverrideLogger = new LoggingInterceptor<DocumentFolderProcessor>();

            var indexingSvc = container.GetService<IIndexingService>();

            var docColl = DocumentCollection.Create("Docs", IndexingStrategy.Documents);
            var folder = Folder.Create(@"D:\Dokumente\Bücher\Agile");
            docColl.AddFolder(folder);

            // Act
            await indexingSvc.IndexRepositoryAsync(docColl);

            // Assert
            var indexer = container.GetService<IDocumentIndexer>();
            var docs = indexer.GetIndexedDocuments();

            Console.WriteLine();
            foreach (var t in docs)
            {
                Console.WriteLine($" - {t}");
            }
        }

    }
}
