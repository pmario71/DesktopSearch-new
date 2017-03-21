using DesktopSearch.Core.DataModel;
using System;

namespace DesktopSearch.Core.DataModel.Code
{
    public class FieldDescriptor : IDescriptor, IAPIElement
    {
        public FieldDescriptor(string name, string fieldType)
        {
            Name = name;
            FieldType = fieldType;
        }

        public string Name { get; set; }

        public string FieldType { get; set; }

        public int LineNr { get; set; }

        public API APIDefinition { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}(field)", this.FieldType, this.Name);
        }

        public MemberType Type { get { return MemberType.Field; } }
    }
}