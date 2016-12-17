using DesktopSearch.Core;
using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.Extractors.Tika;
using DesktopSearch.PS.Utils;
using PowershellExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.PS
{
    [Cmdlet(VerbsCommon.Get, "TikaExtractRaw")]
    public class GetTikaExtractRawCmdlet : PSCmdlet
    {
        private Settings _config;
        private TikaServerExtractor _extractor;

        [Parameter(Mandatory = false, HelpMessage = "Files to extract index for.")]
        public string[] File { get; set; }


        #region Dependencies

        [Import]
        internal ConfigAccess ConfigAccess { set; get; }
        #endregion

        protected override void BeginProcessing()
        {
            AppConfig.EnsureLoaded();

            this.Compose();

            _config = ConfigAccess.Get();

            _extractor = new TikaServerExtractor();
        }

        protected override void ProcessRecord()
        {
            AsyncPump.Run(async () =>
            {
                var progress = new ProgressRecord(1, "Extracting content ...", "Files");

                double filesTotalIncrement = 100.0/this.File.Length;
                int filesProcessed = 0;

                foreach (var file in this.File)
                {
                    var result = await _extractor.SendToTikaAsync(new System.IO.FileInfo(file));
                    WriteObject(result);

                    progress.PercentComplete = (int)Math.Round((++filesProcessed)*filesTotalIncrement);
                    WriteProgress(progress);
                }
            });
        }

        protected override void EndProcessing()
        {
            if (_extractor != null)
            {
                _extractor.Dispose();
                _extractor = null;
            }
        }
    }
}
