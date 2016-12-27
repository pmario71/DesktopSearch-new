using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Configuration
{
    /// <summary>
    /// Uses .net core configuration:  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration
    /// </summary>
    public class ConfigBootstrapping
    {
        private static ConfigurationBuilder _builder = new ConfigurationBuilder();

        static ConfigBootstrapping()
        {
            IEnumerable<KeyValuePair<string, string>> initial = new[] 
            {
                new KeyValuePair<string,string>("Test","Value")
            };

            _builder.AddInMemoryCollection(initial);
        }
        
        public static IConfigurationBuilder GetDefault()
        {
            return _builder;
        }
    }
}
