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
    [Cmdlet(VerbsData.Import, "File")]
    public class ImportFileCmdlet : PSCmdlet, IDynamicParameters
    {
        private RuntimeDefinedParameter _contentTypeParameter;

        [Parameter(Mandatory = true, HelpMessage = "Files to extract index for.")]
        public string File { get; set; }

        #region Dependencies
        [Import]
        public Core.Services.ISearchService SearchService { get; set; }
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
                await this.SearchService.IndexDocumentAsync(this.File, (string)_contentTypeParameter.Value);
            });
        }

        protected override void EndProcessing()
        {
        }

        public object GetDynamicParameters()
        {
            var parameters = new RuntimeDefinedParameterDictionary();

            _contentTypeParameter = CreateContentTypeParameter();
            parameters.Add(_contentTypeParameter.Name, _contentTypeParameter);

            return parameters;
        }

        private RuntimeDefinedParameter CreateContentTypeParameter()
        {
            var p = new RuntimeDefinedParameter(
                "ContentType",
                typeof(string),
                new Collection<Attribute>
                {
                    new ParameterAttribute {
                        Position = 1,
                        Mandatory = true,
                        HelpMessage = "Categorization criteria for documents."
                    },
                    new ValidateSetAttribute(Core.Configuration.DocumentSearch.ContentType.ToArray()),
                    new ValidateNotNullOrEmptyAttribute()
                });

            return p;
        }
    }
}
