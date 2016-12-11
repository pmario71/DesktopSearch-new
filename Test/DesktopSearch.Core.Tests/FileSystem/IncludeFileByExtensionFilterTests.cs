using DesktopSearch.Core.FileSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DesktopSearch.Core.FileSystem
{
    [TestFixture]
    public class IncludeFileByExtensionFilterTests
    {

        [TestCase("c:\\temp\\Test.cs")]
        [TestCase("c:\\temp\\Test.1.cs")]
        [TestCase("c:\\temp\\Test.1.CS")]
        [TestCase("c:\\temp\\Test.1.Cs")]
        public void FilterByExtension_over_enumeration(string file)
        {
            var files = new[] {file};

            var sut = new IncludeFileByExtensionFilter("cs");
            var result = sut.FilterByExtension(files);
            
            //CollectionAssert.Equal(files, result, StringComparer.OrdinalIgnoreCase);
            Assert.True(files.SequenceEqual(result));
        }

        [TestCase("c:\\temp\\Test.cs", true)]
        [TestCase("Test.cs", true)]
        [TestCase("c:\\temp\\Test.1.cs", true)]
        [TestCase("c:\\temp\\Test.1.Cs", true)]
        [TestCase("Test.fds", false)]
        public void FilterByExtension_over_single_value(string file, bool expectedResult)
        {
            var sut = new IncludeFileByExtensionFilter("cs");
            Assert.AreEqual(expectedResult, sut.FilterByExtension(file));
        }
    }
}
