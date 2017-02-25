using System;
using System.IO;
using Newtonsoft.Json;

namespace DesktopSearch.Core.Configuration
{
    public class Tools
    {
        [JsonProperty]
        private string _luke;

        [JsonProperty]
        private string _vsCode;

        /// <summary>
        /// Path to Luke installation.
        /// </summary>
        public string Luke
        {
            get { return _luke; }
            set
            {
                if (!File.Exists(value))
                {
                    throw new ArgumentException($"File does not exist: {value}!");
                }
                if (Path.GetExtension(value).ToLower() != ".jar" || !Path.GetFileNameWithoutExtension(value).ToLower().Contains("luke"))
                {
                    throw new ArgumentException($"'{Path.GetFileName(value)}' does not seem to be a valid Luke version!");
                }
                _luke = value;
            }
        }

        /// <summary>
        /// Path to VS Code installation
        /// </summary>
        public string VSCode
        {
            get { return _vsCode; }
            set
            {
                if (!File.Exists(value))
                {
                    throw new ArgumentException($"File does not exist: {value}!");
                }
                if (Path.GetFileName(value).ToLower() != "code.cmd")
                {
                    throw new ArgumentException($"'{Path.GetFileName(value)}' does not seem to be a valid VS Code version!");
                }
                _vsCode = value;
            }
        }
    }
}