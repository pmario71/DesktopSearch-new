using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DesktopSearch.Core.DataModel;
using DesktopSearch.Core.DataModel.Code;
using Lucene.Net.Support;

namespace DesktopSearch.Core.Extractors.Roslyn
{
    internal class APIWalker : CSharpSyntaxWalker
    {
        private readonly string _filePath;
        private readonly bool _ignoreTesterNamespaces;
        private bool _isClassContext;
        private string _namespace;
        private readonly List<TypeDescriptor> _parsedTypes = new List<TypeDescriptor>();
        private TypeDescriptor _typeDescriptor;

        public APIWalker(string filePath, bool ignoreTesterNamespaces)
        {
            this._filePath = filePath;
            this._ignoreTesterNamespaces = ignoreTesterNamespaces;
        }

        public IEnumerable<TypeDescriptor> ParsedTypes
        {
            get { return this._parsedTypes; }
        }

        public override void Visit(SyntaxNode node)
        {
            //var syntaxKind = node.Kind();

            //Console.WriteLine("syntaxkind: {0}\r\n\t{1}", syntaxKind.ToString(), node.GetText().ToString());

            //if (syntaxKind == SyntaxKind.NamespaceDeclaration)
            //{
            //    this._namespace = ((NamespaceDeclarationSyntax)node).Name.ToFullString().PrepareNamespace();
            //    if (this._namespace.IndexOf("test", StringComparison.OrdinalIgnoreCase) > 0)
            //    {
            //        // ignore namespaces that contain 'test' token
            //        return;
            //    }
            //}
            //else if (syntaxKind == SyntaxKind.Attribute)
            //{
            //    var attributeSyntax = ((Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax) node);
            //    //Console.WriteLine($"Name: {attributeSyntax.Name} / {attributeSyntax.ArgumentList}");

            //    var name = attributeSyntax.Name.ToString();

            //    switch (name)
            //    {
            //        case "ServiceContract":
            //        {
            //            _typeDescriptor.WCFContract = attributeSyntax.ArgumentList?.Arguments.ToString();
            //            return;
            //        }
            //        case "Export":
            //        {
            //            _typeDescriptor.MEFDefinition = MEF.Export;
            //            return;
            //        }
            //        case "Import":
            //        {
            //            _typeDescriptor.MEFDefinition = MEF.Import;
            //            return;
            //        }
            //    }
            //    return;
            //}
            //else if (syntaxKind == SyntaxKind.ClassDeclaration)
            //{
            //    var classDeclaration = ((ClassDeclarationSyntax) node);

            //    var visibility = GetVisibility(classDeclaration.Modifiers);
            //    if (visibility == Visibility.Private)
            //    {
            //        // not a public class => ignore this leave of the syntaxtree
            //        return;
            //    }
            //    var @class = classDeclaration.Identifier.ValueText.Trim();

            //    if (string.IsNullOrEmpty(@class))
            //    {
            //        // ignore
            //        return;
            //    }

            //    string comment = ExtractComment(classDeclaration) ?? String.Empty;

            //    _typeDescriptor = new TypeDescriptor(ElementType.Class, @class, visibility,
            //        _namespace, _filePath, ExtractLineNR(node), comment);

            //    _typeDescriptor.APIDefinition = APIDefinitionExtractor.Parse(comment);

            //    _parsedTypes.Add(_typeDescriptor);
            //}
            //else if (syntaxKind == SyntaxKind.MethodDeclaration)
            //{
            //    var methodDeclaration = ((MethodDeclarationSyntax) node);

            //    if (methodDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
            //    {
            //        var returnType = methodDeclaration.ReturnType.ToFullString().Trim();

            //        var memberDescriptor = new MethodDescriptor(methodDeclaration.Identifier.ValueText,
            //            methodDeclaration.ParameterList.Parameters.ToFullString(),
            //            returnType,
            //            _filePath,
            //            ExtractLineNR(node));

            //        ExtractAPIDefinition(memberDescriptor, methodDeclaration);

            //        _typeDescriptor.Members.Add(memberDescriptor);
            //    }
            //    return;
            //}
            //else if (syntaxKind == SyntaxKind.PropertyDeclaration)
            //{
            //    var propertyDeclaration = ((PropertyDeclarationSyntax) node);

            //    if (propertyDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
            //    {
            //        var returnType = propertyDeclaration.Type.ToFullString().Trim();

            //        var memberDescriptor = new FieldDescriptor(propertyDeclaration.Identifier.ValueText,
            //            propertyDeclaration.Type.ToString());

            //        //returnType,
            //        //_filePath,
            //        //ExtractLineNR(node));

            //        ExtractAPIDefinition(memberDescriptor, propertyDeclaration);

            //        _typeDescriptor.Members.Add(memberDescriptor);
            //    }
            //    return;
            //}
            //else if (syntaxKind == SyntaxKind.FieldDeclaration)
            //{
            //    var fieldDeclaration = ((FieldDeclarationSyntax) node);

            //    if (fieldDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
            //    {
            //        //TODO: not very confident that this is always working
            //        var x = fieldDeclaration.Declaration.Variables.First().Identifier;

            //        var memberDescriptor = new FieldDescriptor(x.ValueText, fieldDeclaration.Declaration.Type.ToString());
            //        //fieldDeclaration.Declaration.Type.GetText().ToString().Trim());
            //        //"",
            //        //_filePath,
            //        //ExtractLineNR(node));

            //        ExtractAPIDefinition(memberDescriptor, fieldDeclaration);

            //        _typeDescriptor.Members.Add(memberDescriptor);
            //    }
            //    return;
            //}
            //else if (syntaxKind == SyntaxKind.InterfaceDeclaration)
            //{
            //    var interfaceDeclaration = ((InterfaceDeclarationSyntax) node);

            //    var visibility = GetVisibility(interfaceDeclaration.Modifiers);
            //    if (visibility == Visibility.Private)
            //        return;

            //    var name = interfaceDeclaration.Identifier.Text;
            //    if (string.IsNullOrEmpty(name))
            //    {
            //        return;
            //    }

            //    string comment = ExtractComment(interfaceDeclaration);

            //    _typeDescriptor = new TypeDescriptor(ElementType.Interface,
            //        name,
            //        visibility,
            //        _namespace,
            //        _filePath,
            //        ExtractLineNR(node),
            //        comment);

            //    _typeDescriptor.APIDefinition = APIDefinitionExtractor.Parse(comment);

            //    this._parsedTypes.Add(_typeDescriptor);
            //}
            //else if (syntaxKind == SyntaxKind.EnumDeclaration)
            //{
            //    var enumDeclaration = ((EnumDeclarationSyntax) node);

            //    var visibility = GetVisibility(enumDeclaration.Modifiers);
            //    if (visibility == Visibility.Private)
            //        return;

            //    var name = enumDeclaration.Identifier.Text;
            //    if (string.IsNullOrEmpty(name))
            //    {
            //        return;
            //    }

            //    //if (enumDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
            //    {
            //        string comment = ExtractComment(enumDeclaration) ?? string.Empty;
            //        _typeDescriptor = new TypeDescriptor(ElementType.Enum,
            //            name,
            //            visibility,
            //            _namespace,
            //            this._filePath,
            //            ExtractLineNR(node), comment);

            //        _typeDescriptor.APIDefinition = APIDefinitionExtractor.Parse(comment);

            //        this._parsedTypes.Add(_typeDescriptor);
            //    }
            //    return;
            //}
            //else if (syntaxKind == SyntaxKind.StructDeclaration)
            //{
            //    var structDeclaration = ((StructDeclarationSyntax) node);

            //    var visibility = GetVisibility(structDeclaration.Modifiers);
            //    if (visibility == Visibility.Private)
            //        return;

            //    var name = structDeclaration.Identifier.Text;
            //    if (string.IsNullOrEmpty(name))
            //    {
            //        return;
            //    }

            //    //if (structDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
            //    {
            //        string comment = ExtractComment(structDeclaration) ?? string.Empty;
            //        _typeDescriptor = new TypeDescriptor(ElementType.Struct,
            //            name,
            //            visibility,
            //            _namespace,
            //            this._filePath,
            //            ExtractLineNR(node),
            //            comment);

            //        _typeDescriptor.APIDefinition = APIDefinitionExtractor.Parse(comment);

            //        this._parsedTypes.Add(_typeDescriptor);
            //    }
            //    return;
            //}

            base.Visit(node);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            if (_ignoreTesterNamespaces)
            {
                _namespace = node.Name.ToFullString().PrepareNamespace();
                if (_namespace.IndexOf("test", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    // ignore namespaces that contain 'test' token
                    return;
                }
            }
            base.VisitNamespaceDeclaration(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var visibility = GetVisibility(node.Modifiers);
            if (visibility != Visibility.Public)
            {
                return;
            }
            _isClassContext = true;

            var @class = node.Identifier.ValueText.Trim();

            if (string.IsNullOrEmpty(@class))
            {
                // ignore
                return;
            }

            string comment = ExtractComment(node) ?? String.Empty;

            _typeDescriptor = new TypeDescriptor(ElementType.Class, @class, visibility,
                _namespace, _filePath, ExtractLineNR(node), comment);

            _typeDescriptor.APIDefinition = APIDefinitionExtractor.Parse(comment);

            _parsedTypes.Add(_typeDescriptor);

            base.VisitClassDeclaration(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            var visibility = GetVisibility(node.Modifiers);
            if (visibility == Visibility.Private)
                return;

            _isClassContext = false;

            var name = node.Identifier.Text;
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            string comment = ExtractComment(node);

            _typeDescriptor = new TypeDescriptor(ElementType.Interface,
                name,
                visibility,
                _namespace,
                _filePath,
                ExtractLineNR(node),
                comment);

            _typeDescriptor.APIDefinition = APIDefinitionExtractor.Parse(comment);

            this._parsedTypes.Add(_typeDescriptor);

            base.VisitInterfaceDeclaration(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            var visibility = GetVisibility(node.Modifiers);
            if (visibility == Visibility.Private)
                return;

            var name = node.Identifier.Text;
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            _isClassContext = false;

            //if (structDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                string comment = ExtractComment(node) ?? string.Empty;
                _typeDescriptor = new TypeDescriptor(ElementType.Struct,
                    name,
                    visibility,
                    _namespace,
                    this._filePath,
                    ExtractLineNR(node),
                    comment);

                _typeDescriptor.APIDefinition = APIDefinitionExtractor.Parse(comment);

                this._parsedTypes.Add(_typeDescriptor);
            }
            
            base.VisitStructDeclaration(node);
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            var visibility = GetVisibility(node.Modifiers);
            if (visibility == Visibility.Private)
                return;

            var name = node.Identifier.Text;
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            //if (enumDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                string comment = ExtractComment(node) ?? string.Empty;
                _typeDescriptor = new TypeDescriptor(ElementType.Enum,
                    name,
                    visibility,
                    _namespace,
                    this._filePath,
                    ExtractLineNR(node), comment);

                _typeDescriptor.APIDefinition = APIDefinitionExtractor.Parse(comment);

                this._parsedTypes.Add(_typeDescriptor);
            }
            base.VisitEnumDeclaration(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (!_isClassContext || node.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                var returnType = node.ReturnType.ToFullString().Trim();

                var memberDescriptor = new MethodDescriptor(node.Identifier.ValueText,
                    node.ParameterList.Parameters.ToFullString(),
                    returnType,
                    _filePath,
                    ExtractLineNR(node));

                ExtractAPIDefinition(memberDescriptor, node);

                _typeDescriptor.Members.Add(memberDescriptor);
            }

            base.VisitMethodDeclaration(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            if (!_isClassContext || node.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                var returnType = node.Type.ToFullString().Trim();

                var memberDescriptor = new PropertyDescriptor(node.Identifier.ValueText,
                    node.Type.ToString());

                //returnType,
                //_filePath,
                //ExtractLineNR(node));

                ExtractAPIDefinition(memberDescriptor, node);

                _typeDescriptor.Members.Add(memberDescriptor);
            }
            base.VisitPropertyDeclaration(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            if (node.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                //TODO: not very confident that this is always working
                var x = node.Declaration.Variables.First().Identifier;

                var memberDescriptor = new FieldDescriptor(x.ValueText, node.Declaration.Type.ToString());
                //fieldDeclaration.Declaration.Type.GetText().ToString().Trim());
                //"",
                //_filePath,
                //ExtractLineNR(node));

                ExtractAPIDefinition(memberDescriptor, node);

                _typeDescriptor.Members.Add(memberDescriptor);
            }

            base.VisitFieldDeclaration(node);
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
            var name = node.Name.ToString();

            switch (name)
            {
                case "ServiceContract":
                    {
                        _typeDescriptor.WCFContract = node.ArgumentList?.Arguments.ToString();
                        return;
                    }
                case "Export":
                    {
                        _typeDescriptor.MEFDefinition = MEF.Export;
                        return;
                    }
                case "Import":
                    {
                        _typeDescriptor.MEFDefinition = MEF.Import;
                        return;
                    }
            }
            base.VisitAttribute(node);
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            if (!_isClassContext || node.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                var name = node.Declaration.Variables.FirstOrDefault();

                var memberDescriptor = new EventDescriptor(name.Identifier.ValueText, node.Declaration.Type.ToString());

                //returnType,
                //_filePath,
                //ExtractLineNR(node));

                ExtractAPIDefinition(memberDescriptor, node);

                _typeDescriptor.Members.Add(memberDescriptor);
            }
            base.VisitEventFieldDeclaration(node);
        }

        private static Visibility GetVisibility(SyntaxTokenList list)
        {
            if (list.Any(SyntaxKind.PublicKeyword))
                return Visibility.Public;

            if (list.Any(SyntaxKind.InternalKeyword))
                return Visibility.Internal;

            return Visibility.Private;
        }

        private static void ExtractAPIDefinition(IAPIElement element, CSharpSyntaxNode methodDeclaration)
        {
            string comment = ExtractComment(methodDeclaration);
            element.APIDefinition = APIDefinitionExtractor.Parse(comment);
        }

        public static string ExtractComment(CSharpSyntaxNode classDeclaration)
        {
            if (classDeclaration.HasLeadingTrivia)
            {
                string xml = null;
                var trivia = classDeclaration.GetLeadingTrivia();
                foreach (var t in trivia)
                {
                    //Console.WriteLine("{0}\r\n\t{1}", t.Kind().ToString(), ""); //t.GetStructure().ToString()
                    if (t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    {
                        xml = t.GetStructure().ToString();
                        return CommentCleaner.PrepareSinglelineComment(xml);
                    }
                    if (t.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
                    {
                        xml = t.GetStructure().ToString();
                        return CommentCleaner.PrepareMultilineComment(xml);
                    }
                }
            }
            return string.Empty;
        }

        private static int ExtractLineNR(SyntaxNode node)
        {
            return node.GetLocation().GetLineSpan().StartLinePosition.Line;
        }
    }
}