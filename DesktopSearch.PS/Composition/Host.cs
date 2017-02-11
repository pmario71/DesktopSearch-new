using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.ComponentModel.Composition;
using Microsoft.Extensions.Logging;
using DesktopSearch.Core.ElasticSearch;
using DesktopSearch.Core;
using DesktopSearch.Core.Configuration;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace DesktopSearch.PS.Composition
{
    internal class Host : IHost
    {
        private IContainer _container;

        public Host()
        {
            var bootstrapper = new CustomizedBootstrapper();
            bootstrapper.AddTestOverrides = c =>
            {
                c.Register<Services.IKeywordSuggestions, Services.KeywordSuggestionService>();
            };

            _container = bootstrapper.Initialize();
        }

        public IContainer Container => _container;

        public void Dispose()
        {
            if (_container != null)
            {
                ((IDisposable)_container).Dispose();
            }
        }
    }

    
    [Export(typeof(IContainer))]
    internal class ContainerAccess : IContainer
    {
        private CompositionContainer _container;

        public ContainerAccess(CompositionContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            var instance = _container.GetExports(serviceType, null, null).FirstOrDefault();

            if (instance == null)
            {
                throw new Exception($"Service not available: {serviceType.Name}");
            }
            return instance.Value;
        }

        public TInstance GetService<TInstance>()
            where TInstance : class
        {
            return _container.GetExportedValue<TInstance>();
        }

        public IFace GetService<IFace, TInstance>()
            where IFace : class
        {
            return _container.GetExportedValue<IFace>();
        }

        public void Inject(object instance)
        {
            _container.SatisfyImportsOnce(instance);
        }
    }

    [Export(typeof(ILogger<>))]
    [Export(typeof(ILogger))]
    public class PowerShellLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Warning;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Console.WriteLine(formatter(state, exception));
        }
    }
}
