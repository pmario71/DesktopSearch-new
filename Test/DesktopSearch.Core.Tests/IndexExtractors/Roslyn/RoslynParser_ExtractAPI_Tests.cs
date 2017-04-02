using System;
using System.IO;
using System.Linq;
using DesktopSearch.Core.DataModel.Code;
using DesktopSearch.Core.Extractors.Roslyn;
using NUnit.Framework;

namespace CodeSearchTests.Indexing.Roslyn
{
    [TestFixture]
    public class RoslynParser_ExtractAPI_Tests
    {
        [Test]
        public void Classes_are_parsed()
        {
            var path = "D:\\Projects\\Prototyping\\SYBRW_ImageCallup\\Source\\syngo.RemoteServices.Browsing.ServerLocatorRSC\\IServerLocatorRSC.cs";

            string csharp = @"   namespace HelloWorld.Something 
                                       { 
                                           // comment to be stripped
                                           [Test]
                                           public class TestClass
                                           { 
                                               int not=0;
                                               public int YES=0;

                                               // comment to be stripped
                                               [Test]
                                               public void PublicMethod(string inParam)
                                               {
                                                  // to be removed
                                                  int i =0;
                                               }
                                               // comment to be stripped
                                               [Test]
                                               private void PreivateMethod()
                                               {
                                                // to be removed
                                                int i =0;
                                               }

                                               public string PublicProperty { get; set; }
                                               internal string PublicProperty { get; set; }
                                           } 
                                       }";

            csharp = File.ReadAllText(path);

            var parser = new RoslynParser();
            var text = parser.ExtractAPI(csharp, path);

            Console.WriteLine(text);
        }
    }
}