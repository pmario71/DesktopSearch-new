using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Advanced;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopSearch.Core
{
    public class Bootstrapper
    {
        public IContainer Initialize()
        {
            var container = new Container();
            container.Options.AllowOverridingRegistrations = true;
            container.Options.AutoWirePropertiesImplicitly();

            var convention = container.RegisterConstructorSelectorConvention();
            container.Options.PropertySelectionBehavior = new ImportPropertySelectionBehavior();

            Configure(container);
            RegisterBaseServices(container);
            RegisterServices(new ContainerWrapper(container));
            RegisterServicesFinal?.Invoke(container);

            container.Verify();

            return new ContainerWrapper(container);
        }

        protected virtual void RegisterServices(IContainerInitializer container)
        {
        }

        protected virtual void Configure(ConfigurationBuilder builder)
        {
        }

        public Action<Container> RegisterServicesFinal { get; set; }

        private void Configure(Container container)
        {
            container.Register(typeof(IConfigAccess<TikaConfig>), typeof(ConfigAccess<TikaConfig>), Lifestyle.Singleton);
            container.Register(typeof(IConfigAccess<ElasticSearchConfig>), typeof(ConfigAccess<ElasticSearchConfig>), Lifestyle.Singleton);
            container.Register(typeof(IConfigAccess<LuceneConfig>), typeof(ConfigAccess<LuceneConfig>), Lifestyle.Singleton);
        }

        private void RegisterBaseServices(Container container)
        {
            container.Register<DesktopSearch.Core.ElasticSearch.ClientFactory>(Lifestyle.Singleton);
            container.Register<Nest.IElasticClient>(() => container.GetInstance<ElasticSearch.ClientFactory>().SearchClient,
                Lifestyle.Singleton);
            //Func<Nest.IElasticClient> f = () => container.GetInstance<ElasticSearch.ClientFactory>().SearchClient;
            //container.Register<Nest.IElasticClient, Nest.ElasticClient>(Lifestyle.Singleton);

            // removed
            //container.Register<Core.ElasticSearch.ManagementService>(Lifestyle.Singleton);

            container.Register<Extractors.Tika.ITikaServerExtractor,Extractors.Tika.TikaServerExtractor>(Lifestyle.Singleton);


            //container.Register<Core.Services.IDocumentCollectionPersistence, Core.Services.DocumentCollectionElasticStore>(Lifestyle.Singleton);
            container.Register<Core.Services.IDocumentCollectionPersistence, Core.Services.DocumentCollectionConfigStore>(Lifestyle.Singleton);
            container.Register<Core.Services.IDocumentCollectionRepository, Core.Services.DocumentCollectionRepository>(Lifestyle.Singleton);

            container.Register<Core.Services.ISearchService, Services.LuceneSearchService>(Lifestyle.Singleton);  // Core.Services.SearchService

            container.Register<Core.Services.IIndexingService, Core.Services.IndexingService>(Lifestyle.Singleton);

            container.Register<Core.Processors.CodeFolderProcessor>(Lifestyle.Singleton);
            container.Register<Core.Processors.DocumentFolderProcessor>(Lifestyle.Singleton);

            container.Register<ICurrentDirectoryProvider, DefaultDirectoryProvider>(Lifestyle.Singleton);
            container.Register<Core.Configuration.IStreamFactory, Core.Configuration.FileStreamFactory>(Lifestyle.Singleton);

            container.Register<Core.Configuration.IConfigAccess, Core.Configuration.ConfigAccess>(Lifestyle.Singleton);
            container.Register<Lucene.IIndexProvider, Lucene.IndexProvider>(Lifestyle.Singleton);

            var registration = Lifestyle.Singleton.CreateRegistration<Lucene.CodeIndex>(container);
            container.AddRegistration(typeof(Lucene.ICodeIndexer), registration);
            container.AddRegistration(typeof(Lucene.ICodeSearch), registration);

            container.Register<Services.IIndexingStatistics, Services.IndexingStatisticsService>(Lifestyle.Singleton);
            //var ca = new ContainerAccess(container);
            //container.ComposeExportedValue<IContainer>(ca);
        }
    }
}
