using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DesktopSearch.Core.DataModel.Documents
{
    [ElasticsearchType(Name = "doctype", IdProperty = "Name")]
    public sealed class DocType : IDocType
    {

        private readonly List<Folder> _folders;

        public DocType()
        {
             _folders = new List<Folder>();
        }

        public static IDocType Create(string name, string rootFolder)
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

        [Keyword]
        public string Name { get; private set; }

        /// <summary>
        /// Optional: If specified, DesktopSearch knows to mount container before opening
        /// </summary>
        public string TrueCryptContainerPath { get; set; }

        /// <summary>
        /// machine specific storage locations
        /// </summary>
        [JsonIgnore]
        IReadOnlyCollection<IFolder> IDocType.Folders { get { return _folders; } }

        [JsonProperty]
        List<Folder> Folders { get { return _folders; } }

        /// <summary>
        /// File extensions to be ignored when indexing
        /// </summary>
        [Keyword(Ignore =true)]
        public string[] ExcludedExtensions { get; set; }

        /// <summary>
        /// File extensions to be included when indexing
        /// </summary>
        [Keyword(Ignore = true)]
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

    public interface IFolder
    {
        string IndexingType { get; set; }

        string Path { get; }
    }

    public interface IDocType
    {
        /// <summary>
        /// Unique name for the DocType
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Folders contributing to the <see cref="IDocType"/>
        /// </summary>
        IReadOnlyCollection<IFolder> Folders { get; }

        /// <summary>
        /// File extensions to be excluded when indexing
        /// </summary>
        string[] ExcludedExtensions { get; set; }

        /// <summary>
        /// File extensions to be included when indexing
        /// </summary>
        string[] IncludedExtensions { get; set; }
    }
}
