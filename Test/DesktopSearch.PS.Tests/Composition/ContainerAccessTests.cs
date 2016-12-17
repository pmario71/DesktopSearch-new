using DesktopSearch.PS.Composition;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.PS.Tests.Composition
{

    [TestFixture]
    public class ContainerAccessTests
    {
        [Test]
        public void GetService_from_internal_container()
        {
            var conventions = new RegistrationBuilder();
            var cat0 = new AssemblyCatalog(typeof(MyClass).Assembly);
            var container = new CompositionContainer(cat0);

            var sut = new ContainerAccess(container);

            var result = sut.GetService(typeof(MyClass));

            container.Dispose();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<MyClass>(result);
        }

        [Test]
        public void GetService_for_generic_interface()
        {
            using (CurrentDirectoryContext.SetToWorkfinDirectory())
            {
                CompositionContainer container = null;

                try
                {
                    var conventions = new RegistrationBuilder();
                    var cat0 = new AssemblyCatalog(typeof(PowerShellLogger<>).Assembly);
                    container = new CompositionContainer(cat0);

                    object result = container.GetExportedValue<Microsoft.Extensions.Logging.ILogger<ContainerAccessTests>>();

                    var sut = new ContainerAccess(container);

                    result = sut.GetService(typeof(Microsoft.Extensions.Logging.ILogger<ContainerAccessTests>));

                }
                finally
                {
                    container.Dispose();
                }
            }
        }

        [Test]
        public void Check_if_all_serivces_setup_in_Host_are_composable()
        {
            using(CurrentDirectoryContext.SetToWorkfinDirectory())
            {
                var sut = new Host();

                var tester = new HostTest();
                tester.TryCreate(sut.Container);
            }
        }

        class CurrentDirectoryContext : IDisposable
        {
            readonly string _previousDirectory;

            public CurrentDirectoryContext()
            {
                _previousDirectory = Environment.CurrentDirectory;
                Environment.CurrentDirectory = TestContext.CurrentContext.WorkDirectory;
            }

            public static IDisposable SetToWorkfinDirectory()
            {
                var x = new CurrentDirectoryContext();
                return x;
            }

            public void Dispose()
            {
                if (!string.IsNullOrEmpty(_previousDirectory))
                {
                    Environment.CurrentDirectory = _previousDirectory;
                }                
            }
        }
    }

    [Export]
    class MyClass
    {

    }
}
