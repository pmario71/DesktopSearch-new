using System;
using System.IO;
using DesktopSearch.Core.DataModel.Code;
using DesktopSearch.Core.DataModel.Documents;
using DesktopSearch.Core.Lucene;
using KellermanSoftware.CompareNetObjects;
using Lucene.Net.Documents;
using NUnit.Framework;

namespace DesktopSearch.Core.Tests.Lucene
{
    [TestFixture()]
    public class DocumentExtensions_TypeDescriptor_Tests
    {
        [Test]
        public void TypeDescriptorToDocumentConversion()
        {
            //Arrange
            TypeDescriptor typeDescriptor = GetFilledDocDescriptor();

            //Act
            var document = DocumentExtensions.FromTypeDescriptor(typeDescriptor);
            var descriptor = DocumentExtensions.ToTypeDescriptor(document);

            // Assert
            var comparisonConfig = new ComparisonConfig();
            //comparisonConfig.MembersToIgnore.Add("Content");

            var cl = new CompareLogic(comparisonConfig);
            var compareResult = cl.Compare(typeDescriptor, descriptor);

            if (!compareResult.AreEqual)
            {
                Assert.Fail(compareResult.DifferencesString);
            }
        }

        [Test]
        public void Ensure_that_comment_is_optional()
        {
            //Arrange
            TypeDescriptor typeDescriptor = GetFilledDocDescriptor();
            typeDescriptor.Comment = null;

            //Act
            var document = DocumentExtensions.FromTypeDescriptor(typeDescriptor);
            var descriptor = DocumentExtensions.ToTypeDescriptor(document);

            //Assert
            Assert.AreEqual(string.Empty, descriptor.Comment);
        }

        private static TypeDescriptor GetFilledDocDescriptor()
        {
            return new TypeDescriptor(ElementType.Class, 
                                      "CamelCasedClass", 
                                      Visibility.Public, 
                                      "Test.Namespace", 
                                      Path.GetTempFileName(), 
                                      666, 
                                      "Was mine mammon since whom favour the land who later was sister " +
                                      "despair despair artless massy though the pangs peace");
        }
    }
}