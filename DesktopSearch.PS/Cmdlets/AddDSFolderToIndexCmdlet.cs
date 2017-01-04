using DesktopSearch.Core.FileSystem;
using DesktopSearch.Core.Configuration;
using DesktopSearch.PS.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PowershellExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.DataModel.Documents;

namespace DesktopSearch.PS
{
    [Cmdlet(VerbsCommon.Add, "DSFolderToIndex", DefaultParameterSetName = "All")]
    public class AddDSFolderToIndexCmdlet : PSCmdlet
    {
        private Settings _settings;
        private List<IFolder> _folders;

        [Parameter(Mandatory = true, HelpMessage = "Paths of folders to add.")]
        [ValidatePath()]
        public string[] Path { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "Defines which index extractor is used for the given folder(s).")]
        public string IndexingType { get; set; }

        #region Dependencies
        [Import]
        internal ConfigAccess ConfigAccess { set; get; }
        #endregion

        protected override void BeginProcessing()
        {
            this.Compose();

            throw new NotImplementedException();
            //try
            //{
            //    _settings = ConfigAccess.Get();
            //    _folders = new List<IFolder>(_settings.FoldersToIndex.Folders ?? new List<IFolder>());
            //}
            //catch(Exception ex)
            //{
            //    WriteWarning($"Recovering from error loading configuration: {ex.Message}");
            //    _settings = new Settings()
            //    {
            //        FoldersToIndex = new FoldersToIndex()
            //    };
            //    _folders = new List<Folder>();
            //}
        }

        protected override void ProcessRecord()
        {
            //TODO: not possible without access to DocTypeRepo anymore!  Refactor
            throw new NotImplementedException("Needs to be refactored");
            //foreach (var p in Path)
            //{
            //    var folder = new Folder
            //    {
            //        Path = p,
            //        IndexingType = this.IndexingType
            //    };
            //    _folders.Add(folder);
            //    WriteVerbose($"Added folder: {folder.Path}\r\n   Type [{folder.IndexingType}]");
            //}
        }

        protected override void EndProcessing()
        {
            _settings.FoldersToIndex.Folders = _folders.ToList();
            ConfigAccess.SaveChanges(_settings);
        }
    }
}
