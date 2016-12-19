using DesktopSearch.Core.Contracts;
using DesktopSearch.Core.Tagging;
using DesktopSearch.PS.UI;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.PS.Tests.UI
{
    [TestFixture]
    public class TaggingDialogTests
    {

        [Test, Explicit, Apartment(System.Threading.ApartmentState.STA)]
        public void InteractiveTest()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add(MetaTypes.Title, "The Title");
            map.Add(MetaTypes.Author, "The Author");
            map.Add(MetaTypes.Keywords, "SQL;Test");

            var tagDesc = new TagDescriptor(map);

            var sut = new TaggingDialog();
            sut.TagDescriptor = tagDesc;

            DialogFactory.ShowDialog(sut);

            foreach (var tag in tagDesc.Tags)
            {
                Console.WriteLine($"{tag.Key}  -  {tag.Value}");
            }
        }
    }
}
