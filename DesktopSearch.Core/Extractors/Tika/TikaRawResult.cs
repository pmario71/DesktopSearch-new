namespace DesktopSearch.Core.Extractors.Tika
{

    /// <summary>
    /// Result from Tika Extraction
    /// </summary>
    public class TikaRawResult
	{
        private readonly string _result;

        internal TikaRawResult(string result)
        {
            _result = result;
        }

        internal string Raw { get { return _result; } }

        public override string ToString()
        {
            return _result;
        }
    }
}