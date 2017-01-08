using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.PS.Utils;
using PowershellExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.PS.Cmdlets
{
    [Cmdlet(VerbsData.Import, "DSContent")]
    public class ImportDSContentCmdlet : PSCmdlet
    {
        private const string SetName_DocDescritor = "DocDescriptor";
        private const string SetName_File = "File";

        [Parameter(Mandatory = true, ParameterSetName = SetName_File, HelpMessage = "Files to extract index for.")]
        [ValidatePath]
        public string File { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = SetName_DocDescritor, HelpMessage = "DocDescriptor to import into index.")]
        [ValidateNotNull]
        public DocDescriptor DocDescriptor { get; set; }

        #region Dependencies
        [Import]
        public Core.Services.IIndexingService IndexService { get; set; }
        #endregion

        protected override void BeginProcessing()
        {
            AppConfig.EnableLocalAssemblyResolution();
            this.Compose();
        }

        protected override void ProcessRecord()
        {
            AsyncPump.Run(async () =>
            {
                if (this.ParameterSetName == SetName_DocDescritor)
                {
                    await this.IndexService.IndexDocumentAsync(DocDescriptor);
                }
                else
                {
                    await this.IndexService.IndexDocumentAsync(this.File);//, (string)_contentTypeParameter.Value);
                }
            });
        }

        protected override void EndProcessing()
        {
        }
    }
}
