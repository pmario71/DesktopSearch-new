using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DesktopSearch.Core.FileSystem
{
    /// <summary>
    /// Returns true, in case a file's extension is on the extension white list.
    /// </summary>
    public class IncludeFileByExtensionFilter
    {
        private readonly HashSet<string> _map = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                                                              {
                                                                  ".cs",
                                                              };

        public IncludeFileByExtensionFilter(params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");

            if (extensions.Any(e => (e[0] != '.')))
                throw new ArgumentException("All extensions need to start with period (e.g. '.cs)'", nameof(extensions));

            _map = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);
        }

        public bool FilterByExtension(string arg)
        {
            var ext = Path.GetExtension(arg);
            if (string.IsNullOrEmpty(ext))
                return false;

            if (_map.Contains(ext))
            {
                return true;
            }
            return false;
        }

        public IEnumerable<string> FilterByExtension(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                if (FilterByExtension(file))
                {
                    yield return file;
                }
            }
        }
    }
}