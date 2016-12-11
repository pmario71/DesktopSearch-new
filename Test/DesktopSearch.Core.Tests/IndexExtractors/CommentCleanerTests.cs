using DesktopSearch.Core.Extractors.Roslyn;
using NUnit.Framework;

namespace CodeSearchTests.Indexing
{
    [TestFixture]
    public class CommentCleanerTests
    {
        [Test]
        public void PrepareSinglelineComment_on_single_comment_line()
        {
            const string input = @"/// <summary>This is an xml doc comment</summary>";
            var result = CommentCleaner.PrepareSinglelineComment(input);

            Assert.AreEqual("<summary>This is an xml doc comment</summary>", result);
        }

        [Test]
        public void PrepareSinglelineComment_on_multiple_comment_lines()
        {
            const string input = @"   /// <summary>First line.
   /// second line</summary>  ";
            var result = CommentCleaner.PrepareSinglelineComment(input);

            Assert.AreEqual("<summary>First line.\r\nsecond line</summary>", result);
        }

        [Test]
        public void PrepareMultilineComment_on_single_comment_line()
        {
            const string input = @"/* <summary>This is an xml doc comment</summary> */";
            var result = CommentCleaner.PrepareMultilineComment(input);

            Assert.AreEqual("<summary>This is an xml doc comment</summary>", result);
        }

        [Test]
        public void PrepareMultilineComment_on_multiple_comment_lines()
        {
            const string input = @"/* <summary>First line. 
 some other line.</summary> */";
            var result = CommentCleaner.PrepareMultilineComment(input);

            Assert.AreEqual("<summary>First line.\r\nsome other line.</summary>", result);
        }
    }
}