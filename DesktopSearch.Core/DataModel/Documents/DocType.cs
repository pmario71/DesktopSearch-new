using System;
using System.Collections.Generic;
using System.IO;

namespace DesktopSearch.Core.DataModel.Documents
{

    public sealed class DocType
    {
        private readonly List<Folder> _folders;

        private DocType()
        {
             _folders = new List<Folder>();
        }

        public static DocType Create(string name, string rootFolder)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name");

            if (Uri.CheckHostName(name) == UriHostNameType.Unknown)
                throw new ArgumentException($"name may not contain whitespaces or special characters: '{name}'");

            var dt = new DocType();
            dt.Name = name;

            var folder = Folder.Create(rootFolder);
            dt._folders.Add(folder);

            return dt;
        }

        public string Name { get; private set; }

        /// <summary>
        /// Optional: If specified, DesktopSearch knows to mount container before opening
        /// </summary>
        public string TrueCryptContainerPath { get; set; }

        /// <summary>
        /// machine specific storage locations
        /// </summary>
        public IReadOnlyCollection<Folder> Folders { get { return _folders; } }

        /// <summary>
        /// File extensions to be ignored when indexing
        /// </summary>
        public string[] ExcludedExtensions { get; set; }

        /// <summary>
        /// File extensions to be included when indexing
        /// </summary>
        public string[] IncludedExtensions { get; set; }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            DocType f = obj as DocType;
            if ((f != null ) && 
                (StringComparer.CurrentCultureIgnoreCase.Compare(this.Name, f.Name) == 0))
            {
                return true;
            }
            return false;
        }
    }
}
