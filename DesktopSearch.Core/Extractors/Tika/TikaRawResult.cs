using System;
using DesktopSearch.Core.DataModel.Documents;

namespace DesktopSearch.Core.Extractors.Tika
{

    /// <summary>
    /// Result from Tika Extraction
    /// </summary>
    public class TikaRawResult
    {
        private readonly string _result;

        private TikaRawResult(string result)
        {
            _result = result;
        }

        private TikaRawResult(ErrorState error)
        {
            Error = error;
        }

        internal string Raw
        {
            get
            {
                if (Error != ErrorState.None)
                {
                    throw new InvalidOperationException("Object is in Error state!");
                }
                return _result;
            }
        }

        public ErrorState Error { get; set; }

        public override string ToString()
        {
            return _result;
        }

        public static TikaRawResult FromContent(string rawContent)
        {
            var result = new TikaRawResult(rawContent);
            return result;
        }

        public static TikaRawResult FromError(ErrorState error)
        {
            var result = new TikaRawResult(error);
            result.Error = error;
            return result;
        }
    }
}