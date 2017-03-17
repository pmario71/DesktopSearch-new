using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Extractors.Tika;
using DesktopSearch.Core.Processors;
using DesktopSearch.Core.Services;
using DesktopSearch.Core.Tests.Utils;
using Microsoft.Extensions.Logging;
using Moq;
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

        private string CreateTestFolder()
        {
            var newFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(newFolder);
            _foldersCreated.Add(newFolder);

            return newFolder;
        }
    }
}
