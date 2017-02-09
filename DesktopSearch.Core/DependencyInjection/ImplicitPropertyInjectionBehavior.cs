using SimpleInjector;
using SimpleInjector.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.DependencyInjection
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    using SimpleInjector.Advanced;

    /// <summary>
    /// <see cref="https://github.com/simpleinjector/SimpleInjector/blob/43b21b0c1a02c838e570f3bf8a050f89b95a1dec/src/SimpleInjector.CodeSamples/ImplicitPropertyInjectionExtensions.cs"/>
    /// </summary>
    public static class ImplicitPropertyInjectionExtensions
    {
        [DebuggerStepThrough]
        public static void AutoWirePropertiesImplicitly(this ContainerOptions options)
        {
            options.PropertySelectionBehavior = new ImplicitPropertyInjectionBehavior(
                options.PropertySelectionBehavior, options);
        }

        internal sealed class ImplicitPropertyInjectionBehavior
            : IPropertySelectionBehavior
        {
            private readonly IPropertySelectionBehavior core;
            private readonly ContainerOptions options;

            internal ImplicitPropertyInjectionBehavior(IPropertySelectionBehavior core,
                ContainerOptions options)
            {
                this.core = core;
                this.options = options;
            }

            [DebuggerStepThrough]
            public bool SelectProperty(Type type, PropertyInfo property)
            {
                return this.IsImplicitInjectable(property) ||
                    this.core.SelectProperty(type, property);
            }

            [DebuggerStepThrough]
            private bool IsImplicitInjectable(PropertyInfo property)
            {
                return IsInjectableProperty(property) &&
                    this.IsAvailableService(property.PropertyType);
            }

            [DebuggerStepThrough]
            private static bool IsInjectableProperty(PropertyInfo property)
            {
                MethodInfo setMethod = property.GetSetMethod(nonPublic: false);

                return setMethod != null && !setMethod.IsStatic && property.CanWrite;
            }

            [DebuggerStepThrough]
            private bool IsAvailableService(Type serviceType)
            {
                return this.options.Container.GetRegistration(serviceType) != null;
            }
        }
    }

    //public class ImplicitPropertyInjectionBehavior : IPropertySelectionBehavior
    //{
    //    private readonly Container container;
    //    internal ImplicitPropertyInjectionBehavior(Container container)
    //    {
    //        this.container = container;
    //    }

    //    public bool SelectProperty(Type type, PropertyInfo property)
    //    {
    //        return this.IsImplicitInjectable(property);
    //    }

    //    private bool IsImplicitInjectable(PropertyInfo property)
    //    {
    //        return IsInjectableProperty(property) && this.IsRegistered(property);
    //    }

    //    private static bool IsInjectableProperty(PropertyInfo prop)
    //    {
    //        MethodInfo setMethod = prop.GetSetMethod(nonPublic: false);
    //        return setMethod != null && !setMethod.IsStatic && prop.CanWrite;
    //    }

    //    private bool IsRegistered(PropertyInfo property)
    //    {
    //        return this.container.GetRegistration(property.PropertyType) != null;
    //    }
    //}

    public class GreediestConstructorBehavior : IConstructorResolutionBehavior
    {
        public ConstructorInfo GetConstructor(Type serviceType, Type implementationType)
        {
            return (
                from ctor in implementationType.GetConstructors()
                orderby ctor.GetParameters().Length descending
                select ctor)
                .First();
        }
    }
}
