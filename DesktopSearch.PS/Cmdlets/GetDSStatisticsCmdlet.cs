using DesktopSearch.Core.Services;
using DesktopSearch.PS.Composition;
using DesktopSearch.PS.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.PS.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "DSStatistics")]
    public class GetDSStatisticsCmdlet : PSCmdlet
    {
        #region Dependencies
        [Import]
        internal IIndexingStatistics IndexingStatistics { set; get; }
        #endregion

        protected override void ProcessRecord()
        {
            AppConfig.EnableLocalAssemblyResolution();
            this.Compose();

            AsyncPump.Run(async () =>
            {
                var sw = Stopwatch.StartNew();
                var res = await IndexingStatistics.GetIndexStatistics();

                WriteVerbose($"Generating statistics took: {sw.ElapsedMilliseconds} [ms]");

                WriteObject(res);
            });
        }
    }
}
