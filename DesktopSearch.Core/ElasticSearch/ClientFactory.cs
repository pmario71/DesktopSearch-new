using DesktopSearch.Core.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopSearch.Core.ElasticSearch
{
    public class ClientFactory
    {
        private LuceneConfig _cfg;
        private ConnectionSettings _settings;

        public ClientFactory(IConfigAccess<LuceneConfig> configAccess)
        {
            _cfg = configAccess.Get();
            _settings = new ConnectionSettings(_cfg.ElasticSearchUri);
            _settings.DefaultIndex("docsearch");
        }

        // TODO: rename to CreateSearchClient() when move to SimpleInjector is completed
        public IElasticClient SearchClient
        {
            get
            {
                //settings.DisableDirectStreaming()
                //        .OnRequestCompleted(details =>
                //        {
                //            Console.WriteLine("### ES REQEUST ###");
                //            if (details.RequestBodyInBytes != null) Console.WriteLine(Encoding.UTF8.GetString(details.RequestBodyInBytes));
                //            Console.WriteLine("### ES RESPONSE ###");
                //            if (details.ResponseBodyInBytes != null) Console.WriteLine(Encoding.UTF8.GetString(details.ResponseBodyInBytes));
                //        })
                //        .PrettyJson();

                return new ElasticClient(_settings);
            }
        }
    }
}
