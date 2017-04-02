using System.IO;
using System.Linq;
using System.Text;
using DesktopSearch.Core.DataModel.Code;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DesktopSearch.Core.Extractors.Roslyn
{
    class StructureExtractor : CSharpSyntaxRewriter
    {
        private readonly string _filepath;
        private readonly StreamWriter _writer = new StreamWriter(new MemoryStream());

        private bool _isClassContext;

        public StructureExtractor(string filepath)
        {
            _filepath = filepath;
        }
        public string StructureText
        {
            get
            {
                _writer.Flush();
                var baseStreamPosition = _writer.BaseStream.Position;

                try
                {
                    _writer.BaseStream.Position = 0;
                    var reader = new StreamReader(_writer.BaseStream);
                
                    return reader.ReadToEnd();
                }
                finally
                {
                    _writer.BaseStream.Position = baseStreamPosition;
                }
            }
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var visibility = GetVisibility(node.Modifiers);
            if (visibility != Visibility.Public)
            {
                return null;
            }
            _isClassContext = true;

            _writer.Write("\r\n{0};;;", node.Identifier);
            _writer.Write("{0}({1})", _filepath, GetLine(node.Identifier, true));
            return base.VisitClassDeclaration(node.WithoutLeadingTrivia());
        }

        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            var visibility = GetVisibility(node.Modifiers);
            if (visibility != Visibility.Public)
            {
                return null;
            }
            _isClassContext = false;

            _writer.Write("\r\n{0};;;", node.Identifier);
            _writer.Write("{0}({1})", _filepath, GetLine(node.Identifier, true));
            return base.VisitInterfaceDeclaration(node.WithoutTrivia());
        }

        public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            var visibility = GetVisibility(node.Modifiers);
            if (visibility != Visibility.Public)
            {
                return null;
            }
            _isClassContext = false;

            _writer.Write("\r\n{0};;;", node.Identifier);
            _writer.Write("{0}({1})", _filepath, GetLine(node.Identifier, true));
            return base.VisitEnumDeclaration(node.WithoutTrivia());
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var visibility = GetVisibility(node.Modifiers);
            if (_isClassContext && visibility != Visibility.Public)
            {
                return null;
            }

            _writer.Write("\r\n;{0};", node.Identifier);
            _writer.Write("{0} {1};", node.ReturnType, node.ParameterList.ToString());
            _writer.Write("{0}({1})", _filepath, GetLine(node.Identifier));

            return base.VisitMethodDeclaration(node.WithoutTrivia());
        }

        //public override SyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node)
        //{
        //    var parentClass = (node.Parent as ClassDeclarationSyntax);

        //    if (parentClass == null)
        //    {
        //        return null;
        //    }
        //    var visibility = GetVisibility(parentClass.Modifiers);
        //    if (visibility != Visibility.Public)
        //    {
        //        return null;
        //    }

        //    _writer.Write(";{0};", node.);
        //    _writer.Write("{0};", node.ParameterList.ToString());

        //    return base.VisitVariableDeclaration(node);
        //}

        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            var visibility = GetVisibility(node.Modifiers);
            if (visibility != Visibility.Public)
            {
                return null;
            }

            _writer.Write("\r\n;{0};;", node.Declaration);
            _writer.Write("{0}({1})", _filepath, GetLine(node.Declaration.GetFirstToken()));

            return base.VisitFieldDeclaration(node.WithoutTrivia());
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var visibility = GetVisibility(node.Modifiers);
            if (_isClassContext && visibility != Visibility.Public)
            {
                return null;
            }

            _writer.Write("\r\n;{0};{1};", node.Identifier, node.Type);
            _writer.Write("{0}({1})", _filepath, GetLine(node.Identifier));

            return base.VisitPropertyDeclaration(node.WithoutTrivia());
        }

        public override SyntaxNode VisitBlock(BlockSyntax node)
        {
            return null;
        }

        private static Visibility GetVisibility(SyntaxTokenList list)
        {
            if (list.Any(SyntaxKind.PublicKeyword))
                return Visibility.Public;

            if (list.Any(SyntaxKind.InternalKeyword))
                return Visibility.Internal;

            return Visibility.Private;
        }

        private int _capturedLine = 0;

        private int GetLine(SyntaxToken node, bool capture=false)
        {
            if (capture)
            {
                var line = node.GetLocation().GetLineSpan().StartLinePosition.Line;
                _capturedLine = line;
                return line - 1;
            }
            else
            {
                var line = node.GetLocation().GetLineSpan().StartLinePosition.Line;
                return line + _capturedLine -1;
            }
        }
    }
}