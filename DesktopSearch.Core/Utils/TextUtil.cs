using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Utils
{
    internal class TextUtil
    {
        public static string ExtractText(string docContent, int nrOfWords=20)
        {
            int pos = docContent.Length / 3;
            
            if (!SkipToNextWord(docContent, ref pos))
            {
                throw new Exception("Failed to find next word boundary!");
            }
            int start = pos;
            int end = pos + 1;

            for (int i = 0; i < nrOfWords; i++)
            {
                if (!SkipToNextWord(docContent, ref end))
                {
                    throw new Exception($"Failed to extract {nrOfWords} words!");
                }
                //Console.WriteLine($"({end}) - {docContent.Substring(end, 10)}");
                end++;
            }
            return docContent.Substring(start, end-start-2);
        }

        public static bool SkipToNextWord(string docContent, ref int pos)
        {
            if (pos == 0)
            {
                return true;
            }

            var docContentLength = docContent.Length;
            
            for (int i = pos; i < docContentLength; i++)
            {
                if (IsWordStart(docContent, i))
                {
                    pos = i;
                    return true;
                }
            }
            return false;
        }

        public static bool IsWordStart(string docContent, int pos)
        {
            if (pos == 0)
            {
                return true;
            }
            return char.IsWhiteSpace(docContent[pos-1]) && !char.IsWhiteSpace(docContent[pos]);
        }
    }
}
