using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.DataModel.Documents;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;
using NUnit.Framework;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DesktopSearch.Core.Configuration.ConfigAccess;

namespace DesktopSearch.PS.Tests.Configuration
{
    [TestFixture]
    public class ConfigAccessTests
    {

        [Test]
        public void NullValueHandling_for_ElasticSearchURI_Test()
        {
            LuceneConfig settings = new LuceneConfig
            {
                IndexDirectory = "c:\\test"
            };

            TestFactory testFactory = new TestFactory();
            var sut = new ConfigAccess(testFactory);

            sut.SaveChanges(settings);

            var strm = testFactory.LastUsedStream;
            strm.Position = 0;

            var sr = new StreamReader(strm);
            var s = sr.ReadToEnd();

            // check that stream does not contain serialized 
            CollectionAssert.DoesNotContain(s, "localhost");

            var result = sut.Get();

            Assert.AreEqual(settings.IndexDirectory, result.IndexDirectory);
        }

        [Test]
        public void Return_default_configuration_if_no_config_file_is_found_Test()
        {
            var sut = new ConfigAccess(new TestFactory());

            var result = sut.Get();

            Assert.NotNull(result);
        }
    }

    [TestFixture]
    public class ConfigAccess_generics_Tests
    {

        [Test]
        public void Get_returns_object_when_no_config_was_found_Test()
        {
            var sut = new ConfigAccess<TestSettings>(new TestFactory());

            Assert.IsNotNull(sut.Get());
        }

        [Test]
        public void Save_config_Test()
        {
            var sut = new ConfigAccess<TestSettings>(new TestFactory());

            var cfg = sut.Get();
            cfg.SomeUri = "a short string";

            sut.Save(cfg);
        }

        [Test]
        public void Save_and_Get_Test()
        {
            var sut = new ConfigAccess<TestSettings>(new TestFactory());

            var cfg = sut.Get();
            cfg.SomeUri = "a short string";

            sut.Save(cfg);

            var cloned = sut.Get();

            var cl = new CompareLogic();
            var result = cl.Compare(cfg, cloned);

            if (!result.AreEqual)
            {
                Assert.Fail(result.DifferencesString);
            }
        }

        [Test]
        public void SimpleInjector_usage_Test()
        {
            var cont = new Container();
            cont.Register<IStreamFactory, TestFactory>();
            cont.Register(typeof(IConfigAccess<TestSettings>), typeof(ConfigAccess<TestSettings>));

            cont.Verify();

            var svc = cont.GetInstance<IConfigAccess<TestSettings>>();

            Assert.IsNotNull(svc);
        }
    }


    public class TestSettings
    {
        [JsonProperty]
        public string SomeUri { get; set; }
    }

    

    internal class TestFactory : IStreamFactory
    {
        private MemoryStream _lastUsedStream;
        private string _content;

        public TestFactory()
        {
        }
        //public TestFactory(string contentToInitialize)
        //{
        //    _content = contentToInitialize;            
        //}

        public MemoryStream LastUsedStream { get => _lastUsedStream; }

        public Stream GetReadableStream(string id)
        {
            if (_lastUsedStream != null)
            {
                _lastUsedStream.Position = 0;
                return _lastUsedStream;
            }
            if (_content != null)
                _lastUsedStream = new MemoryStreamEx(Encoding.Default.GetBytes(_content));
            else
                _lastUsedStream = new MemoryStreamEx();

            return _lastUsedStream;
        }

        public Stream GetWritableStream(string id)
        {
            _lastUsedStream = new MemoryStreamEx();
            return _lastUsedStream;
        }
    }
}
