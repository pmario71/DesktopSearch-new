using DesktopSearch.Core.DataModel.Code;
using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Lucene
{
    class DocumentConverter
    {

        public static Document From(TypeDescriptor descriptor)
        {
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

        public static string GetTypeID(TypeDescriptor descriptor)
        {
            return $"{descriptor.Namespace}.{descriptor.Name}";
        }

    }
}
