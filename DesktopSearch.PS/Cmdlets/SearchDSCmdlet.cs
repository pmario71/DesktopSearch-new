﻿using DesktopSearch.PS.Composition;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;
using DesktopSearch.PS.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Code;
using System.Diagnostics;

namespace DesktopSearch.PS.Cmdlets
{
    /// <summary>
    /// Persists configuration settings back to disk.
    /// </summary>
    [Cmdlet(VerbsCommon.Search, "DS")]
    public class SearchDSCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, 
            ValueFromPipeline = true, Position = 0, ParameterSetName = "Code")]
        [Alias("cs")]
        [ValidateNotNullOrEmpty]
        public string SearchCode { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Code")]
        public ElementType? ElementType { get; set; }

        [Parameter(Mandatory = true,
            ValueFromPipeline = true, Position = 0, ParameterSetName = "Doc")]
        [ValidateNotNullOrEmpty]
        public string SearchDocs { get; set; }

        #region Dependencies
        [Import]
        internal ISearchService SearchService { get; set; }
        #endregion

        protected override void BeginProcessing()
        {
            AppConfig.EnableLocalAssemblyResolution();
            this.Compose();
        }

        protected override void ProcessRecord()
        {
            var sw = Stopwatch.StartNew();
            AsyncPump.Run(async () =>
            {
                IEnumerable<object> results;

                if (base.ParameterSetName == "Code")
                {
                    results = await this.SearchService.SearchCodeAsync(this.SearchCode, this.ElementType);
                }
                else
                {
                    results = await this.SearchService.SearchDocumentAsync(this.SearchDocs);
                }

                if (!results.Any())
                {
                    WriteWarning($"Search did not return any hits!");
                }
                else
                {
                    WriteVerbose($"Search took: {sw.ElapsedMilliseconds} [ms]");
                    PrintVerbose(results);
                    WriteObject(results, enumerateCollection: true);
                }
            });
        }

        private void PrintVerbose(IEnumerable<object> results)
        {
            if (!MyInvocation.BoundParameters.ContainsKey("Verbose"))
                return;

            if (ParameterSetName == "Code")
            {
                var sb = new StringBuilder();
                foreach (var tp in results.Cast<TypeDescriptor>().Take(5))
                {
                    sb.AppendLine($"{tp.Name} - {tp.Namespace}");
                }
                WriteVerbose(sb.ToString());
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var tp in results.Cast<DocDescriptor>().Take(5))
                {
                    sb.AppendLine($"{tp.Title} - {tp.Path}");
                }
                WriteVerbose(sb.ToString());
            }
        }

        protected override void EndProcessing()
        {
            
        }
    }
}
