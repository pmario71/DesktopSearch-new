using DesktopSearch.Core.DataModel.Code;
using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;

namespace DesktopSearch.Core.Lucene
{
    static class DocumentExtensions
    {
        public static string[] EmptyArray = new string[0];

        public static TypeDescriptor ToTypeDescriptor(this Document doc)
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            return new TypeDescriptor(
                (ElementType)doc.GetField("elementtype").NumericValue,
                doc.GetField("name")?.StringValue,
                (Visibility)doc.GetField("visibility").NumericValue,
                doc.GetField("namespace").StringValue,
                doc.GetField("filepath").StringValue,
                (int)doc.GetField("linenr").NumericValue,
                doc.GetField("comment")?.StringValue)
            {
                APIDefinition = (API)doc.GetField("apidefinition").NumericValue,
                MEFDefinition = (MEF)(doc.GetField("mef")?.NumericValue ?? MEF.None),
                WCFContract = doc.GetField("wcfcontract")?.StringValue
            };
        }

        public static Document FromTypeDescriptor(this TypeDescriptor descriptor, string documentCollectionName=null)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            var doc = new Document
                    {
                        new TextField("name", descriptor.Name, Field.Store.YES),
                        new TextField("namespace", descriptor.Namespace, Field.Store.YES),
                        new TextField("filepath", descriptor.FilePath, Field.Store.YES),
                        new IntField("elementtype", (int)descriptor.ElementType, Field.Store.YES),
                        new IntField("visibility", (int)descriptor.Visibility, Field.Store.YES),
                        new IntField("linenr", descriptor.LineNr, Field.Store.YES),
                        new TextField("comment", descriptor.Comment ?? string.Empty, Field.Store.YES),
                        new IntField("apidefinition", (int)descriptor.APIDefinition, Field.Store.YES),
                        new IntField("mef", (int)descriptor.MEFDefinition, Field.Store.YES),
                        new TextField("wcfcontract", descriptor.WCFContract ?? string.Empty, Field.Store.YES),
                    };

            if (!string.IsNullOrWhiteSpace(documentCollectionName))
            {
                doc.Fields.Add(new TextField("doccollection", documentCollectionName, Field.Store.NO));
            }

            return doc;
        }

        public static Document FromDocDescriptor(this DocDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            // ensure that there always a title set
            if (descriptor.Title == null)
                descriptor.Title = Path.GetFileNameWithoutExtension(descriptor.Path);

            var doc = new Document
            {
                new TextField("title", descriptor.Title, Field.Store.YES),
                new TextField("keywords", (descriptor.Keywords != null) ? string.Join(";", descriptor.Keywords) : string.Empty, Field.Store.YES),
                new TextField("filepath", descriptor.Path, Field.Store.YES),
                new IntField("rating", (int)descriptor.Rating, Field.Store.YES),
                new TextField("content", descriptor.Content ?? string.Empty, Field.Store.NO),
                new TextField("lastmodified", DateTools.DateToString(descriptor.LastModified, DateTools.Resolution.SECOND), Field.Store.YES),
                new TextField("contenttype", descriptor.ContentType, Field.Store.YES),
            };
            if (!string.IsNullOrEmpty(descriptor.Author))
            {
                doc.Add(new TextField("author", descriptor.Author, Field.Store.YES));
            }

            return doc;
        }

        public static DocDescriptor ToDocDescriptor(this Document doc)
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            var descriptor = new DocDescriptor();
            descriptor.Title = doc.GetField("title")?.StringValue;

            var keywords = doc.GetField("keywords")?.StringValue;
            
            string[] keywordsArray = EmptyArray;
            if (!string.IsNullOrEmpty(keywords))
            {
                keywordsArray = keywords.Split(new[] { ';' });
            }
            descriptor.Keywords = keywordsArray;

            descriptor.Path = doc.GetField("filepath").StringValue;
            descriptor.Rating = (int)doc.GetField("rating").NumericValue;

            var docField = doc.GetField("author")?.StringValue;
            descriptor.Author = (docField ?? string.Empty);
            descriptor.LastModified = (DateTime)DateTools.StringToDate(doc.GetField("lastmodified").StringValue);
            descriptor.ContentType = doc.GetField("contenttype")?.StringValue;

            return descriptor;
        }

        public static string GetTypeID(this TypeDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            return $"{descriptor.Namespace}.{descriptor.Name}";
        }

    }
}
