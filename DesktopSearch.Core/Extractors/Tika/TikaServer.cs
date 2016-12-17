using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DesktopSearch.Core.Extractors.Tika
{
    public class TikaServer : ITikaServer
    {
        private const string processArguments = "-jar {0}";
        private readonly string _tikaServerJar;
        private static Process _server;
        private static readonly object _lock = new object();
        private bool _stopping;

        public TikaServer(bool start = false)
        {
            var info = new DirectoryInfo(".\\TikaServer");
            var fileInfos = info.GetFiles("tika-server*.jar");

            if (fileInfos.Length != 1)
            {
                const string msg = "Failed to identify tika-server-x.x.jar! Installation error!";
                //_log.Error(msg);
                throw new Exception(msg);
            }
            _tikaServerJar = fileInfos.First().FullName;

            if (start)
            {
                this.Start();
            }
        }

        public TikaServer() : this(true)
        {
        }

        public void Start()
        {
            if (IsRunning)
                return;

            lock (_lock)
            {
                var fileName = string.Format(processArguments, _tikaServerJar);
                const string javaExe = "java.exe";

                // kill running orphaned services
                var processes = Process.GetProcessesByName(javaExe).Where(p => p.StartInfo.Arguments == fileName);
                foreach (var process in processes)
                    process.Kill();

                // start a new tika server
                var psi = new ProcessStartInfo(javaExe, fileName)
                {
                    UseShellExecute = false,
                    //WindowStyle = ProcessWindowStyle.Minimized
                    CreateNoWindow = true,
                    RedirectStandardError = true
                };

                _server = new Process
                {
                    StartInfo = psi,
                    EnableRaisingEvents = true
                };
                var result = _server.Start();
                if (!result)
                {
                    //_log.Error(_server.StandardError.ReadToEnd());
                    throw new Exception("Failed to start tika-server! Check logs for details!");
                }
                var stackTrace = new System.Diagnostics.StackTrace();
                Console.WriteLine("Started TikaServer: {0}", _server.Id);

                //TODO: track from where server is started when running tests
                //Console.WriteLine(stackTrace);

                _server.Exited += RestartServer;
                //_log.InfoFormat("Tika server started id({0}) ...", _server.Id);
            }
        }

        public void Stop()
        {
            if (!IsRunning)
                return;

            lock (_lock)
            {
                _stopping = true;
                _server.Exited -= RestartServer;
                _server.Kill();
                if (!_server.WaitForExit(500))
                {
                    //_log.Error("Tika server failed to stop");
                }
                //_log.Info("Tika server stopped");
                _server = null;
                _stopping = false;
            }
        }

        public bool IsRunning
        {
            get
            {
                lock (_lock)
                {
                    return _server != null && !_server.HasExited;
                }
            }
        }

        private void RestartServer(object sender, EventArgs eventArgs)
        {
            if (_stopping)
                return;

            var sb = new StringBuilder();
            sb.AppendLine("tika-server exited unexpectedly and will be restarted!");
            sb.AppendLine(_server.StandardError.ReadToEnd());
            //_log.Warn(sb.ToString());
            Start();
        }
    }
}
