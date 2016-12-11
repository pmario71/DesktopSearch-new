using System.Linq;

using DesktopSearch.Core.DataModel;
using DesktopSearch.Core.Extractors.Roslyn;
using DesktopSearch.Core.DataModel.Code;
using NUnit.Framework;

namespace CodeSearchTests.Indexing.Roslyn
{
    [TestFixture]
    public class RoslynParser_DefinedTypes_Tests
    {
        [Test]
        public void Classes_are_parsed()
        {
            const string csharp = @"   namespace HelloWorld.Something 
                                       { 
                                           public class TestClass
                                           { 
                                               public void Method()
                                               {
                                               }
                                           } 
                                       }";

            var parser = new RoslynParser();
            var APIMethods = parser.ExtractTypes(csharp);

            Assert.AreEqual(1, APIMethods.Count());

            var typeDescriptor = APIMethods.First();
            Assert.AreEqual("TestClass", typeDescriptor.Name);
            Assert.AreEqual("HelloWorld.Something", typeDescriptor.Namespace);
            Assert.AreEqual(ElementType.Class, typeDescriptor.ElementType);
        }

        [Test]
        public void Enums_are_parsed()
        {
            const string csharp = @"   namespace HelloWorld.Something 
                                       { 
                                           public enum TestEnum 
                                           { 
                                               Value1,
                                               Value2
                                           } 
                                       }";

            var parser = new RoslynParser();
            var APIMethods = parser.ExtractTypes(csharp);

            Assert.AreEqual(1, APIMethods.Count());
            
            var typeDescriptor = APIMethods.First();
            Assert.AreEqual("TestEnum", typeDescriptor.Name);
            Assert.AreEqual("HelloWorld.Something", typeDescriptor.Namespace);
            Assert.AreEqual(ElementType.Enum, typeDescriptor.ElementType);
        }

        [Test]
        public void Interfaces_are_parsed()
        {
            const string csharp = @"   namespace HelloWorld.Something 
                                       { 
                                           public interface ITest
                                           { 
                                               void Method();
                                           } 
                                       }";

            var parser = new RoslynParser();
            var APIMethods = parser.ExtractTypes(csharp);

            Assert.AreEqual(1, APIMethods.Count());
            
            var typeDescriptor = APIMethods.First();
            Assert.AreEqual("ITest", typeDescriptor.Name);
            Assert.AreEqual("HelloWorld.Something", typeDescriptor.Namespace);
            Assert.AreEqual(ElementType.Interface, typeDescriptor.ElementType);
        }

        [Test]
        public void Structs_are_parsed()
        {
            const string csharp = @"   namespace HelloWorld.Something 
                                       { 
                                           public struct TestStruct
                                           { 
                                               void Method();
                                           } 
                                       }";

            var parser = new RoslynParser();
            var APIMethods = parser.ExtractTypes(csharp);

            Assert.AreEqual(1, APIMethods.Count());

            var typeDescriptor = APIMethods.First();
            Assert.AreEqual("TestStruct", typeDescriptor.Name);
            Assert.AreEqual("HelloWorld.Something", typeDescriptor.Namespace);
            Assert.AreEqual(ElementType.Struct, typeDescriptor.ElementType);
        }
    }
}