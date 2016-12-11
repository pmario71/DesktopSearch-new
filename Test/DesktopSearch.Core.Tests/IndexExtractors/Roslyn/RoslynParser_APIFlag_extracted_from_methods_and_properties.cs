
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel;
using DesktopSearch.Core.Extractors.Roslyn;
using DesktopSearch.Core.DataModel.Code;
using NUnit.Framework;

namespace CodeSearchTests.Indexing.Roslyn
{
    public class RoslynParser_APIFlag_extracted_from_methods_and_properties
    {

        [Test]
        public void API_flag_extracted_from_comments()
        {
            const string csharp = @"   using System; using System.Collections.Generic; 
                                       using System.Text;
                                       namespace HelloWorld.Something 
                                       { 
                                           /// <summary>API:NO
                                           /// This is a summary description.
                                           /// </summary>
                                           public class Program 
                                           { 
                                               public string File 
                                               { 
                                                  get { return null; }
                                               } 
                                           } 
                                       }";

            var parser = new RoslynParser();
            var extractedTypes = parser.ExtractTypes(csharp);

            Assert.AreEqual(API.No, extractedTypes.First().APIDefinition);
        }

        [Test]
        public void API_flag_extracted_from_property()
        {
            const string csharp = @"   using System; using System.Collections.Generic; 
                                       using System.Text;
                                       namespace HelloWorld.Something 
                                       { 
                                           /// <summary>API:NO
                                           /// This is a summary description.
                                           /// </summary>
                                           public class Program 
                                           { 
                                               /// <summary>API:YES</summary>
                                               public string File 
                                               { 
                                                  get { return null; }
                                               } 
                                           } 
                                       }";

            var parser = new RoslynParser();
            var extractedTypes = parser.ExtractTypes(csharp);

            Assert.AreEqual(API.Yes, extractedTypes.First().Members.First().APIDefinition);
        }

        [Test]
        public void API_flag_extracted_from_method()
        {
            const string csharp = @"   using System; using System.Collections.Generic; 
                                       using System.Text;
                                       namespace HelloWorld.Something 
                                       { 
                                           /// <summary>API:NO
                                           /// This is a summary description.
                                           /// </summary>
                                           public class Program 
                                           { 
                                               /// <summary>API:YES</summary>
                                               public void File()
                                               { 
                                               } 
                                           } 
                                       }";

            var parser = new RoslynParser();
            var extractedTypes = parser.ExtractTypes(csharp);

            Assert.AreEqual(API.Yes, extractedTypes.First().Members.First().APIDefinition);
        }

        [Test]
        public void API_flag_extracted_from_public_field()
        {
            const string csharp = @"   using System; using System.Collections.Generic; 
                                       using System.Text;
                                       namespace HelloWorld.Something 
                                       { 
                                           /// <summary>API:NO
                                           /// This is a summary description.
                                           /// </summary>
                                           public class Program 
                                           { 
                                               /// <summary>API:YES</summary>
                                               public string File = "";
                                           } 
                                       }";

            var parser = new RoslynParser();
            var extractedTypes = parser.ExtractTypes(csharp);

            Assert.AreEqual(API.Yes, extractedTypes.First().Members.First().APIDefinition);
        }
    }
}
