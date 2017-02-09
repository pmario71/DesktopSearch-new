using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Configuration
{
    public class CodeSearch
    {
        public const string IndexName = "codesearch";


        public CodeSearch()
        {
            this.Folders = new List<string>();
        }


        public ICollection<string> Folders { get; private set; }
    }
}
