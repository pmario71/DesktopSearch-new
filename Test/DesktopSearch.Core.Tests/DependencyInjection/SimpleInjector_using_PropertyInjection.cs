using DesktopSearch.Core.DependencyInjection;
using NUnit.Framework;
using SimpleInjector;
using SimpleInjector.Advanced;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Tests.DependencyInjection
{

    [TestFixture]
    public class SimpleInjector_using_PropertyInjection
    {

        [Test]
        public void Properties_can_be_injected_into_existing_instance()
        {
            var cont =  new SimpleInjector.Container();
            cont.Options.PropertySelectionBehavior = new ImportPropertySelectionBehavior();

            cont.Register<InjectedService>(Lifestyle.Singleton);

            var sut = new TestService();
            cont.RegisterSingleton(sut);
            cont.Verify();

            Assert.IsNotNull(sut.Svc);
            Assert.IsNull(sut.Blubb);
        }

        [Test]
        public void DesireProperties_can_be_injected_into_existing_instancedResult_without_reregistering()
        {
            var sut = new TestService();

            var cont = new SimpleInjector.Container();
            cont.Options.PropertySelectionBehavior = new ImportPropertySelectionBehavior();
            cont.Register<InjectedService>(Lifestyle.Singleton);

            cont.Verify();

            InstanceProducer producer = cont.GetRegistration(sut.GetType(), throwOnFailure: true);
            Registration registration = producer.Registration;
            registration.InitializeInstance(sut);

            Assert.IsNotNull(sut.Svc);
            Assert.IsNull(sut.Blubb);
        }
    }

    class TestService
    {
        [Import]
        public InjectedService Svc { get; set; }

        public object Blubb { get; set; }

    }

    class InjectedService
    {
    }
}
