using DesktopSearch.Core.DataModel.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.PS.Cmdlets
{
    /// <summary>
    /// Persists configuration settings back to disk.
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "DSIndex")]
    public class SaveDSSettingCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public DocDescriptor DocDescriptor { get; set; }

        [Parameter(Mandatory = true)]
        public string Path { get; set; }

        #region Dependencies
        #endregion

        protected override void BeginProcessing()
        {

        }

        protected override void ProcessRecord()
        {
        }

        protected override void EndProcessing()
        {
            
        }
    }
}
