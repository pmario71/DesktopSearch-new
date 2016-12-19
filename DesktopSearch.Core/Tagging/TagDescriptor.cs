using DesktopSearch.Core.Contracts;
using DesktopSearch.Core.Extractors.Tika;
using System.Collections.Generic;

namespace DesktopSearch.Core.Tagging
{
    public class TagDescriptor
    {
        private readonly Dictionary<string, string> _tags;
        private readonly List<string> _keywords;

        public TagDescriptor(Dictionary<string, string> tags)
        {
            this._tags = tags;
            _keywords = new List<string>(KeywordParser.Parse(SafeGet(MetaTypes.Keywords)));
        }

        public string Title
        {
            get { return SafeGet(MetaTypes.Title); }
            set { _tags[MetaTypes.Title] = value; }
        }

        public string Author
        {
            get { return SafeGet(MetaTypes.Author); }
            set { _tags[MetaTypes.Author] = value; }
        }

        public ICollection<string> Keywords
        {
            get { return _keywords; }
            //set { _tags[MetaTypes.Keywords] = string.Join(";", value); }
        }

        public IDictionary<string, string> Tags
        {
            get
            {
                // update keywords
                _tags[MetaTypes.Keywords] = string.Join(";", _keywords);
                return _tags;
            }
        }

        private string SafeGet(string name)
        {
            string result;
            if (!_tags.TryGetValue(name, out result))
            {
                result = string.Empty;
            }
            return result;
        }
    }
}