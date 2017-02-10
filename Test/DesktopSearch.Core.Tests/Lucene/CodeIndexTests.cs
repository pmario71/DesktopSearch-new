using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Code;
using DesktopSearch.Core.Lucene;
using Lucene.Net.Store;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Tests.Lucene
{
    [TestFixture]
    public class CodeIndexTests
    {

        [Test]
        public async Task Index_Type_Descriptor()
        {
            var indexProviderMock = new Mock<IIndexProvider>();

            indexProviderMock.Setup(m => m.GetIndexDirectory())
                             .Returns(new RAMDirectory());
            

            var sut = new CodeIndex(indexProviderMock.Object);

            var td = new TypeDescriptor(ElementType.Class, "TestClass", Visibility.Public, "syngo.Common.Test", "c:\\temp\\filename.cs", 123, "Some commend");
            await sut.IndexAsync(new[] { td});


            var results = sut.Search("name:test");

            Console.WriteLine("Results");
            Console.WriteLine("-------");
            foreach (var result in results)
            {
                Console.WriteLine($"{result.Name}");
            }
        }

    }
}
