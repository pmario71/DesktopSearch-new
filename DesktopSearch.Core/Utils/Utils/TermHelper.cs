using Lucene.Net.Index;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Lucene.Utils
{
    internal class TermHelper
    {
        public static Term FromInt(int val)
        {
            BytesRef bytes = new BytesRef(NumericUtils.BUF_SIZE_INT);
            NumericUtils.IntToPrefixCoded(val, 0, bytes);
            return new Term("elementtype", bytes);
        }
    }
}
