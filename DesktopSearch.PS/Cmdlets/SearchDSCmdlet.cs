using DesktopSearch.PS.Composition;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Services;
using DesktopSearch.PS.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class SearchDSCmdlet : PSCmdlet, IDynamicParameters
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

        [Import]
        public IDocumentCollectionRepository Repository { get; set; }
        #endregion


        // ================================================================================================
        // Add DocumentCollection as parameter for Indexing and Search   !!!!!!!!
        // ================================================================================================

        public SearchDSCmdlet()
        {
            AppConfig.EnableLocalAssemblyResolution();
        }

        protected override void BeginProcessing()
        {
            this.Compose();
        }

        protected override void ProcessRecord()
        {
            var sw = Stopwatch.StartNew();
            AsyncPump.Run(async () =>
            {
                IEnumerable<object> results;

                IDocumentCollection collection;
                if (!Repository.TryGetDocumentCollectionByName((string)_docCollectionParameter.Value, out collection))
                {
                    throw new SystemException("Should not happen!");
                }
                if (base.ParameterSetName == "Code")
                {
                    results = await this.SearchService.SearchCodeAsync(collection, this.SearchCode, this.ElementType);
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


        private RuntimeDefinedParameter _docCollectionParameter;

        public object GetDynamicParameters()
        {
            // do not compose multiple times
            if (this.Repository == null)
            {
                this.Compose();
            }
            
            var parameters = new RuntimeDefinedParameterDictionary();

            _docCollectionParameter = CreateDocumentCollectionParameter();
            parameters.Add(_docCollectionParameter.Name, _docCollectionParameter);

            return parameters;
        }
        private RuntimeDefinedParameter CreateDocumentCollectionParameter()
        {
            string[] collectionNames;

            if (ParameterSetName == "Code")
            {
                collectionNames = Repository.GetIndexedCollections()
                    .Where(n => n.IndexingStrategy == IndexingStrategy.Code)
                    .Select(n => n.Name)
                    .ToArray();
            }
            else
            {
                collectionNames = Repository.GetIndexedCollections()
                    .Where(n => n.IndexingStrategy == IndexingStrategy.Documents)
                    .Select(n => n.Name)
                    .ToArray();
            }
            

            var p = new RuntimeDefinedParameter(
                "DocumentCollection",
                typeof(string),
                new Collection<Attribute>
                {
                        new ParameterAttribute {
                            Position = 1,
                            Mandatory = true,
                            HelpMessage = "Categorization criteria for documents."
                        },
                        new ValidateSetAttribute(collectionNames),
                        new ValidateNotNullOrEmptyAttribute()
                });
            return p;
        }
    }
}
