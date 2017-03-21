using DesktopSearch.Core.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Tests.Utils
{
    class CfgMocks
    {
        public static IConfigAccess<TikaConfig> GetTikaConfigMock()
        {
            var tikaMock = new Mock<IConfigAccess<TikaConfig>>();
            tikaMock.Setup(m => m.Get()).Returns(new TikaConfig());

            return tikaMock.Object;
        }

        public static IConfigAccess<LuceneConfig> GetLuceneConfigMock()
        {
            var luceneMock = new Mock<IConfigAccess<LuceneConfig>>();
            luceneMock.Setup(m => m.Get()).Returns(new LuceneConfig());

            return luceneMock.Object;
        }
    }
}
