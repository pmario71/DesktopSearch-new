using DesktopSearch.PS.Composition;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;
using DesktopSearch.PS.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Code;
using System.Diagnostics;

namespace DesktopSearch.PS.Cmdlets
{
    /// <summary>
    /// Persists configuration settings back to disk.
    /// </summary>
    [Cmdlet(VerbsCommon.Search, "DS")]
    public class SearchDSCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, ParameterSetName = "Code")]
        [ValidateNotNullOrEmpty]
        public string SearchCode { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Code")]
        public ElementType? ElementType { get; set; }

        #region Dependencies
        [Import]
        internal ISearchService SearchService { get; set; }
        #endregion

        protected override void BeginProcessing()
        {
            AppConfig.EnableLocalAssemblyResolution();
            this.Compose();
        }

        protected override void ProcessRecord()
        {
            var sw = Stopwatch.StartNew();
            AsyncPump.Run(async () =>
            {
                var results = await this.SearchService.SearchCodeAsync(this.SearchCode, this.ElementType);
                    
                if (!results.Any())
                {
                    WriteWarning( $"Search did not return any hits!");
                }
                else
                    WriteObject(results, enumerateCollection: true);
            });
            WriteVerbose($"Search took: {sw.ElapsedMilliseconds} [ms]");
        }

        protected override void EndProcessing()
        {
            
        }
    }
}
