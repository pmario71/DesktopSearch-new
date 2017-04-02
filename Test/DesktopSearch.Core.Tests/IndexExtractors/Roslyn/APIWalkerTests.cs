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
   public class IWCFServiceInterface
   {{
   }}
}}";

            sut.VisitSourceCode(sourcecode);

            var typeDescriptor = sut.ParsedTypes.Single(td => td.Name == "IWCFServiceInterface");

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
