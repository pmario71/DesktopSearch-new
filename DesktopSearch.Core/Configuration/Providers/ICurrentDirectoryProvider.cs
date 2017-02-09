using System;

namespace DesktopSearch.Core.Configuration
{
    public interface ICurrentDirectoryProvider
    {
        string GetCurrentDirectory();
    }

    internal class DefaultDirectoryProvider : ICurrentDirectoryProvider
    {
        public string GetCurrentDirectory()
        {
            return Environment.CurrentDirectory;
        }
    }
}