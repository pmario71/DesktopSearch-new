using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.PS.Utils
{
    public abstract class AppConfig : IDisposable
    {
        private static object _isInitializedLock = new object();
        private static bool _isInitialized;
        private static bool _isAssemblyLoaderHooked;

        public static AppConfig Change(string path)
        {
            return new ChangeAppConfig(path);
        }

        /// <summary>
        /// Ensure that app.config is loaded
        /// </summary>
        public static void EnsureLoaded()
        {
            if (!_isInitialized)
            {
                lock (_isInitializedLock)
                {
                    if (!_isInitialized)
                    {
                        string loc = typeof(AppConfig).Assembly.Location;
                        string appConfigPath = $"{loc}.config";
                        
                        AppDomain currentDomain = AppDomain.CurrentDomain;
                        currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);

                        new ChangeAppConfig(appConfigPath);
                        _isInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Ensure that app.config is loaded
        /// </summary>
        public static void EnableLocalAssemblyResolution()
        {
            if (!_isAssemblyLoaderHooked)
            {
                lock (_isInitializedLock)
                {
                    if (!_isAssemblyLoaderHooked)
                    {
                        AppDomain currentDomain = AppDomain.CurrentDomain;
                        currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);

                        _isAssemblyLoaderHooked = true;
                    }
                }
            }
        }

        private static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");

            if (!File.Exists(assemblyPath))
            {
                return null;
            }

            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }

        public abstract void Dispose();

        private class ChangeAppConfig : AppConfig
        {
            private readonly string oldConfig =
                AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE").ToString();

            private bool disposedValue;

            public ChangeAppConfig(string path)
            {
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", path);
                ResetConfigMechanism();
            }

            public override void Dispose()
            {
                if (!disposedValue)
                {
                    AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", oldConfig);
                    ResetConfigMechanism();


                    disposedValue = true;
                }
                GC.SuppressFinalize(this);
            }

            private static void ResetConfigMechanism()
            {
                typeof(ConfigurationManager)
                    .GetField("s_initState", BindingFlags.NonPublic |
                                             BindingFlags.Static)
                    .SetValue(null, 0);

                typeof(ConfigurationManager)
                    .GetField("s_configSystem", BindingFlags.NonPublic |
                                                BindingFlags.Static)
                    .SetValue(null, null);

                typeof(ConfigurationManager)
                    .Assembly.GetTypes()
                    .Where(x => x.FullName ==
                                "System.Configuration.ClientConfigPaths")
                    .First()
                    .GetField("s_current", BindingFlags.NonPublic |
                                           BindingFlags.Static)
                    .SetValue(null, null);
            }
        }
    }
}