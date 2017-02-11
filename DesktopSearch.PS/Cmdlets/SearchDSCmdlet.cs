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

namespace DesktopSearch.PS.Cmdlets
{
    /// <summary>
    /// Persists configuration settings back to disk.
    /// </summary>
    [Cmdlet(VerbsCommon.Search, "DS")]
    public class SearchDSCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ValidateNotNullOrEmpty]
        public string SearchString { get; set; }

        //[Parameter(Mandatory = true)]
        //public string Path { get; set; }

        #region Dependencies
        [Import]
        internal ISearchService SearchService { get; set; }
        #endregion

        protected override void BeginProcessing()
        {
            this.Compose();
        }

        protected override void ProcessRecord()
        {
            AsyncPump.Run(async () =>
            {
                var results = await this.SearchService.SearchDocumentAsync(this.SearchString);
                    
                if (!results.Any())
                {
                    WriteWarning( $"Search did not return any hits!");
                }
                else
                    WriteObject(results, enumerateCollection: true);
            });
        }

        protected override void EndProcessing()
        {
            
        }
    }
}
