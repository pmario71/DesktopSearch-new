using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.Lucene;
using DesktopSearch.Core.Services;
using DesktopSearch.PS.Composition;
using DesktopSearch.PS.Utils;

namespace DesktopSearch.PS.Cmdlets
{
    [Cmdlet(VerbsCommon.Clear, "DSIndex")]
    public class ClearDSIndexCmdlet : PSCmdlet
    {
        //[Parameter(Mandatory = false, HelpMessage = "Name of the DocumentCollection(s) for which the index shall be synced, otherwise all configured repositories are used.")]
        //public string DocumentCollectionName { get; set; }

        #region Dependencies

        [Import]
        internal ICodeIndexer Indexer { get; set; }

        #endregion

        protected override void BeginProcessing()
        {
            AppConfig.EnableLocalAssemblyResolution();
            this.Compose();
        }

        protected override void ProcessRecord()
        {
            AsyncPump.Run(async () =>
            {
                await this.Indexer.DeleteAllEntries();
            });
        }
    }
}
