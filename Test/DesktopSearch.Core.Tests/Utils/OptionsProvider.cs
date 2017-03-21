using DesktopSearch.Core.Configuration;
using Microsoft.Extensions.Options;

namespace DesktopSearch.Core.Tests.Utils
{

    public class OptionsProvider<T> : IOptions<T>
        where T : class, new()
    {
        private T _config;

        public OptionsProvider(T config)
        {
            _config = config;
        }
        public T Value => _config;

        public static IOptions<T> Get()
        {
            var r = new T();
            return new OptionsProvider<T>(r);
        }

        public static IOptions<T> Get(T instance)
        {
            return new OptionsProvider<T>(instance);
        }
    }
}
