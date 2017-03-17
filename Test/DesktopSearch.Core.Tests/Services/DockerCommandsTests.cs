using System;
using System.Threading.Tasks;
using DesktopSearch.Core.Services;
using NUnit.Framework;

namespace DesktopSearch.Core.Tests.Services
{
    [TestFixture()]
    public class DockerCommandsTests_Integration
    {

        [Test, Explicit("Exercises starting and stopping Tika")]
        public async Task EnsureTikaStarted()
        {
            var sut = new DockerService();
            Console.WriteLine("Cleaning up");
            await sut.CleanupTikaContainers();

            Console.WriteLine("starting Tika");
            await sut.EnsureTikaStarted();

            Console.WriteLine("stopping Tika");
            await sut.StopTika();

            Console.WriteLine("starting again");
            await sut.EnsureTikaStarted();

            Console.WriteLine("stopping Tika");
            await sut.StopTika();

            Console.WriteLine("Cleaning up");
            await sut.CleanupTikaContainers();
        }
    }
}
