using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.Tests.ElasticSearch;
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

        public static IConfigAccess<ElasticSearchConfig> GetElasticSearchConfigMock()
        {
            var elasticMock = new Mock<IConfigAccess<ElasticSearchConfig>>();
            elasticMock.Setup(m => m.Get()).Returns(ElasticTestClientFactory.Config);

            return elasticMock.Object;
        }

        public static IConfigAccess<TikaConfig> GetTikaConfigMock()
        {
            var tikaMock = new Mock<IConfigAccess<TikaConfig>>();
            tikaMock.Setup(m => m.Get()).Returns(new TikaConfig());

            return tikaMock.Object;
        }
    }
}
