using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSearch.Core.Utils;
using NUnit.Framework;

namespace DesktopSearch.Core.Tests.Utils
{
    public class TextUtilTests
    {
        [TestCase("The quick brown fox", 1, ExpectedResult = false)]
        [TestCase("The quick brown fox", 4, ExpectedResult = true)]
        [TestCase("The quick brown fox", 0, ExpectedResult = true)]
        public bool IsWordStart(string text, int pos)
        {
            return TextUtil.IsWordStart(text, pos);
        }

        [TestCase("The quick brown fox", 1, ExpectedResult = 4)]
        [TestCase("The quick brown fox", 4, ExpectedResult = 4)]
        [TestCase("The quick brown fox", 0, ExpectedResult = 0)]
        public int SkipToNextWord(string text, int pos)
        {
            Assert.True(TextUtil.SkipToNextWord(text, ref pos));

            Console.WriteLine($"Extracted: {text.Substring(pos).Replace(' ', '_')}");

            return pos;
        }

        [Test]
        public void ExtractText()
        {
            string text = "Building the backbone systems for a cross-team effort at Microsoft with several hundred " +
                          "engineers and an investment volume the size of the Windows Azure Platform is at or " +
                          "beyond the complexity level of many mission-critical Enterprise systems. Running such " +
                          "a system and upgrading or changing parts of such a system in flight and without downtime " +
                          "is not only complex, it’s an art form. You need loose coupling between subsystems, " +
                          "you need a lot of flexibility and extensibility, and you need to have a clear notion of " +
                          "what that other system is going to accept and return in terms of messages. What I keep " +
                          "finding is that once you confront a “simpler” communications model with real-world " +
                          "requirements of the sort we’ve got on the Windows Azure backbone, you almost inevitably " +
                          "end up reinventing the wheel at the protocol level and you increasingly make the life of implementers harder.";

            var extractText = TextUtil.ExtractText(text, 10);
            Console.WriteLine(extractText);

            Assert.AreEqual(10, extractText.Split(' ').Length);

        }
    }
}
