using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.ElasticSearch;
using DesktopSearch.Core.Utils.Async;
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
    /// <summary>
    /// Returns configuration settings.
    /// </summary>
    [Cmdlet(VerbsData.Initialize, "DSIndex")]
    public class InitializeDSIndexCmdlet : PSCmdlet
    {
        #region Dependencies
        [Import]
        internal ManagementService ManagementService { get; set; }
        #endregion

        protected override void ProcessRecord()
        {
            this.Compose();

            AsyncPump.Run(async () =>
            {
                await this.ManagementService.EnsureIndicesCreated();
            });
        }
    }
}
