using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Code;
using DesktopSearch.Core.Lucene;
using Lucene.Net.Store;
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
            var dir = new RAMDirectory();

            var sut = new CodeIndex(dir);

            var td = new TypeDescriptor(ElementType.Class, "TestClass", Visibility.Public, "syngo.Common.Test", "c:\\temp\\filename.cs", 123, "Some commend");
            await sut.IndexAsync(new[] { td});
        }

    }
}
