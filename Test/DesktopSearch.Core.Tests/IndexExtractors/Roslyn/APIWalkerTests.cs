using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Code;
using DesktopSearch.Core.Extractors.Roslyn;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace CodeSearchTests.Indexing.Roslyn
{
    [TestFixture]
    public class APIWalkerTests
    {
        [TestCase("Export", MEF.Export)]
        [TestCase("Import", MEF.Import)]
        public void Class_exported_with_MEF_attribute(string inc, MEF mef)
        {
            var sut = new APIWalker("",
                false);

            string sourcecode = $@"  namespace Something
{{
   /// <summary>This is an xml doc comment.
   /// the second line.</summary>
   [{inc}(typeof(ISomething))]   
   public class WCFServiceInterface
   {{
   }}
}}";

            sut.VisitSourceCode(sourcecode);

            var typeDescriptor = sut.ParsedTypes.Single(td => td.Name == "WCFServiceInterface");

            Assert.AreEqual(mef, typeDescriptor.MEFDefinition);
        }

        [Test]
        public void Class_flagged_as_ServiceContract()
        {
            var sut = new APIWalker("",
                false);

            string sourcecode = $@"  namespace Something
{{
   /// <summary>This is an xml doc comment.
   /// the second line.</summary>
   [ServiceContract(Name = ""IWCFServiceInterface"", Namespace = ""Tests.SomeNameSpace"")]   
   public class IWCFServiceInterface
   {{
   }}
}}";

            sut.VisitSourceCode(sourcecode);

            var typeDescriptor = sut.ParsedTypes.Single(td => td.Name == "IWCFServiceInterface");

            Assert.AreEqual("Name = \"IWCFServiceInterface\", Namespace = \"Tests.SomeNameSpace\"", typeDescriptor.WCFContract);
        }

        [Test]
        public void Interface_methods_are_extracted()
        {
            var sut = new APIWalker("",
                false);

            string sourcecode = $@"  namespace Something
{{
   /// <summary>This is an xml doc comment.
   /// the second line.</summary>
   public interface IWCFServiceInterface
   {{
        string GetResult(int i);
   }}
}}";

            sut.VisitSourceCode(sourcecode);

            var typeDescriptor = sut.ParsedTypes.Single(td => td.Name == "IWCFServiceInterface");

            Assert.AreEqual(1, typeDescriptor.Members.Count);
        }

        [Test]
        public void Enums_are_extracted()
        {
            var sut = new APIWalker("",
                false);

            string sourcecode = $@"  namespace Something
{{
   /// <summary>This is an xml doc comment.
   /// the second line.</summary>
   public enum ServiceState
   {{
        Started,
        Stopped,
   }}
}}";

            sut.VisitSourceCode(sourcecode);

            var typeDescriptor = sut.ParsedTypes.Single(td => td.Name == "ServiceState");

            Assert.IsTrue(typeDescriptor.Comment.Contains("xml doc comment"));

            // members are currently not extracted
            //Assert.AreEqual(2, typeDescriptor.Members.Count);
        }

        [Test]
        public void Properties_are_extracted()
        {
            var sut = new APIWalker("",
                false);

            string sourcecode = $@"  namespace Something
{{
   /// <summary>This is an xml doc comment.
   /// the second line.</summary>
   public class ServiceState
   {{
        public string MyProperty {{ get; set; }}
        string MyPropertyPrivate {{ get; set; }}
   }}
}}";

            sut.VisitSourceCode(sourcecode);

            var typeDescriptor = sut.ParsedTypes.Single(td => td.Name == "ServiceState");

            Assert.IsTrue(typeDescriptor.Comment.Contains("xml doc comment"));
            Assert.AreEqual(1, typeDescriptor.Members.Count(e => e.Type == MemberType.Property), "Only a single public event is defined");
            // members are currently not extracted
            //Assert.AreEqual(2, typeDescriptor.Members.Count);
        }


        [Test]
        public void Events_are_extracted()
        {
            var sut = new APIWalker("",
                false);

            string sourcecode = $@"  namespace Something
{{
   /// <summary>This is an xml doc comment.
   /// the second line.</summary>
   public class ServiceState
   {{
        public event SampleEventHandler SampleEvent;
        event SampleEventHandler SampleEvent2;

   }}
}}";

            sut.VisitSourceCode(sourcecode);

            var typeDescriptor = sut.ParsedTypes.Single(td => td.Name == "ServiceState");

            Assert.IsTrue(typeDescriptor.Comment.Contains("xml doc comment"));
            Assert.AreEqual(1, typeDescriptor.Members.Count(e => e.Type == MemberType.Event), "Only a single public event is defined");
            // members are currently not extracted
            //Assert.AreEqual(2, typeDescriptor.Members.Count);
        }

        public event EventHandler<string> EventName;
        [Test]
        public void RoslynAPI_prototyping_get_type_of_field()
        {
            //var fname = @"c:\Projects\Tools\DesktopSearch\DesktopSearch.Core\Extractors\ParserContext.cs";
            //var sut = new APIWalker(fname, true);
        }
    }

    static class APIWalkerExtension
    {
        public static void VisitSourceCode(this APIWalker walker, string sourcecode)
        {
            var tree = CSharpSyntaxTree.ParseText(sourcecode);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            walker.Visit(root);
        }
    }
}
