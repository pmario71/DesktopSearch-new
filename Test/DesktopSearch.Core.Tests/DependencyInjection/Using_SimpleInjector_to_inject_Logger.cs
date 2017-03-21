using DesktopSearch.Core.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Tests.DependencyInjection
{

    [TestFixture]
    public class Using_SimpleInjector_to_inject_Logger
    {

        [Test]
        public void Injecting_Logger_Implementation()
        {
            var cont = new SimpleInjector.Container();
            cont.Options.PropertySelectionBehavior = new ImportPropertySelectionBehavior();
            //cont.ResolveUnregisteredType += Cont_ResolveUnregisteredType;
            cont.Register(typeof(ILogger<>), typeof(Logger<>), Lifestyle.Singleton);

            cont.Register<ClassWhichUsesLogging>(Lifestyle.Singleton);

            cont.Verify();

            var sut = cont.GetInstance<ClassWhichUsesLogging>();

        }

        [Test]
        public void Instaniate_generic_type()
        {
            var unregisteredType = typeof(ILogger<ClassWhichUsesLogging>);

            var d1 = typeof(Logger<>);
            Type[] typeArgs = { unregisteredType.GetGenericArguments()[0] };
            var makeme = d1.MakeGenericType(typeArgs);

            var instance = Activator.CreateInstance(makeme);

            Console.WriteLine(instance.GetType().Name);

            Assert.IsNotNull(instance);
        }

        private void Cont_ResolveUnregisteredType(object sender, UnregisteredTypeEventArgs e)
        {
            Type t = e.UnregisteredServiceType;


            if (t.GetGenericTypeDefinition() == typeof(ILogger<>))
            {
                var a = t.GenericTypeArguments[0];

                var d1 = typeof(Logger<>);
                Type[] typeArgs = { t.GetGenericArguments()[0] };
                var makeme = d1.MakeGenericType(typeArgs);

                e.Register(() => Activator.CreateInstance( makeme ));
            }
        }

        public class Logger<T> : ILogger<T>
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                Console.WriteLine($"{Enum.GetName(typeof(LogLevel), logLevel)} - Logged successfully!");
            }
        }

        public class ClassWhichUsesLogging
        {
            public ClassWhichUsesLogging(ILogger<ClassWhichUsesLogging> logger)
            {
                logger.Log<string>(LogLevel.Information, new EventId(1), "some text", null, null);
            }
        }

    }
}
