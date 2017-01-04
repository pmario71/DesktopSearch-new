using Nest;
using Newtonsoft.Json;
using System;
using System.IO;

namespace DesktopSearch.Core.DataModel.Documents
{

    /// <summary>
    /// Refers to a machine specific storage location.
    /// </summary>
    [ElasticsearchType(Name = "folder", IdProperty = "Path")]
    public sealed class Folder : IFolder
    {
        public Folder()
        {
        }

        internal static Folder Create(string path, string indexingType=null)
        {
            if (!System.IO.Path.IsPathRooted(path))
                throw new ArgumentException("Path not rooted!", "path");

            if (!Directory.Exists(path))
                throw new ArgumentException($"Path does not exist: {path}", "path");

            var f = new Folder();
            if (path[path.Length-1] != '\\')
            {
                f.Path = $"{path}\\";
            }
            else
            {
                f.Path = path;
            }
            f.Machinename = Environment.MachineName;

            return f;
        }

        [Keyword(Ignore = true)]
        public string Path { get; internal set; }

        /// <summary>To be removed!!!!!!
        /// Defines which type of indexing to apply to the folder.
        /// </summary>
        [Keyword(Ignore = true)]
        public string IndexingType { get; set; }

        [Keyword(Ignore =true)]
        public string Machinename { get; internal set; }

        public override int GetHashCode()
        {
            return this.Path.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Folder f = obj as Folder;
            if (f != null && this.Path.GetHashCode() == f.Path.GetHashCode())
            {
                return true;
            }
            return false;
        }
    }
}
