using System.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace DesktopSearch.Core.Configuration
{
    public class ConfigAccess : IConfigAccess
    {
        private static JsonSerializerSettings _formatSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ContractResolver = new PrivateFieldResolver()
        };

        private IStreamFactory _factory;

        public ConfigAccess(IStreamFactory factory)
        {
            _factory = factory;
        }

        public Settings Get()
        {
            string content = null;

            
            using (var rd = new StreamReader(_factory.GetReadableStream()))
            {
                content = rd.ReadToEnd();
            }

            var foldersToIndex = JsonConvert.DeserializeObject<Settings>(content, _formatSettings);

            return foldersToIndex ?? new Settings();
        }

        public static string GetJSONExample()
        {
            throw new NotImplementedException("Not necessary anymore!");
            //var fs = new FoldersToIndex()
            //{
            //    Folders = new[]
            //    {
            //        Folder.Create("c:\\temp", indexingType:"Code"),
            //        Folder.Create("c:\\temp", indexingType:"Documents"),
            //    }.ToList()
            //};

            //var serialized =
            //JsonConvert.SerializeObject(
            //    fs,
            //    _formatSettings);

            //return serialized;
        }

        public void SaveChanges(Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var serialized =
            JsonConvert.SerializeObject(
                settings,
                _formatSettings);

            using (var sw = new StreamWriter(_factory.GetWritableStream()))
            {
                sw.Write(serialized);
            }
        }

        public class PrivateFieldResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .Select(p => base.CreateProperty(p, memberSerialization))
                            .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                       .Select(f => base.CreateProperty(f, memberSerialization)))
                            .ToList();
                props.ForEach(p => { p.Writable = true; p.Readable = true; });
                return props;
            }
        }
    }

    
}
