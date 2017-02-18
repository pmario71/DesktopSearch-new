using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Code;
using DesktopSearch.PS.Composition;
using DesktopSearch.PS.Utils;

namespace DesktopSearch.PS.Cmdlets
{
    [Cmdlet(VerbsCommon.Open, "DSCode")]
    public class OpenDSCodeCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Folder path to Luke*.jar")]
        public TypeDescriptor[] TypeDescriptor { get; set; }

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

            if (config.Tools.VSCode == null)
            {
                if (this.Path == null)
                {
                    throw new ArgumentException($"Path to Lucene tool 'Luke' is not set. Please use optional -Path parameter to set it.");
                }
                config.Tools.VSCode = this.Path;
                Config.Save(config);
            }

            string indexPath = System.IO.Path.GetFullPath(config.IndexDirectory);

            var pi = new ProcessStartInfo();
            pi.UseShellExecute = false;
            pi.CreateNoWindow = true;
            pi.FileName = config.Tools.VSCode;

            var sb = new StringBuilder("-g");
            for (int i = 0; i < Math.Min(3,this.TypeDescriptor.Length); i++)
            {
                sb.AppendFormat(" {0}:{1}", TypeDescriptor[i].FilePath, TypeDescriptor[i].LineNr);
            }
            pi.Arguments = sb.ToString();

            var p = Process.Start(pi);

            Host.UI.WriteLine("Starting VSCode ...");
        }

        protected override void EndProcessing()
        {
        }
    }
}
