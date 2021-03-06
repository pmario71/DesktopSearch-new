using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace CodeSearchTests.Indexing.Roslyn
{
    public class RoslynAPI_Prototyping
    {
        [Test]
        public void Get_visibility_of_class()
        {
            var classDoc = @"public class SomeClass
            {
                private SomeOtherClass someOtherClass;
            }";

            SyntaxTree classTree = SyntaxFactory.ParseSyntaxTree(classDoc);
            var classDecl = (ClassDeclarationSyntax)classTree.GetRoot().DescendantNodes().First(d => d is ClassDeclarationSyntax);

            Assert.True(classDecl.Modifiers.Any(t => t.IsKind(SyntaxKind.PublicKeyword) || t.IsKind(SyntaxKind.InternalKeyword)));
        }

        [Test]
        public void Get_type_of_field()
        {
            var classDoc = @"public class SomeClass
            {
                private SomeOtherClass someOtherClass;
            }";

            SyntaxTree classTree = SyntaxFactory.ParseSyntaxTree(classDoc);
            var classDecl = (ClassDeclarationSyntax)classTree.GetRoot().DescendantNodes().First(d => d is ClassDeclarationSyntax);
            var field = classDecl.Members.OfType<FieldDeclarationSyntax>().First();
            var fieldType = field.Declaration.Type;

            Assert.AreEqual("SomeOtherClass", fieldType.ToString());
        }

        [Test]
        public void Get_type_of_property()
        {
            var classDoc = @"public class SomeClass
            {
                public SomeOtherClass someOtherClass { get; };
            }";

            SyntaxTree classTree = SyntaxFactory.ParseSyntaxTree(classDoc);
            var classDecl = (ClassDeclarationSyntax)classTree.GetRoot().DescendantNodes().First(d => d is ClassDeclarationSyntax);
            var field = classDecl.Members.OfType<PropertyDeclarationSyntax>().First();
            var fieldType = field.Type;

            Assert.AreEqual("SomeOtherClass", fieldType.ToString());
        }
    }
}