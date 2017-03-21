using System;
using System.Threading.Tasks;
using DesktopSearch.Core.Contracts;
using DesktopSearch.Core.Lucene;

namespace DesktopSearch.Core.Services
{
    public interface IIndexingStatistics
    {
        Task<IndexStatistics> GetIndexStatistics();
    }

    public class IndexingStatisticsService : IIndexingStatistics
    {
        private ICodeIndexer _indexer;

        public IndexingStatisticsService(ICodeIndexer indexer)
        {
            _indexer = indexer;
        }

        public async Task<IndexStatistics> GetIndexStatistics()
        {
            var stat = new IndexStatistics();
            await _indexer.GetStatistics(stat);

            return stat;
        }
    }
}
