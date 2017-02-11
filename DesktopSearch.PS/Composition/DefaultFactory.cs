using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DesktopSearch.PS.Composition
{
    internal class DefaultFactory
    {
        private readonly IEnumerable<string> _assembliesToIgnore;
        private static string _ownAssemblyName;

        public DefaultFactory()
        {
            _ownAssemblyName = typeof(DefaultFactory).Assembly.GetName().Name;
            _assembliesToIgnore = new[]
            {
                _ownAssemblyName,
                "System.*",          // ignore .net core assemblies, which get deployed locally
                "Microsoft.*"        // 
            };
        }

        internal DefaultFactory(IEnumerable<string> assembliesToIgnore) : this()
        {
            //Contract.Requires<ArgumentNullException>(assembliesToIgnore != null);
            if (assembliesToIgnore == null)
                throw new ArgumentNullException(nameof(assembliesToIgnore));

            _assembliesToIgnore = AddOwnAssemblyToList(assembliesToIgnore);
        }

        public Func<IHost> Create()
        {
            return InternalCreate;
        }

        private IHost InternalCreate()
        {
            Type hostImpl = CollectAndInspectAssemblies();

            var instance = (IHost)Activator.CreateInstance(hostImpl);
            return instance;
        }

        internal Type CollectAndInspectAssemblies()
        {
            var location = Path.GetDirectoryName(typeof(DefaultFactory).Assembly.Location);

            var comparer = new BlacklistComparer(_assembliesToIgnore);

            var assemblies = Directory.GetFiles(location, "*.dll")
                .Where(f => !comparer.Contains(Path.GetFileName(f)));

            foreach (var assembly in assemblies)
            {
                try
                {
                    Assembly loadedAssembly = Assembly.LoadFrom(assembly);

                    var types = from type in loadedAssembly.GetTypes()
                                where IsConcreteTypeImplementingInterface(type)
                                select type;

                    if (types.Any())
                    {
                        return types.First();
                    }
                }
                catch (Exception)
                {

                }
            }

            string msg = $"No host implementation found:\r\n{string.Join("; ", assemblies)}";
            throw new ApplicationException(msg);
        }

        internal static bool IsConcreteTypeImplementingInterface(Type type)
        {
            return (typeof(IHost).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
        }

        private static IEnumerable<string> AddOwnAssemblyToList(IEnumerable<string> assembliesToIgnore)
        {
            return new[] { _ownAssemblyName }.Concat(assembliesToIgnore);
        }
    }
}