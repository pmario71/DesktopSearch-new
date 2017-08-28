using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.PS.Cmdlets;
using DesktopSearch.PS.Services;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace DesktopSearch.PS.Tests.Services
{
    [TestFixture(), Ignore("Tika not started using Docker containers anymore!")]
    public class DockerCommandsTests
    {

        [Test]
        public async Task EnsureTikaStarted()
        {
            await DockerCommands.EnsureTikaStarted();
        }

        [Test]
        public async Task StopTika()
        {
            await DockerCommands.StopTika();
        }

        [Test]
        public async Task CleanupStoppedTikaInstances()
        {
            await DockerCommands.CleanupTikaContainers();
        }
        

    }
}
