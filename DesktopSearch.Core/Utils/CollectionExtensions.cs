using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Utils
{
    public static class CollectionExtensions
    {

        public static IEnumerable<T> FilterBySequence<T>(this IEnumerable<T> sequence, IEnumerable<T> compareSeq)
            where T : IEquatable<T>
        {
            foreach (var item in sequence)
            {
                if (!compareSeq.Contains(item))
                {
                    yield return item;
                }
            }
        }

    }
}
