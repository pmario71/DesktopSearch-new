using DesktopSearch.Core.Configuration;
using DesktopSearch.PS.Composition;
using DesktopSearch.PS.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.PS.Cmdlets
{
    [Cmdlet(VerbsCommon.Open, "DSLuke")]
    public class OpenDSLukeCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "Folder path to Luke*.jar")]
        public string Path { get; set; }

        #region Dependencies
        [Import]
        public IConfigAccess<LuceneConfig> Config { get; set; }
        #endregion

        protected override void BeginProcessing()
        {
            AppConfig.EnableLocalAssemblyResolution();
            this.Compose();
        }

        protected override void ProcessRecord()
        {
            var config = Config.Get();

            if (config.Tools.Luke == null)
            {
                if (this.Path == null)
                {
                    throw new ArgumentException($"Path to Lucene tool 'Luke' is not set. Please use optional -Path parameter to set it.");
                }
                config.Tools.Luke = this.Path;
                Config.Save(config);
            }

            string indexPath = System.IO.Path.GetFullPath(config.IndexDirectory);

            var pi = new ProcessStartInfo();
            pi.FileName = config.Tools.Luke;

            string cmdline = $"-index {indexPath} -force";
            pi.Arguments = cmdline;

            var p = Process.Start(pi);

            Host.UI.WriteLine("Starting Luke ...");
        }

        protected override void EndProcessing()
        {
        }
    }
}
