using DesktopSearch.PS.Composition;
using DesktopSearch.Core.FileSystem;
using DesktopSearch.Core.Configuration;
using DesktopSearch.PS.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;

namespace DesktopSearch.PS
{
    [Cmdlet(VerbsCommon.Add, "DSDocumentCollection")]
    public class AddDSDocumentCollectionCmdlet : PSCmdlet
    {
        private const string AddFolder = "AddFolder";
        private IDocumentCollection _documentCollection;

        [Parameter(Mandatory = true, HelpMessage = "Name of the DocumentCollection for which an additional folder shall be added.")]
        public string DocumentCollectionName { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Name of the Document Repository for which an additional folder shall be added.")]
        public IndexingStrategy? IndexingStrategy { get; set; }

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
                if (!IndexingStrategy.HasValue)
                {
                    throw new Exception($"Specified {nameof(DocumentCollectionName)} '{DocumentCollectionName}' is unknown! In order to create a new, {nameof(IndexingStrategy)} needs to be specified as well.");
                }

                IDocumentCollection dt = DocumentCollection.Create(this.DocumentCollectionName, this.IndexingStrategy.Value);
                this.Repository.AddDocumentCollection(dt);
                _documentCollection = dt;
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
