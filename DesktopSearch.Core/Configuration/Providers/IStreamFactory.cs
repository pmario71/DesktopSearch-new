using System;
using System.IO;

namespace DesktopSearch.Core.Configuration
{
    /// <summary>
    /// Extension interface to allow testing <see cref="ConfigAccess"/> without filesystem.
    /// </summary>
    public interface IStreamFactory
    {
        Stream GetWritableStream(string id);
        Stream GetReadableStream(string id);
    }

    public class FileStreamFactory : IStreamFactory
    {
        private const string _settingsFile = ".json";
        private readonly string _directory;

        public FileStreamFactory(ICurrentDirectoryProvider currentDirectory)
        {
            _directory = currentDirectory.GetCurrentDirectory(); //Directory.GetCurrentDirectory();
        }

        public Stream GetReadableStream(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            return new FileStream(Path.Combine(_directory, $"{id}.json"), FileMode.OpenOrCreate, FileAccess.Read);
        }

        public Stream GetWritableStream(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            return new FileStream(Path.Combine(_directory, $"{id}.json"), FileMode.Truncate, FileAccess.Write);
        }
    }

    public class MemoryStreamEx : MemoryStream
    {
        public MemoryStreamEx() : base() {}

        public MemoryStreamEx(byte[] content) : base(content) { }

        /// <summary>
        /// Do not dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
        }
    }
}