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
        private IConfigurationRoot _config;

        public IContainer Initialize()
        {
            var container = new Container();
            container.Options.AutoWirePropertiesImplicitly();
            var convention = container.RegisterConstructorSelectorConvention();

            RegisterBaseServices(container);
            RegisterServices(new ContainerWrapper(container));
            container.Verify();

            return new ContainerWrapper(container);
        }

        protected virtual void RegisterServices(IContainerInitializer container)
        {
        }

        protected virtual void Configure(ConfigurationBuilder builder)
        {
        }

        private void Configure(Container container)
        {
            // .net core configuration
            ConfigurationBuilder _builder = new ConfigurationBuilder();
            IEnumerable<KeyValuePair<string, string>> initial = new[]
            {
                new KeyValuePair<string,string>("Test","Value")
            };

            _builder.AddInMemoryCollection(initial);
            _config = _builder.Build();

            container.AddOptions();

            // Binds config.json to the options and setups the change tracking.
            container.Configure<ElasticSearchConfig>(_config.GetSection("ElastcSearch"));
            container.Configure<TikaConfig>(_config.GetSection("Tika"));

            //var elasticSearchConfig = new Core.Configuration.ElasticSearchConfig();
            //_config.Bind(elasticSearchConfig);
        }

        private void RegisterBaseServices(Container container)
        {
            container.Register<DesktopSearch.Core.ElasticSearch.ClientFactory>(Lifestyle.Singleton);
            container.Register<Nest.IElasticClient>(() => container.GetInstance<ElasticSearch.ClientFactory>().SearchClient,
                Lifestyle.Singleton);
            //Func<Nest.IElasticClient> f = () => container.GetInstance<ElasticSearch.ClientFactory>().SearchClient;
            //container.Register<Nest.IElasticClient, Nest.ElasticClient>(Lifestyle.Singleton);

            container.Register<Core.ElasticSearch.ManagementService>(Lifestyle.Singleton);

            container.Register<Core.Services.IDocumentCollectionPersistence, Core.Services.DocumentCollectionElasticStore>(Lifestyle.Singleton);
            container.Register<Core.Services.IDocumentCollectionRepository, Core.Services.DocumentCollectionRepository>(Lifestyle.Singleton);

            container.Register<Core.Services.ISearchService, Core.Services.SearchService>(Lifestyle.Singleton);

            container.Register<Core.Services.IIndexingService, Core.Services.IndexingService>(Lifestyle.Singleton);

            container.Register<Core.Processors.CodeFolderProcessor>(Lifestyle.Singleton);
            container.Register<Lucene.ICodeIndexer, Lucene.CodeIndex>(Lifestyle.Singleton);

            container.Register<Core.Processors.DocumentFolderProcessor>(Lifestyle.Singleton);

            container.Register<Core.Configuration.IStreamFactory, Core.Configuration.FileStreamFactory>(Lifestyle.Singleton);

            container.Register<Core.Configuration.IConfigAccess, Core.Configuration.ConfigAccess>(Lifestyle.Singleton);

            //var ca = new ContainerAccess(container);
            //container.ComposeExportedValue<IContainer>(ca);
        }
    }
}
