using DesktopSearch.Core.DataModel.Code;
using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Lucene
{
    static class DocumentExtensions
    {
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
                APIDefinition = (API)doc.GetField("apidefinition").NumericValue
            };
        }

        public static Document From(this TypeDescriptor descriptor)
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
                    };

            return doc;
        }

        public static string GetTypeID(this TypeDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            return $"{descriptor.Namespace}.{descriptor.Name}";
        }

    }
}
