using DesktopSearch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using DesktopSearch.Core.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DesktopSearch.PS.Composition
{
    internal class CustomizedBootstrapper : Bootstrapper
    {
        public CustomizedBootstrapper()
        {
            AddTestOverrides = _ => { };
        }

        protected override void Configure(ConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");
        }

        protected override void RegisterServices(IContainerInitializer initializer)
        {
            initializer.Register<Services.IKeywordSuggestions, Services.KeywordSuggestionService>();
            initializer.Register(typeof(ILogger<>), typeof(PowerShellLogger<>));

            AddTestOverrides(initializer);
        }

        public Action<IContainerInitializer> AddTestOverrides { get; set; }
    }
}
