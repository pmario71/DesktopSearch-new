using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Extractors.Tika
{
    public class KeywordParser
    {
        internal static string[] Parse(string keywordStream)
        {
            char[] separators = new[] { ';', ',' };
            var tokens = keywordStream.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return tokens.Select(t => t.Trim()).ToArray();
        }
    }
}
