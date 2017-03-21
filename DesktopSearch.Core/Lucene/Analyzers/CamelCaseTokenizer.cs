using System.IO;
using Lucene.Net.Util;

namespace DesktopSearch.Core.Lucene.Analyzers
{
    public sealed class CamelCaseTokenizer : ConsecutiveCharTokenizer
    {
        public CamelCaseTokenizer(LuceneVersion matchVersion, TextReader @in) : base(matchVersion, @in)
        {
        }

        public CamelCaseTokenizer(LuceneVersion matchVersion, AttributeSource.AttributeFactory factory, TextReader @in) : base(matchVersion, factory, @in)
        {
        }

        protected override bool IsLastTokenChar(int c, int next)
        {
            return char.IsUpper((char)next);
        }

        protected override int Normalize(int c)
        {
            return char.ToLower((char)c);
        }
    }
}