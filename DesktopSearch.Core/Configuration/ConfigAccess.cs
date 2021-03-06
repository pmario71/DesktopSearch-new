﻿using System.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using System.Reflection;
using DesktopSearch.Core.Utils;
using Newtonsoft.Json.Serialization;
using static DesktopSearch.Core.Configuration.ConfigAccess;
using Microsoft.Extensions.Logging;

namespace DesktopSearch.Core.Configuration
{
    public class ConfigAccess : IConfigAccess
    {
        private static readonly JsonSerializerSettings _formatSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ContractResolver = new PrivateFieldResolver()
        };

        private readonly IStreamFactory _factory;

        public ConfigAccess(IStreamFactory factory)
        {
            _factory = factory;
        }

        public LuceneConfig Get()
        {
            string content = null;

            
            using (var rd = new StreamReader(_factory.GetReadableStream(nameof(LuceneConfig))))
            {
                content = rd.ReadToEnd();
            }

            var foldersToIndex = JsonConvert.DeserializeObject<LuceneConfig>(content, _formatSettings);

            return foldersToIndex ?? new LuceneConfig();
        }

        public void SaveChanges(LuceneConfig settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var serialized =
            JsonConvert.SerializeObject(
                settings,
                _formatSettings);

            using (var sw = new StreamWriter(_factory.GetWritableStream(nameof(LuceneConfig))))
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

    internal class ConfigAccess<T> : IConfigAccess<T>
        where T : class, new()
    {
        private static readonly JsonSerializerSettings FormatSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ContractResolver = new PrivateFieldResolver(),
            TypeNameHandling = TypeNameHandling.All
        };

        private readonly IStreamFactory                 _factory;
        private readonly Lazy<ILogger<ConfigAccess<T>>> _logger = new Lazy<ILogger<ConfigAccess<T>>>(Logging.GetLogger<ConfigAccess<T>>);

        public ConfigAccess(IStreamFactory factory)
        {
            _factory = factory;
        }

        public T Get()
        {
            string content = null;

            using (var rd = new StreamReader(_factory.GetReadableStream(typeof(T).Name)))
            {
                content = rd.ReadToEnd();
            }

            T foldersToIndex = null;

            try
            {
                foldersToIndex = JsonConvert.DeserializeObject<T>(content, FormatSettings);
            }
            catch(JsonSerializationException ex)
            {
                _logger.Value.LogWarning(new EventId(LoggedIds.ErrorDeserializingConfiguration), 
                                         $"Failed to deserialize type {typeof(T)} from configuration!", 
                                         ex);
            }

            return foldersToIndex ?? new T();
        }

        public void Save(T config)
        {
            var serialized = JsonConvert.SerializeObject(config,
                                                         FormatSettings);

            using (var sw = new StreamWriter(_factory.GetWritableStream(typeof(T).Name)))
            {
                sw.Write(serialized);
            }
        }
    }

}
