using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;
using DesktopSearch.PS.Composition;
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
    [Cmdlet(VerbsCommon.Remove, "DSDocumentCollection")]
    public class RemoveDSDocumentCollectionCmdlet : PSCmdlet
    {
        private const string AddFolder = "AddFolder";
        private const string psAllCollections = "AllCollections";
        private const string psSingleCollection = "SingleCollection";

        private IDocumentCollection _documentCollection;

        [Parameter(Mandatory = true, 
            HelpMessage = "Name of the DocumentCollection for which an additional folder shall be added.",
            ParameterSetName = psSingleCollection)]
        public string DocumentCollectionName { get; set; }

        [Parameter(Mandatory = true,
            HelpMessage = "Name of the DocumentCollection for which an additional folder shall be added.",
            ParameterSetName = psAllCollections)]
        public SwitchParameter All { get; set; }

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
            if (base.ParameterSetName == psAllCollections)
            {
                if (this.All)
                {
                    this.Repository.RemoveAll();
                }
            }
            else if (base.ParameterSetName == psSingleCollection)
            {
                this.Repository.Remove(this.DocumentCollectionName);
            }
        }

        protected override void EndProcessing()
        {
        }
    }
}
