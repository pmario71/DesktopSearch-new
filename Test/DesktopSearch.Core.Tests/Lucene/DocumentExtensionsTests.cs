using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Lucene;
using KellermanSoftware.CompareNetObjects;
using Lucene.Net.Documents;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace DesktopSearch.Core.Tests.Lucene
{
    [TestFixture()]
    public class DocumentExtensionsTests
    {
        [Test]
        public void DocTypeToDocumentConversion()
        {
            //Arrange
            DocDescriptor docDescriptor = GetFilledDocDescriptor();

            //Act
            var document = DocumentExtensions.FromDocDescriptor(docDescriptor);
            var descriptor = DocumentExtensions.ToDocDescriptor(document);


            // Assert
            docDescriptor.LastModified = DateTools.StringToDate(DateTools.DateToString(docDescriptor.LastModified, DateTools.Resolution.SECOND));
            var comparisonConfig = new ComparisonConfig();
            comparisonConfig.MembersToIgnore.Add("Content");

            var cl = new CompareLogic(comparisonConfig);
            var compareResult = cl.Compare(docDescriptor, descriptor);

            if (!compareResult.AreEqual)
            {
                Assert.Fail(compareResult.DifferencesString);
            }
        }

        [Test]
        public void Ensure_that_keywords_are_optional()
        {
            //Arrange
            DocDescriptor docDescriptor = GetFilledDocDescriptor();
            docDescriptor.Keywords = null;

            //Act
            var document = DocumentExtensions.FromDocDescriptor(docDescriptor);
            var descriptor = DocumentExtensions.ToDocDescriptor(document);

            //Assert
            Assert.AreEqual(DocumentExtensions.EmptyArray, descriptor.Keywords);
        }

        private static DocDescriptor GetFilledDocDescriptor()
        {
            return new DocDescriptor
            {
                Title = "Ein Titel mit Ümlauten",
                Author = "Mario Plendl",
                Keywords = new[] { "Keyword1", "Keyword2" },
                Path = Path.GetTempFileName(),
                LastModified = DateTime.Now,
                Rating = 5,
                ContentType = "mime/text",
            };
        }
    }
}
