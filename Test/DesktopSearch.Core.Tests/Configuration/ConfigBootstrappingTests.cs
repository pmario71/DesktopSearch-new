using DesktopSearch.Core.Configuration;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Tests.Configuration
{
    [TestFixture]
    public class ConfigBootstrappingTests
    {

        [Test]
        public void Values_can_be_overridden()
        {
            const string key = "Test";
            const string value = "Overridden";

            var sut = ConfigBootstrapping.GetDefault();
            IEnumerable<KeyValuePair<string, string>> initial = new[]
            {
                new KeyValuePair<string,string>(key,value)
            };

            sut.AddInMemoryCollection(initial);

            var cfg = sut.Build();

            Assert.AreEqual(value, cfg[key]);
        }

        [Test]
        public void Values_can_be_bound()
        {
            const string key = "Test";
            const string value = "Overridden";

            var sut = ConfigBootstrapping.GetDefault();
            var initial = new Dictionary<string, string>
            {
                { "Option1", "some value" },
                { "Option2", "42" }
            };

            sut.AddInMemoryCollection(initial);

            var cfg = sut.Build();

            var opt = new MyOptions();
            cfg.Bind(opt);

            Assert.AreEqual("some value", opt.Option1);
            Assert.AreEqual(42, opt.Option2);
        }
    }

    public class MyOptions
    {
        public MyOptions()
        {
            // Set default value.
            Option1 = "value1_from_ctor";
        }
        public string Option1 { get; set; }

        public int Option2 { get; set; } = 5;
    }
}
