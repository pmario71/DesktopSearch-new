using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.Extractors.Roslyn;
using DesktopSearch.Core.Services;
using DesktopSearch.PS.Composition;
using DesktopSearch.PS.Utils;

namespace DesktopSearch.PS.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "DSCodeStructure")]
    public class GetDSCodeStructureCmdlet : PSCmdlet
    {
        private RoslynParser _parser;

        [Parameter(Mandatory = true, HelpMessage = "Files to extract index for.")]
        public string File { get; set; }

        #region Dependencies
        #endregion

        protected override void BeginProcessing()
        {
            AppConfig.EnableLocalAssemblyResolution();
            this.Compose();

            _parser = new RoslynParser();
        }

        protected override void ProcessRecord()
        {
            var sw = Stopwatch.StartNew();

            var typeDescriptors = _parser.ExtractTypes(new[] {File});

            WriteVerbose($"Generating statistics took: {sw.ElapsedMilliseconds} [ms]");

            WriteObject(typeDescriptors, enumerateCollection:true);
        }
    }
}
