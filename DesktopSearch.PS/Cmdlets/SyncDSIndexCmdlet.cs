using DesktopSearch.Core;
using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;
using DesktopSearch.PS.Utils;
using PowershellExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.PS
{
    [Cmdlet(VerbsData.Sync, "DSIndex")]
    public class SyncDSIndexCmdlet : PSCmdlet
    {
        private Settings _config;

        [Parameter(Mandatory = false, HelpMessage = "Name of the DocumentCollection(s) for which the index shall be synced, otherwise all configured repositories are used.")]
        public string DocumentCollectionName { get; set; }


        #region Dependencies

        [Import]
        internal IDocumentCollectionRepository Repository { get; set; }

        [Import]
        internal IIndexingService IndexingService { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            this.Compose();
        }

        protected override void ProcessRecord()
        {
            List<IFolder> folders = FindCollectionsToUpdateIndexFor();

            AsyncPump.Run(async () =>
            {
                var progress = new ProgressRecord(1, "Synching Index", "Folder");
                Action<int> progressCallback = p =>
                {
                    progress.PercentComplete = p;
                    WriteProgress(progress);
                };

                var aggregator = new DesktopSearch.Core.Utils.Async.AggregatingProgressReporter(progressCallback);

                foreach (var folder in folders)
                {
                    IProgress<int> pc = aggregator.CreateClient();

                    await this.IndexingService.IndexRepositoryAsync(folder, pc);
                }
            });
        }

        private List<IFolder> FindCollectionsToUpdateIndexFor()
        {
            var folders = new List<IFolder>();
            IEnumerable<IDocumentCollection> indexedCollection = null;

            if (DocumentCollectionName == null)
            {
                indexedCollection = Repository.GetIndexedCollections();
            }
            else
            {
                IDocumentCollection dtc;
                if (!Repository.TryGetDocumentCollectionByName(this.DocumentCollectionName, out dtc))
                {
                    throw new ArgumentException($"Name of the IndexedCollection '{this.DocumentCollectionName}' is unknown!");
                }
                indexedCollection = new[] { dtc };
            }

            // filter for locally available collections
            foreach (var dtc in indexedCollection)
            {
                IFolder folder;
                if (this.Repository.TryGetConfiguredLocalFolder(dtc, out folder))
                {
                    folders.Add(folder);
                }
            }

            return folders;
        }
    }
}
