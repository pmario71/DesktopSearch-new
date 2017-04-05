using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DesktopSearch.Core.DataModel;
using DesktopSearch.Core.DataModel.Code;

namespace DesktopSearch.Core.Extractors.Roslyn
{
    /// <summary>
    /// Public parser for c# files.
    /// </summary>
    public class RoslynParser
    {
        private bool _ignoreTesterNamespaces = true;

        public bool IgnoreTesterNamespaces
        {
            get { return this._ignoreTesterNamespaces; }
            set { _ignoreTesterNamespaces = value; }
        }

        public IEnumerable<TypeDescriptor> ExtractTypes(IEnumerable<string> filePaths)
        {
            var collection = new List<TypeDescriptor>();

            //var filteredPaths = (_ignoreTesterNamespaces) ?
            //                        filePaths.Where(s => s.IndexOf("test", StringComparison.InvariantCultureIgnoreCase) < 0) :
            //                        filePaths;


            var result = Parallel.ForEach(filePaths, p =>
            {
                SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(p));
                var root = (CompilationUnitSyntax)tree.GetRoot();

                var walker = new APIWalker(p, _ignoreTesterNamespaces);
                walker.Visit(root);
                lock (collection)
                {
                    collection.AddRange(walker.ParsedTypes);
                }
            });
            return collection;
        }

        public IEnumerable<TypeDescriptor> ExtractTypes(string csharp, string filePath = "<none>")
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(csharp);

            var root = (CompilationUnitSyntax)tree.GetRoot();

            var walker = new APIWalker(filePath, _ignoreTesterNamespaces);
            walker.Visit(root);

            return walker.ParsedTypes;
        }

        public string ExtractAPI(string csharp, string filename)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(csharp);

            var root = (CompilationUnitSyntax) tree.GetRoot();

            //var extractor = new StructureExtractor(filename);
            var extractor = new APIWalker(filename, false);
            extractor.Visit(root);

            var sb = new StringBuilder();
            foreach (var type in extractor.ParsedTypes)
            {
                string rootName = string.Empty;
                //if (type.ElementType == ElementType.Class ||
                //    type.ElementType == ElementType.Interface ||
                //    type.ElementType == ElementType.Struct ||
                //    type.ElementType == ElementType.Enum)
                {
                    rootName = type.Name;
                    sb.AppendLine($"{type.FilePath}({type.LineNr}): {type.ElementType} - {type.Name}");

                    foreach (var member in type.Members)
                    {
                        var method = member as MethodDescriptor;
                        if (method != null)
                        {
                            sb.AppendLine($"{type.FilePath}({type.LineNr}):                   - {rootName}.{method.MethodName}({method.Parameters}) : {method.ReturnType}");
                        }
                        var field = member as FieldDescriptor;
                        if (field != null)
                        {
                            sb.AppendLine($"{type.FilePath}({type.LineNr}):                   - {rootName}.{field.Name} : {field.Type}");
                        }
                    }
                }
            }
            return sb.ToString();

            //var syntaxNodes = modified.DescendantNodes().Where(n => (n is TypeDeclarationSyntax));
            //return extractor.StructureText;


            //var sb = new StringBuilder();

            //using (StreamWriter tw = new StreamWriter(new MemoryStream()))
            //{
            //    foreach (var node in syntaxNodes)
            //    {
            //        sb.AppendLine(node.ToString());
            //    }
            //}


        }
    }
}