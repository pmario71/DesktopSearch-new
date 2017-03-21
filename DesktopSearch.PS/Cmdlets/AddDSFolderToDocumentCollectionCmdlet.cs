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
    [Cmdlet(VerbsCommon.Add, "DSFolderToDocumentCollection")]
    public class AddDSFolderToDocumentCollectionCmdlet : PSCmdlet
    {
        private IDocumentCollection _documentCollection;

        [Parameter(Mandatory = true, HelpMessage = "Name of the DocumentCollection for which an additional folder shall be added.")]
        public string DocumentCollectionName { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "Folder path to add.")]
        [ValidatePath()]
        public string Path { get; set; }

        #region Dependencies
        [Import]
        public IDocumentCollectionRepository Repository { get; set; }
        #endregion

        protected override void BeginProcessing()
        {
            AppConfig.EnableLocalAssemblyResolution();
            this.Compose();

            if (!this.Repository.TryGetDocumentCollectionByName(this.DocumentCollectionName, out _documentCollection))
            {
                throw new Exception($"Specified DocumentCollection '{DocumentCollectionName}' is unknown!");
            }
        }

        protected override void ProcessRecord()
        {
            this.Repository.AddFolderToDocumentCollection(_documentCollection, this.Path);
        }

        protected override void EndProcessing()
        {
        }
    }
}