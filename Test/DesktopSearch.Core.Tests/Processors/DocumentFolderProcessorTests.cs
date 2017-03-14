using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Extractors.Tika;
using DesktopSearch.Core.Processors;
using DesktopSearch.Core.Services;
using DesktopSearch.Core.Tests.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using Nest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace DesktopSearch.Core.Tests.Processors
{
    [TestFixture]
    public class DocumentFolderProcessorTests : IDisposable
    {
        private List<string> _foldersCreated = new List<string>();

        public void Dispose()
        {
            foreach (var folder in _foldersCreated)
            {
                Directory.Delete(folder, true);
            }
        }


        [Test, Ignore("Broken because of Elastic to Lucene")]
        public void Run_on_empty_folder()
        {
            var docColRepo = new Moq.Mock<IDocumentCollectionRepository>();
            //var client = new Moq.Mock<IElasticClient>();
            var tikaExtractor = new Moq.Mock<ITikaServerExtractor>();
            
            string folder = CreateTestFolder();

            //TODO: fix
            var sut = new DocumentFolderProcessor(
                docColRepo.Object, 
                null, 
                CfgMocks.GetLuceneConfigMock(),
                tikaExtractor.Object);

            var cfgFolder = Folder.Create(folder);

            var dcMock = new Moq.Mock<IDocumentCollection>();
            dcMock.Setup(m => m.Name).Returns("Some name");
            cfgFolder.DocumentCollection = dcMock.Object;
            

            sut.ProcessAsync(cfgFolder);
        }

        [Test, Ignore("Elastic to Lucene")]
        public async Task Run_on_file()
        {
            var docColRepo = new Mock<IDocumentCollectionRepository>();
            //var client = new Moq.Mock<IElasticClient>();
            
            var logging = new Moq.Mock<ILogger<DocumentFolderProcessor>>();
            var tika = new Moq.Mock<ITikaServerExtractor>();

            var docDesc = new DocDescriptor();
            tika.Setup(t => t.ExtractAsync(It.IsAny<Extractors.ParserContext>(), It.IsAny<FileInfo>())).
                ReturnsAsync(docDesc);

            string folder = CreateTestFolder();
            var filePath = folder + "\\TestDocument.txt";
            File.WriteAllText(filePath, "The content");

            // ensure that mock return
            var result = new Mock<IIndexResponse>();
            result.Setup(t => t.IsValid)
                .Returns(true);

            //client.Setup(t => t.IndexAsync(It.Is<DocDescriptor>(d => d.Path == filePath), null, default(CancellationToken)))

            //TODO: fix
            //client.Setup(t => t.IndexAsync(It.IsAny<DocDescriptor>(), 
            //                               It.IsAny<Func<IndexDescriptor<DocDescriptor>, IIndexRequest>>(), 
            //                               It.IsAny<CancellationToken>()))
            //                                    .Returns(Task.FromResult<IIndexResponse>(result.Object));


            var sut = new DocumentFolderProcessor(
                docColRepo.Object, 
                null, 
                CfgMocks.GetLuceneConfigMock(),
                tika.Object
                /*, logging.Object*/);

            var cfgFolder = Folder.Create(folder);

            var dcMock = new Mock<IDocumentCollection>();
            dcMock.Setup(m => m.Name).Returns("Some name");
            cfgFolder.DocumentCollection = dcMock.Object;


            await sut.ProcessAsync(cfgFolder);

            //client.VerifyAll();            
        }

        private string CreateTestFolder()
        {
            var newFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(newFolder);
            _foldersCreated.Add(newFolder);

            return newFolder;
        }
    }
}
