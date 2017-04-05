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

        public virtual MemberType Type { get { return MemberType.Field; } }
    }

    public class PropertyDescriptor : FieldDescriptor
    {
        public PropertyDescriptor(string name, string propertyType) : base(name,propertyType)
        {
        }

        public override MemberType Type { get { return MemberType.Property; } }

        public override string ToString()
        {
            return string.Format("{0} {1}(property)", this.FieldType, this.Name);
        }
    }

    public class EventDescriptor : IDescriptor, IAPIElement
    {
        public EventDescriptor(string name, string eventType)
        {
            Name = name;
            EventType = eventType;
        }

        public string Name { get; set; }

        public string EventType { get; set; }

        public int LineNr { get; set; }

        public API APIDefinition { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}(field)", this.EventType, this.Name);
        }

        public MemberType Type { get { return MemberType.Event; } }
    }
}