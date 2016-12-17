using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.FileSystem;
using Nest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DesktopSearch.Core.Processors
{
    public class DocumentFolderProcessor : IFolderProcessor
    {
        private readonly IElasticClient _client;
        //private readonly ILogger<DocumentFolderProcessor> _logging;

        public DocumentFolderProcessor(IElasticClient client/*, ILogger logging*/)
        {
            _client = client;
            //_logging = logging;
        }

        public Task Process(Folder folder)
        {
            return Process(folder, null);
        }

        public Task Process(Folder folder, IProgress<int> progress)
        {
            var extensionFilter = new ExcludeFileByExtensionFilter(".bin", ".lnk");

            var filesToProcess = Directory.GetFiles(folder.Path, "*", SearchOption.AllDirectories)
                                          .Where(f => extensionFilter.FilterByExtension(f));

            return ExtractFilesAsync(filesToProcess, progress);
        }

        private async Task ExtractFilesAsync(IEnumerable<string> filesToParse, IProgress<int> progress)
        {
            int current = 0;
            var maxFiles = filesToParse.Count();

            foreach (var filePath in filesToParse)
            {
                var stopWatch = Stopwatch.StartNew();
                IIndexResponse result = await IndexingHelper.IndexDocumentAsync(_client, filePath);

                if (!result.IsValid)
                {
                    //_logging.LogWarning($"Failed to index document: {filePath}!", result.OriginalException);
                    Console.WriteLine($"Failed to index document: {filePath}!", result.OriginalException);
                }
                else
                {
                    stopWatch.Stop();
                    //_logging.LogInformation($"Added '{Path.GetFileName(filePath)}' to index  took: {stopWatch.Elapsed.TotalSeconds} [s]");
                }

                ++current;
                if (progress != null)
                {
                    progress.Report((int)(current * 100 / (double)maxFiles));
                }
            }
        }
    }

    internal class IndexingHelper
    {
        public static async Task<IIndexResponse> IndexDocumentAsync(IElasticClient client, string filePath)
        {
            var content = Convert.ToBase64String(File.ReadAllBytes(filePath));
            var doc = new DocDescriptor
            {
                Path = filePath,
                Attachment = new Attachment() { Content=content }
            };

            //var req = new IndexRequest<DocDescriptor>("docsearch_test", "");
            //req.Document = doc;

            var result = await client.IndexAsync(doc);
            return result;
        }
    }
}
