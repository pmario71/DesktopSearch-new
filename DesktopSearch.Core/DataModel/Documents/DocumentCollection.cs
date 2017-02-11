using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DesktopSearch.Core.DataModel.Documents
{
    [ElasticsearchType(Name = "documentcollection", IdProperty = "Name")]
    public sealed class DocumentCollection : IDocumentCollection
    {
        [JsonIgnore]
        private readonly List<Folder> _folders;

        public DocumentCollection()
        {
             _folders = new List<Folder>();
        }

        public static IDocumentCollection Create(string name)
        {
            return Create(name, IndexingStrategy.Documents);
        }
        public static IDocumentCollection Create(string name, IndexingStrategy indexingStrategy)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name");

            if (Uri.CheckHostName(name) == UriHostNameType.Unknown)
                throw new ArgumentException($"name may not contain whitespaces or special characters: '{name}'");

            var dt = new DocumentCollection();
            dt.Name = name;
            dt.IndexingStrategy = indexingStrategy;

            //if (rootFolder != null)
            //{
            //    var folder = Folder.Create(rootFolder);
            //    folder.DocumentCollection = dt;
            //    dt._folders.Add(folder);
            //}

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
        IReadOnlyCollection<IFolder> IDocumentCollection.Folders { get { return _folders; } }

        [JsonProperty]
        internal List<Folder> Folders { get { return _folders; } }

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

        public IndexingStrategy IndexingStrategy
        {
            get; set;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            DocumentCollection f = obj as DocumentCollection;
            if ((f != null ) && 
                (StringComparer.CurrentCultureIgnoreCase.Compare(this.Name, f.Name) == 0))
            {
                return true;
            }
            return false;
        }

        public void AddFolder(Folder folder)
        {
            folder.DocumentCollection = this;
            _folders.Add(folder);
        }
    }

    public interface IFolder
    {
        string Path { get; }

        IDocumentCollection DocumentCollection { get; }
    }

    public interface IDocumentCollection
    {
        /// <summary>
        /// Unique name for the <see cref="IDocumentCollection"/>
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Folders contributing to the <see cref="IDocumentCollection"/>
        /// </summary>
        IReadOnlyCollection<IFolder> Folders { get; }

        IndexingStrategy IndexingStrategy { get; }

        /// <summary>
        /// File extensions to be excluded when indexing
        /// </summary>
        string[] ExcludedExtensions { get; set; }

        /// <summary>
        /// File extensions to be included when indexing
        /// </summary>
        string[] IncludedExtensions { get; set; }

        void AddFolder(Folder folder);
    }

    public enum IndexingStrategy
    {
        Documents = 0,
        Code = 1
    }
}
