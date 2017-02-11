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

namespace DesktopSearch.PS.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "TikaExtract")]
    public class GetTikaExtractCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Files to extract index for.")]
        public string[] File { get; set; }

        #region Dependencies
        public ITikaServerExtractor Extractor { get; set; }
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
                var progress = new ProgressRecord(1, "Extracting content ...", "Files");

                double filesTotalIncrement = 100.0 / this.File.Length;
                int filesProcessed = 0;

                foreach (var file in this.File)
                {
                    var pc = new Core.Extractors.ParserContext();
                    var result = await Extractor.ExtractAsync(pc, new System.IO.FileInfo(file));
                    WriteObject(result);

                    progress.PercentComplete = (int)Math.Round((++filesProcessed) * filesTotalIncrement);
                    WriteProgress(progress);
                }
            });
        }

        protected override void EndProcessing()
        {
        }
    }
}
