using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Analysis.Util;
using Lucene.Net.Support;
using Lucene.Net.Util;

namespace DesktopSearch.Core.Tests.Lucene
{
    /// <summary>
    /// An abstract base class for simple, character-oriented tokenizers. 
    /// <para>
    /// <a name="version">You must specify the required <seealso cref="LuceneVersion"/> compatibility
    /// when creating <seealso cref="ConsecutiveCharTokenizer"/>:
    /// <ul>
    /// <li>As of 3.1, <seealso cref="ConsecutiveCharTokenizer"/> uses an int based API to normalize and
    /// detect token codepoints. See <seealso cref="#isTokenChar(int)"/> and
    /// <seealso cref="#normalize(int)"/> for details.</li>
    /// </ul>
    /// </para>
    /// <para>
    /// A new <seealso cref="ConsecutiveCharTokenizer"/> API has been introduced with Lucene 3.1. This API
    /// moved from UTF-16 code units to UTF-32 codepoints to eventually add support
    /// for <a href=
    /// "http://java.sun.com/j2se/1.5.0/docs/api/java/lang/Character.html#supplementary"
    /// >supplementary characters</a>. The old <i>char</i> based API has been
    /// deprecated and should be replaced with the <i>int</i> based methods
    /// <seealso cref="#isTokenChar(int)"/> and <seealso cref="#normalize(int)"/>.
    /// </para>
    /// <para>
    /// As of Lucene 3.1 each <seealso cref="ConsecutiveCharTokenizer"/> - constructor expects a
    /// <seealso cref="LuceneVersion"/> argument. Based on the given <seealso cref="LuceneVersion"/> either the new
    /// API or a backwards compatibility layer is used at runtime. For
    /// <seealso cref="LuceneVersion"/> < 3.1 the backwards compatibility layer ensures correct
    /// behavior even for indexes build with previous versions of Lucene. If a
    /// <seealso cref="LuceneVersion"/> >= 3.1 is used <seealso cref="ConsecutiveCharTokenizer"/> requires the new API to
    /// be implemented by the instantiated class. Yet, the old <i>char</i> based API
    /// is not required anymore even if backwards compatibility must be preserved.
    /// <seealso cref="ConsecutiveCharTokenizer"/> subclasses implementing the new API are fully backwards
    /// compatible if instantiated with <seealso cref="LuceneVersion"/> < 3.1.
    /// </para>
    /// <para>
    /// <strong>Note:</strong> If you use a subclass of <seealso cref="ConsecutiveCharTokenizer"/> with <seealso cref="LuceneVersion"/> >=
    /// 3.1 on an index build with a version < 3.1, created tokens might not be
    /// compatible with the terms in your index.
    /// </para>
    /// 
    /// </summary>
    public abstract class ConsecutiveCharTokenizer : Tokenizer
    {
        /// <summary>
        /// Creates a new <seealso cref="ConsecutiveCharTokenizer"/> instance
        /// </summary>
        /// <param name="matchVersion">
        ///          Lucene version to match </param>
        /// <param name="input">
        ///          the input to split up into tokens </param>
        protected ConsecutiveCharTokenizer(LuceneVersion matchVersion, TextReader input)
            : base(input)
        {
            Init(matchVersion);
        }

        /// <summary>
        /// Creates a new <seealso cref="ConsecutiveCharTokenizer"/> instance
        /// </summary>
        /// <param name="matchVersion">
        ///          Lucene version to match </param>
        /// <param name="factory">
        ///          the attribute factory to use for this <seealso cref="Tokenizer"/> </param>
        /// <param name="input">
        ///          the input to split up into tokens </param>
        protected ConsecutiveCharTokenizer(LuceneVersion matchVersion, AttributeFactory factory, TextReader input)
            : base(factory, input)
        {
            Init(matchVersion);
        }

        /// <summary>
        /// LUCENENET Added in the .NET version to assist with setting the attributes
        /// from multiple constructors.
        /// </summary>
        /// <param name="matchVersion"></param>
        private void Init(LuceneVersion matchVersion)
        {
            _charUtils = CharacterUtils.GetInstance(matchVersion);
            _termAtt = AddAttribute<ICharTermAttribute>();
            _offsetAtt = AddAttribute<IOffsetAttribute>();
        }

        private int _offset = 0, _bufferIndex = 0, _dataLen = 0, _finalOffset = 0;
        private const int MAX_WORD_LEN = 255;
        private const int IO_BUFFER_SIZE = 4096;

        private ICharTermAttribute _termAtt;
        private IOffsetAttribute   _offsetAtt;

        private CharacterUtils     _charUtils;
        private readonly CharacterUtils.CharacterBuffer _ioBuffer = CharacterUtils.NewCharacterBuffer(IO_BUFFER_SIZE);

        /// <summary>
        /// Returns true if a codepoint should be included in a token. This tokenizer
        /// generates as tokens adjacent sequences of codepoints which satisfy this
        /// predicate. Codepoints for which this is false are used to define token
        /// boundaries and are not included in tokens.
        /// </summary>
        protected abstract bool IsLastTokenChar(int c, int next);

        /// <summary>
        /// Called on each token character to normalize it before it is added to the
        /// token. The default implementation does nothing. Subclasses may use this to,
        /// e.g., lowercase tokens.
        /// </summary>
        protected virtual int Normalize(int c)
        {
            return c;
        }

        public sealed override bool IncrementToken()
        {
            ClearAttributes();

            int length = 0;
            int start = -1; // this variable is always initialized
            int end = -1;
            char[] buffer = _termAtt.Buffer();
            while (true)
            {
                //Console.WriteLine($"l-{length}  s-{start}  e-{end}");
                if (_bufferIndex >= _dataLen)
                {
                    _offset += _dataLen;
                    _charUtils.Fill(_ioBuffer, input); // read supplementary char aware with CharacterUtils
                    if (_ioBuffer.Length == 0)
                    {
                        _dataLen = 0; // so next offset += dataLen won't decrement offset
                        if (length > 0)
                        {
                            break;
                        }
                        else
                        {
                            _finalOffset = CorrectOffset(_offset);
                            return false;
                        }
                    }
                    _dataLen = _ioBuffer.Length;
                    _bufferIndex = 0;
                }
                // use CharacterUtils here to support < 3.1 UTF-16 code unit behavior if the char based methods are gone
                int c = _charUtils.CodePointAt(_ioBuffer.Buffer, _bufferIndex, _ioBuffer.Length);

                int next = -1;
                if (_bufferIndex+1 < _ioBuffer.Length)
                    next = _charUtils.CodePointAt(_ioBuffer.Buffer, _bufferIndex + 1, _ioBuffer.Length);

                int charCount = Character.CharCount(c);
                _bufferIndex += charCount;

                bool last = (next < 0) || IsLastTokenChar(c, next); // if it's a token char

                if (length == 0) // start of token
                {
                    Debug.Assert(start == -1);
                    start = _offset + _bufferIndex - charCount;
                    end = start;
                } // check if a supplementary could run out of bounds
                else if (length >= buffer.Length - 1)
                {
                    buffer = _termAtt.ResizeBuffer(2 + length); // make sure a supplementary fits in the buffer
                }
                end += charCount;
                length += Character.ToChars(Normalize(c), buffer, length); // buffer it, normalized
                if (length >= MAX_WORD_LEN)
                    // buffer overflow! make sure to check for >= surrogate pair could break == test
                {
                    break;
                }
                if (last && length > 0)
                {
                    break; // return 'em
                }
            }

            _termAtt.Length = length;
            Debug.Assert(start != -1);
            _offsetAtt.SetOffset(CorrectOffset(start), _finalOffset = CorrectOffset(end));


            //Console.WriteLine($"returned term: {_termAtt.ToString()}");

            return true;
        }

        public sealed override void End()
        {
            base.End();
            // set final offset
            _offsetAtt.SetOffset(_finalOffset, _finalOffset);
        }

        public override void Reset()
        {
            base.Reset();
            _bufferIndex = 0;
            _offset = 0;
            _dataLen = 0;
            _finalOffset = 0;
            _ioBuffer.Reset(); // make sure to reset the IO buffer!!
        }
    }
}
