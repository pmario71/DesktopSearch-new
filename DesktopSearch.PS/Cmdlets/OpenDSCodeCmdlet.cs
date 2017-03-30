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
        private string _config;

        [Parameter(Mandatory = true, HelpMessage = "List of TypeDescriptors as they are e.g. returned from Search-DS")]
        public TypeDescriptor[] TypeDescriptor { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Folder path to VS Code")]
        public string Path { get; set; }

        #region Dependencies
        [Import]
        public IConfigAccess<LuceneConfig> Config { get; set; }
        #endregion

        protected override void BeginProcessing()
        {
            AppConfig.EnableLocalAssemblyResolution();
            this.Compose();

            _config = GetVSCodePath();
        }

        protected override void ProcessRecord()
        {
            var pi = new ProcessStartInfo();
            pi.UseShellExecute = false;
            pi.CreateNoWindow = true;
            pi.FileName = _config;

            var sb = new StringBuilder("-g");
            for (int i = 0; i < Math.Min(3, this.TypeDescriptor.Length); i++)
            {
                sb.AppendFormat(" {0}:{1}", TypeDescriptor[i].FilePath, TypeDescriptor[i].LineNr);
            }
            pi.Arguments = sb.ToString();

            var p = Process.Start(pi);

            Host.UI.WriteLine("Starting VSCode ...");
        }

        private string GetVSCodePath()
        {
            var config = Config.Get();

            if (config.Tools.VSCode == null)
            {
                if (this.Path == null)
                {
                    throw new ArgumentException($"Path to 'Visual Studio Code' is not set. Please use optional -Path parameter to set it.");
                }
                config.Tools.VSCode = this.Path;
                Config.Save(config);
            }

            return config.Tools.VSCode;
        }

        protected override void EndProcessing()
        {
        }
    }
}
