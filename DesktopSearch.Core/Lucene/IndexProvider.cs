using DesktopSearch.Core.Configuration;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Lucene
{
    class IndexProvider : IIndexProvider
    {
        private readonly IConfigAccess _configuration;

        public IndexProvider(IConfigAccess configuration)
        {
            _configuration = configuration;
        }

        public Directory GetIndexDirectory()
        {
            var cfg = _configuration.Get();
            return FSDirectory.Open(new System.IO.DirectoryInfo(cfg.IndexDirectory));
        }
    }

    public class InMemoryIndexProvider : IIndexProvider
    {
        public Directory GetIndexDirectory()
        {
            return new RAMDirectory();
        }
    }

    public interface IIndexProvider
    {
        Directory GetIndexDirectory();
    }
}
