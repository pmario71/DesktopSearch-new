using Newtonsoft.Json;

namespace DesktopSearch.Core.Configuration
{
    public class DocumentIndexing
    {
        private string _fileExtensionToIgnore;

        /// <summary>
        /// Semicolon-separated list of file extension that are ignored when parsing (e.g. '.db;.lnk;...')
        /// </summary>
        [JsonProperty]
        public string FileExtensionToIgnore
        {
            get
            {
                return _fileExtensionToIgnore ?? ".bin;.lnk;.db;.obj;.pdp";
            }
            set { _fileExtensionToIgnore = value; }
        }
    }
}