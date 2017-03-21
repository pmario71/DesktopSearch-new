using SimpleInjector.Advanced;
using System;
using System.Linq;
using System.ComponentModel.Composition;
using System.Reflection;

namespace DesktopSearch.Core.DependencyInjection
{
    /// <summary>
    /// Finds properties marked with [Import] attribute
    /// </summary>
    class ImportPropertySelectionBehavior : IPropertySelectionBehavior
    {
        public bool SelectProperty(Type type, PropertyInfo prop)
        {
            return prop.GetCustomAttributes(typeof(ImportAttribute)).Any();
        }
    }
}
