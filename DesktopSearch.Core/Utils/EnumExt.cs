using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Utils
{
    public static class EnumExt
    {
        public static string ToString(this Enum e)
        {
            return Enum.GetName(e.GetType(), e);
        }

    }
}
