using DesktopSearch.Core.Configuration;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Directory = Lucene.Net.Store.Directory;

namespace DesktopSearch.Core.Lucene
{
    class IndexProvider : IIndexProvider
    {
        private readonly IConfigAccess _configuration;

        public IndexProvider(IConfigAccess configuration)
        {
            _configuration = configuration;
        }

        public Directory GetIndexDirectory(IndexType indexType)
        {
            var cfg = _configuration.Get();

            string folder = Path.Combine(cfg.IndexDirectory, indexType.ToString());
            return FSDirectory.Open(new System.IO.DirectoryInfo(folder));
        }
    }

    public class InMemoryIndexProvider : IIndexProvider
    {
        public Directory GetIndexDirectory(IndexType indexType)
        {
            return new RAMDirectory();
        }
    }

    public interface IIndexProvider
    {
        Directory GetIndexDirectory(IndexType indexType);
    }

    public enum IndexType
    {
        Document=0,
        Code=1,
    }
}
