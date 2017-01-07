﻿using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Processors;
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


        [Test]
        public void Run_on_empty_folder()
        {
            var client = new Moq.Mock<IElasticClient>();
            var cfg = new ElasticSearchConfig();
            var logging = new Moq.Mock<ILogger<DocumentFolderProcessor>>();

            string folder = CreateTestFolder();


            var sut = new DocumentFolderProcessor(client.Object, cfg /*, logging.Object*/);

            var cfgFolder = Folder.Create(folder);

            var dcMock = new Mock<IDocumentCollection>();
            dcMock.Setup(m => m.Name).Returns("Some name");
            cfgFolder.DocumentCollection = dcMock.Object;
            

            sut.ProcessAsync(cfgFolder);
        }

        [Test]
        public async Task Run_on_file()
        {
            //TODO: mock TikaExtractor to fix test case
            var client = new Moq.Mock<IElasticClient>();
            var cfg = new ElasticSearchConfig();
            var logging = new Moq.Mock<ILogger<DocumentFolderProcessor>>();

            string folder = CreateTestFolder();
            var filePath = folder + "\\TestDocument.txt";
            File.WriteAllText(filePath, "The content");

            // ensure that mock return
            var result = new Mock<IIndexResponse>();
            result.Setup(t => t.IsValid)
                .Returns(true);

            //client.Setup(t => t.IndexAsync(It.Is<DocDescriptor>(d => d.Path == filePath), null, default(CancellationToken)))
            client.Setup(t => t.IndexAsync(It.IsAny<DocDescriptor>(), 
                                           It.IsAny<Func<IndexDescriptor<DocDescriptor>, IIndexRequest>>(), 
                                           It.IsAny<CancellationToken>()))
                                                .Returns(Task.FromResult<IIndexResponse>(result.Object));


            var sut = new DocumentFolderProcessor(client.Object, cfg /*, logging.Object*/);

            var cfgFolder = Folder.Create(folder);

            var dcMock = new Mock<IDocumentCollection>();
            dcMock.Setup(m => m.Name).Returns("Some name");
            cfgFolder.DocumentCollection = dcMock.Object;


            await sut.ProcessAsync(cfgFolder);

            client.VerifyAll();            
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
