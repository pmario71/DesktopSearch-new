using DesktopSearch.Core.Contracts;
using DesktopSearch.Core.Tagging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Tests.Tagging
{
    [TestFixture]
    public class TagDescriptorTests
    {
        

        [Test]
        public void TypedProperties_are_filled_correctly()
        {
            const string titleString = "The Title";
            const string authorString = "The Author";
            const string keywordsString = "SQL;Test";


            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add(MetaTypes.Title, titleString);
            map.Add(MetaTypes.Author, authorString);
            map.Add(MetaTypes.Keywords, keywordsString);

            var tagDesc = new TagDescriptor(map);

            Assert.AreEqual(titleString, tagDesc.Title);
            Assert.AreEqual(authorString, tagDesc.Author);
            Assert.AreEqual(keywordsString, string.Join(";", tagDesc.Keywords));
        }

        [Test]
        public void UnTyped_Tags_property_contains()
        {
            const string somePropertyName = "SomeProperty";
            const string somePropertyValue = "theValue";


            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add(somePropertyName, somePropertyValue);

            var tagDesc = new TagDescriptor(map);

            Assert.AreEqual(somePropertyValue, tagDesc.Tags[somePropertyName]);
        }

        [Test]
        public void Typed_Tags_do_not_throw_when_empty()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            var tagDesc = new TagDescriptor(map);

            Assert.AreEqual(string.Empty, tagDesc.Title);
            Assert.AreEqual(string.Empty, tagDesc.Author);
            Assert.AreEqual(0, tagDesc.Keywords.Count);
        }

        [Test]
        public void Check_that_Kewords_in_Tags_collection_are_updated()
        {
            const string keywordsString = "SQL;Test";


            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add(MetaTypes.Keywords, keywordsString);

            var tagDesc = new TagDescriptor(map);

            tagDesc.Keywords.Remove("Test");
            tagDesc.Keywords.Add( "NewTest");
            tagDesc.Keywords.Add("xxx");

            Assert.AreEqual("SQL;NewTest;xxx", string.Join(";", tagDesc.Tags[MetaTypes.Keywords]));
        }
    }
}
