﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Code;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using DesktopSearch.Core.Extractors.Roslyn;
using NUnit.Framework;

namespace CodeSearchTests.Indexing
{
    [TestFixture]
    public class RoslynPrototypingTests
    {
        [Test]
        public void SingleLineComment()
        {
            var classDecl = Walker.Create(@"
/// <summary>This is an xml doc comment</summary>
class C
{
}");

            var extractComment = APIWalker.ExtractComment(classDecl);

            Assert.AreEqual("<summary>This is an xml doc comment</summary>", extractComment);
        }

        [Test]
        public void SingleLineComment_multiple_lines()
        {
            var classDecl = Walker.Create(@"  /// <summary>This is an xml doc comment.
   /// the second line.</summary>
   class C
   {
   }");

            var extractComment = APIWalker.ExtractComment(classDecl);

            Assert.AreEqual("<summary>This is an xml doc comment.\r\nthe second line.</summary>", extractComment);
        }

        [Test]
        public void No_comment_results_in_string_empty()
        {
            var classDecl = Walker.Create(@"
class C
{
}");

            var extractComment = APIWalker.ExtractComment(classDecl);

            Assert.AreEqual(string.Empty, extractComment);
        }

        

        [Test]
        public void Class_attributes_extracted()
        {
            var sut = new APIWalker("",
                false);

            var path = TestContext.CurrentContext.TestDirectory + "\\TestData\\IndexExtractors\\Roslyn\\WCFServiceContract.cs";
            string sourcecode = File.ReadAllText(path);

            var tree = CSharpSyntaxTree.ParseText(sourcecode);

            var root = (CompilationUnitSyntax)tree.GetRoot();
            sut.Visit(root);

            var typeDescriptor = sut.ParsedTypes.Single(td => td.Name == "IWCFServiceInterface");

            Assert.AreEqual("Name = \"IWCFServiceInterface\", Namespace = \"Tests.SomeNameSpace\",\r\n                     SessionMode = SessionMode.Required,\r\n                     CallbackContract = typeof(IWCFServiceInterfaceEvents)", typeDescriptor.WCFContract);
            Assert.AreEqual(MEF.Export, typeDescriptor.MEFDefinition);
        }

        class Walker : SyntaxWalker
        {
            public static ClassDeclarationSyntax Create(string sourcecode)
            {
                var tree = CSharpSyntaxTree.ParseText(sourcecode);

                var root = (CompilationUnitSyntax)tree.GetRoot();

                var walker = new Walker();
                walker.Visit(root);
                Assert.NotNull(walker.Node);
                return walker.Node;
            }

            private ClassDeclarationSyntax _node;

            public ClassDeclarationSyntax Node
            {
                get { return _node; }
            }

            public override void Visit(SyntaxNode node)
            {
                if (node.IsKind(SyntaxKind.ClassDeclaration) && node.HasLeadingTrivia)
                {
                    _node = node as ClassDeclarationSyntax;
                    return;
                }
                base.Visit(node);
            }
        }
    }
}
