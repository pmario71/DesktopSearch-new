using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Util;

namespace DesktopSearch.Core.Lucene.Analyzers
{
    internal class CamelCaseAnalyzer : Analyzer
    {
        public override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            var source = new CamelCaseTokenizer(LuceneVersion.LUCENE_48, reader);

            return new TokenStreamComponents(source, source);
        }
    }
}