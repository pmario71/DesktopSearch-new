using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DesktopSearch.Core.Tika;
using DesktopSearch.Core.DataModel.Documents;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Extractors.Tika
{
    public class TikaServerExtractor : IDisposable
    {
        private const string ContentTag = "X-TIKA:content";
        private const string ContentTypeTag = "Content-Type";

        private readonly HttpClient _client;

        /// <summary>
        /// Use local Tika Server listenting on default port (9998).
        /// </summary>
        public TikaServerExtractor() : this(new Uri("http://localhost:9998/"))
        {
        }

        /// <summary>
        /// Make extractor use a Tika Server on different uri.
        /// </summary>
        /// <param name="uri"></param>
        public TikaServerExtractor(Uri uri)
        {
            _client = new HttpClient()
            {
                BaseAddress = uri
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Extracts index information and maps it to a <see cref="DocDescriptor"/> object.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<DocDescriptor> ExtractAsync(ParserContext context, FileInfo filePath)
        {
            var ts = Stopwatch.StartNew();

            var content = await SendToTikaAsync(filePath);
            var doc = await ConvertToDocumentAsync(filePath, content);

            doc.ExtractionDuration = ts.Elapsed;

            return doc;
        }

        /// <summary>
        /// Streams file content to Tika service and returns the extracted information as string.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<TikaRawResult> SendToTikaAsync(FileInfo filePath)
        {
            try
            {
                var msg = await this.SendFileAsync(filePath);
                var content = await msg.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(content))
                {
                    throw new IndexExtractionException("No valid response from Tika Service!", filePath.FullName, ExtractionFailureType.TikaReturnedNoResults);
                }
                return new TikaRawResult(content);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// Parses tika json result into <see cref="DocDescriptor"/>.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="tikaResult"></param>
        /// <returns></returns>
        public async Task<DocDescriptor> ConvertToDocumentAsync(FileInfo filePath, TikaRawResult tikaResult)
        {
            var array = await Task.Run<JToken>(() => (((JArray)JsonConvert.DeserializeObject(tikaResult.Raw))).First);

            var doc = new DocDescriptor()
            {
                Title = filePath.Name,
                Path = filePath.FullName,
                //doc.LastModified (filePath.CreationTime);
                LastModified = filePath.LastWriteTime
            };

            if (array[ContentTypeTag] != null)
                doc.ContentType = array[ContentTypeTag].ToString();

            if (array["Author"] != null)
                doc.Author = array["Author"].ToString();

            if (array["Keywords"] != null)
                doc.Keywords = KeywordParser.Parse(array["Keywords"].ToString());

            if (array["title"] != null)
                doc.Title = array["title"].ToString();

            if (array[ContentTag] != null)
            {
                //var a = new Nest.Attachment();
                //a.Content = array[ContentTag].ToString();
                doc.Content = array[ContentTag].ToString();
            }

            return doc;
        }

        private async Task<HttpResponseMessage> SendFileAsync(FileInfo filePath)
        {
            using (var strm = new FileStream(filePath.FullName, FileMode.Open, FileAccess.Read))
            {
                HttpContent streamContent = new StreamContent(strm);
                return await _client.PutAsync("rmeta", streamContent);
            }
        }

        // ---------------------------
        // --- available meta data ---
        // ---------------------------
        // Author
        // Content-Type
        // Creation-Date
        // Last-Modified
        // Last-Save-Date
        // X-Parsed-By
        // X-TIKA:content
        // X-TIKA:parse_time_millis
        // created
        // creator
        // date
        // dc:creator
        // dc:format
        // dc:title
        // dcterms:created
        // dcterms:modified
        // meta:author
        // meta:creation-date
        // meta:save-date
        // modified
        // pdf:PDFVersion
        // pdf:encrypted
        // producer
        // title
        // xmp:CreatorTool

        #region Dispose Pattern
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                // if this is a dispose call dispose on all state you 
                // hold, and take yourself off the Finalization queue.
                if (disposing)
                {
                    _client.Dispose();
                }

                //TODO: free own state (unmanaged objects)

                _disposed = true;
            }
        }
        #endregion
    }

    public static class JTokenEx
    {
        public static void GetSafe(this JToken token, string name, ref string value)
        {
            if (token[name] != null)
                value = token[name].ToString();
        }
    }
}