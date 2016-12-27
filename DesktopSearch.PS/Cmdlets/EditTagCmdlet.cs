using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.Extractors.Tika;
using DesktopSearch.PS.Utils;
using System;
using PowershellExtensions;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DesktopSearch.Core.Tagging;
using DesktopSearch.PS.UI;

namespace DesktopSearch.PS.Cmdlets
{
    [Cmdlet(VerbsData.Edit, "Tag")]
    public class EditTagCmdlet : PSCmdlet
    {
        private Settings _config;
        
        [Parameter(Mandatory = true, HelpMessage = "Files to extract index for.")]
        public string File { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Files to update.")]
        public string OutputFile { get; set; }

        #region Dependencies

        [Import]
        internal ConfigAccess ConfigAccess { set; get; }
        #endregion

        protected override void BeginProcessing()
        {
            AppConfig.EnableLocalAssemblyResolution();

            this.Compose();

            _config = ConfigAccess.Get();

            
        }

        protected override void ProcessRecord()
        {
            string ext = Path.GetExtension(this.File);
            if (StringComparer.OrdinalIgnoreCase.Compare(ext, ".pdf") != 0)
            {
                throw new Exception("Only pdf is supported!");
            }

            AsyncPump.Run(async () =>
            {
                var tagger = new Tagger();
                var tagDesc = await tagger.ReadAsync(new FileInfo(this.File));

                var dlg = new UI.TaggingDialog();
                dlg.TagDescriptor = tagDesc;

                DialogFactory.ShowDialog(dlg);

                this.WriteVerbose(tagDesc.ToString());

                FileInfo outputFile;
                if (this.OutputFile == null)
                {
                    outputFile = new FileInfo(File);
                }
                else
                {
                    outputFile = new FileInfo(OutputFile);
                    System.IO.File.Copy(File, OutputFile, true);
                }

                await tagger.WriteAsync(outputFile, tagDesc);
            });
        }

        protected override void EndProcessing()
        {

        }
    }
}
