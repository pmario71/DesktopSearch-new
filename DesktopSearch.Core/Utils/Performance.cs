using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Utils
{
    internal class Performance : IPerformance
    {
        private const string CategoryName = "Indexing";

        public static void RegisterPerformanceCounters()
        {
            //if (!PerformanceCounterCategory.Exists(CategoryName))
            {

                //PerformanceCounterCategory customCategory = new PerformanceCounterCategory(CategoryName);

                CounterCreationDataCollection counters =
                     new CounterCreationDataCollection();

                var c1 = new CounterCreationData();
                c1.CounterName = IndexingCounters.FilesRead.ToString();
                c1.CounterType = PerformanceCounterType.RateOfCountsPerSecond32;
                c1.CounterHelp = "Files read from disk";

                var c2 = new CounterCreationData();
                c2.CounterName = IndexingCounters.FilesParsed.ToString();
                c2.CounterType = PerformanceCounterType.RateOfCountsPerSecond32;
                c2.CounterHelp = "Files parsed using Roslyn";

                var c3 = new CounterCreationData();
                c3.CounterName = IndexingCounters.FilesIndexed.ToString();
                c3.CounterType = PerformanceCounterType.RateOfCountsPerSecond32;
                c3.CounterHelp = "Files indexed by Lucene.net";

                counters.Add(c1);
                counters.Add(c2);
                counters.Add(c3);

                if (PerformanceCounterCategory.Exists(CategoryName))
                {
                    PerformanceCounterCategory.Delete(CategoryName);
                }

                PerformanceCounterCategory.Create(CategoryName, 
                                                  "DesktopSearch Indexing PerformanceCounters", 
                                                  PerformanceCounterCategoryType.SingleInstance, 
                                                  counters);
            }
        }

        public PerformanceCounter GetCounter(IndexingCounters counter)
        {
            return new PerformanceCounter(CategoryName, counter.ToString());
        }
    }

    internal interface IPerformance
    {
        PerformanceCounter GetCounter(IndexingCounters counter);
    }

    public enum IndexingCounters
    {
        FilesRead,
        FilesParsed,
        FilesIndexed,
    }
}
