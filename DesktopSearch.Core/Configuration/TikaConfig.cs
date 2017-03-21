using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Configuration
{
    public class TikaConfig
    {
        public TikaConfig()
        {
            this.Uri = "http://localhost:9998/";
        }

        public string Uri { get; set; }
    }
}
