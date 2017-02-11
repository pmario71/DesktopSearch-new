using DesktopSearch.PS.Composition;
using DesktopSearch.Core.Configuration;
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
    /// Persists configuration settings back to disk.
    /// </summary>
    [Cmdlet(VerbsData.Save, "DSSetting")]
    public class SaveDSSettingCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public LuceneConfig Setting { get; set; }

        #region Dependencies
        [Import]
        internal ConfigAccess ConfigAccess { set; get; }
        #endregion

        protected override void ProcessRecord()
        {
            this.Compose();

            ConfigAccess.SaveChanges(Setting);
        }
    }
}
