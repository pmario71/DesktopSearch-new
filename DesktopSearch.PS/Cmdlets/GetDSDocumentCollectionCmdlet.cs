using DesktopSearch.PS.Composition;
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
    [Cmdlet(VerbsCommon.Get, "DSDocumentCollection")]
    public class GetDSDocumentCollectionCmdlet : PSCmdlet
    {
        #region Dependencies
        [Import]
        public IDocumentCollectionRepository Repository { get; set; }
        #endregion

        protected override void BeginProcessing()
        {
            AppConfig.EnableLocalAssemblyResolution();
            this.Compose();
        }

        protected override void ProcessRecord()
        {
            var cols = Repository.GetIndexedCollections();
            WriteObject(cols, enumerateCollection:true);
        }

        protected override void EndProcessing()
        {
        }
    }
}