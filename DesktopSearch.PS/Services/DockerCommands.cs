using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DesktopSearch.PS.Services
{
    public class DockerCommands
    {
        private const string tikaImageName = "docker-tikaserver_mp";
        private static string TikaContainerID = "ad5389485586";

        #region Dependencies
        #endregion

        public static async Task EnsureTikaStarted()
        {
            DockerClient client = GetClient();

            var runningContainers = await client.Containers.ListContainersAsync(new ContainersListParameters() { All = true, });
            var runningTikaContainers = runningContainers.Where(r => (r.Image == tikaImageName && r.State == "running"));

            if (runningTikaContainers.Any())
            {
                return;
            }

            string containerID = runningContainers.FirstOrDefault(r => (r.Image == tikaImageName))?.ID;

            if (containerID == null)
            {
                var p = GetCreateParamsWithPorts("9998", "9998");
                p.Image = tikaImageName;

                var response = await client.Containers.CreateContainerAsync(p);
                containerID = response.ID;
            }

            var res = await client.Containers.StartContainerAsync(containerID, new ContainerStartParameters(){});

            //"run -d -p 9998:9998 docker-tikaserver_mp"
        }

        public static async Task StopTika()
        {
            DockerClient client = GetClient();

            var runningContainers = await client.Containers.ListContainersAsync(new ContainersListParameters() { All = true, });
            var result = runningContainers.Where(r => (r.Image == tikaImageName && r.State == "running"));

            var containers = new List<Task<bool>>();
            foreach (var instance in result)
            {
                containers.Add(client.Containers.StopContainerAsync(instance.ID, new ContainerStopParameters(), CancellationToken.None));
            }
            var whenAll = await Task.WhenAll(containers);
            if (whenAll.Any(e => e == false))
            {
                throw new Exception("Failed to stop at least one container!");
            }
        }

        public static async Task CleanupTikaContainers()
        {
            DockerClient client = GetClient();

            var runningContainers = await client.Containers.ListContainersAsync(new ContainersListParameters() { All = true, });
            var result = runningContainers.Where(r => (r.Image == tikaImageName && r.State == "exited"));

            var containers = new List<Task>();
            foreach (var instance in result)
            {
                containers.Add(client.Containers.RemoveContainerAsync(instance.ID, new ContainerRemoveParameters()));
            }
            
            await Task.WhenAll(containers);
        }

        private static DockerClient GetClient()
        {
            return new DockerClientConfiguration(new Uri("tcp://127.0.0.1:2375")).CreateClient();
        }

        /// <summary>
        /// returns CreateParams where Port mapping is filled
        /// </summary>
        /// <param name="containerPort"></param>
        /// <param name="hostPort"></param>
        /// <returns></returns>
        private static CreateContainerParameters GetCreateParamsWithPorts(string containerPort, string hostPort)
        {
            CreateContainerParameters createParameters = new CreateContainerParameters
            {
                // Image, Name, Entrypoint, ...
                ExposedPorts = new Dictionary<string, object>
                {
                    {
                        containerPort, new
                        {
                            HostPort = hostPort
                        }
                    }
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        {
                            containerPort, new List<PortBinding>
                            {
                                new PortBinding
                                {
                                    HostPort = hostPort
                                }
                            }
                        }
                    }
                }
            };
            return createParameters;
        }
    }
}
