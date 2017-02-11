using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace DesktopSearch.PS.Composition
{
    class BlacklistComparer
    {
        private List<string> _collection = new List<string>();
        private List<string> _wildcardCollection = new List<string>();

        private IComparer<string> _comparer = StringComparer.OrdinalIgnoreCase;

        public BlacklistComparer(IEnumerable<string> collection)
        {
            //Contract.Requires(collection != null);
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            foreach (var item in collection)
            {
                if (item.EndsWith("*"))
                {
                    _wildcardCollection.Add(item.Substring(0, item.Length - 1));
                }
                else if (item.Contains("*"))
                {
                    throw new ArgumentException($"Wildcard * can only be at the end of the string: {item}");
                }
                _collection.Add(item);
            }
        }

        public bool Contains(string value)
        {
            var fullyQualifiedMatches = _collection.Any(e => _comparer.Compare(e, value) == 0);
            return fullyQualifiedMatches || _wildcardCollection.Any(e => value.StartsWith(e, StringComparison.OrdinalIgnoreCase));
        }

        //public bool Contains(string value, IEnumerable<string> collection)
        //{
        //    return collection.Any(e => _comparer.Compare(e,value) == 0);
        //}
    }
}