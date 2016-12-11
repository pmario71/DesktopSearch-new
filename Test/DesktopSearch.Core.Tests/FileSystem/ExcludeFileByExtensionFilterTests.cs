using DesktopSearch.Core.FileSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DesktopSearch.Core.Tests.FileSystem
{
    [TestFixture]
    public class ExcludeFileByExtensionFilterTests
    {

        [TestCase(new string[] { }, new[] { "c:\\temp\\Test.cs", "c:\\test.xaml" })]
        [TestCase(new[] { "c:\\temp\\Test.txt" }, new[] { "c:\\temp\\Test.txt", "c:\\test.xaml" })]
        public void FilterByExtension_over_enumeration(string[] expectedResult, string[] file)
        {
            var sut = new ExcludeFileByExtensionFilter("cs", "xaml");
            var result = sut.FilterByExtension(file);
            
            Assert.True(expectedResult.SequenceEqual(result));
        }

        [TestCase("c:\\temp\\Test.cs", false)]
        [TestCase("c:\\temp\\Test.1.cs", false)]
        [TestCase("c:\\temp\\Test.1.CS", false)]
        [TestCase("c:\\temp\\Test.1.Cs", false)]
        [TestCase("c:\\temp\\Test.1.txt", true)]
        [TestCase("c:\\temp\\Test.xaml", false)]
        public void FilterByExtension_over_single_value(string file, bool expectedResult)
        {
            var sut = new ExcludeFileByExtensionFilter("cs", "xaml");
            Assert.AreEqual(expectedResult, sut.FilterByExtension(file));
        }
    }
}
