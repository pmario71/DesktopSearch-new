using DesktopSearch.PS.Composition;
using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.Extractors.Tika;
using DesktopSearch.PS.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DesktopSearch.Core.Tagging;
using DesktopSearch.PS.UI;
using Shell32;

namespace DesktopSearch.PS.Cmdlets
{
    [Cmdlet(VerbsData.Edit, "Tag")]
    public class EditTagCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Files to extract index for.")]
        public string File { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Files to update.")]
        public string OutputFile { get; set; }

        #region Dependencies
        [Import]
        public Services.IKeywordSuggestions KeywordSuggestionService { get; set; }
        #endregion

        protected override void BeginProcessing()
        {
            AppConfig.EnableLocalAssemblyResolution();

            this.Compose();
        }

        protected override void ProcessRecord()
        {
            string ext = Path.GetExtension(this.File);

            DecodeIfShellLink(ext);
            if (StringComparer.OrdinalIgnoreCase.Compare(ext, ".pdf") != 0)
            {
                throw new Exception("Only pdf is supported!");
            }

            AsyncPump.Run(async () =>
            {
                var tagger = new Tagger();
                var tagDesc = await tagger.ReadAsync(new FileInfo(this.File));

                var dlg = new UI.TaggingDialog(this.KeywordSuggestionService);
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

        private void DecodeIfShellLink(string ext)
        {
            if (StringComparer.OrdinalIgnoreCase.Compare(ext, ".lnk") == 0)
            {
                var shell = new Shell();
                var folder = shell.NameSpace(Path.GetDirectoryName(this.File));
                var file = folder.Items().Item(Path.GetFileName(this.File));

                var linkObject = file.GetLink as ShellLinkObject;
                if (linkObject != null)
                {
                    linkObject.Resolve(5);
                    this.File = linkObject.Target.Path;
                }
            }
        }

        protected override void EndProcessing()
        {

        }
    }
}
