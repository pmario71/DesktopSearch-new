using SimpleInjector;
using System;

namespace DesktopSearch.Core.DependencyInjection
{

    internal class ContainerWrapper : IContainer, IContainerInitializer
    {
        private Container _container;

        public ContainerWrapper(Container container)
        {
            this._container = container;
        }

        public object GetService(Type type)
        {
            return _container.GetInstance(type);
        }

        public TInstance GetService<TInstance>()
            where TInstance : class
        {
            return _container.GetInstance<TInstance>();
        }

        public IFace GetService<IFace, TInstance>()
            where IFace : class
        {
            return _container.GetInstance<IFace>();
        }

        public void Register(Type service, Type implementation, bool isSingleton)
        {
            Lifestyle ls = (isSingleton) ? Lifestyle.Singleton : Lifestyle.Transient;
            _container.Register(service, implementation, ls);
        }

        public void Register<TService, TImplementation>(bool isSingleton)
            where TService : class
            where TImplementation : class, TService
        {
            Lifestyle ls = (isSingleton) ? Lifestyle.Singleton : Lifestyle.Transient;

            _container.Register<TService, TImplementation>(ls);
        }
    }

    public interface IContainerInitializer
    {
        void Register(Type type1, Type type2, bool isSingleton = true);

        void Register<TService, TImplementation>(bool isSingleton=true)
            where TService : class
            where TImplementation : class, TService;
    }
}
