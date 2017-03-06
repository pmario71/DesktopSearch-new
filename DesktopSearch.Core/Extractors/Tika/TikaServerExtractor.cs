using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DesktopSearch.Core.Tika;
using DesktopSearch.Core.DataModel.Documents;
using System.Threading.Tasks;
using DesktopSearch.Core.Contracts;
using DesktopSearch.Core.Configuration;
using DesktopSearch.Core.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DesktopSearch.Core.Extractors.Tika
{
    public interface ITikaServerExtractor
    {
        Task<DocDescriptor> ExtractAsync(ParserContext context, FileInfo filePath);

        /// <summary>
        /// Low-level extraction. Typically <see cref="ExtractAsync"/> is what you are looking for.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        Task<TikaRawResult> SendToTikaAsync(FileInfo filePath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<string> DetectLanguageAsync(string text);
    }

    public class TikaServerExtractor : ITikaServerExtractor, IDisposable
    {
        private readonly IConfigAccess<TikaConfig> _config;
        private readonly Lazy<ILogger<TikaServerExtractor>> _logger = new Lazy<ILogger<TikaServerExtractor>>(Logging.GetLogger<TikaServerExtractor>);

        private const string ContentTag = "X-TIKA:content";
        private const string ContentTypeTag = "Content-Type";

        private readonly HttpClient _client;

        /// <summary>
        /// Make extractor use a Tika Server on different uri.
        /// </summary>
        /// <param name="uri"></param>
        public TikaServerExtractor(IConfigAccess<TikaConfig> config)
        {
            _config = config;
            _client = new HttpClient()
            {
                BaseAddress = new Uri(config.Get().Uri)
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

            var tikaResult = await SendToTikaAsync(filePath);

            //TODO: switch to DocumentExtensions ??? 
            DocDescriptor doc;
            if (tikaResult.Error == ErrorState.UnsupportedFileType)
            {
                doc = DocDescriptor.UnsupportedFileType(filePath.FullName);
            }
            else
            {
                doc = await ConvertToDocumentAsync(filePath, tikaResult);
                try
                {
                    string language = await DetectLanguageAsync(TextUtil.ExtractText(doc.Content));
                    doc.LanguageID = language;
                }
                catch (Exception e)
                {
                    string msg = $"Failed to extract language from file: {filePath}";
                    _logger.Value.LogWarning(new EventId(LoggedIds.ErrorDetectingLanguage), e, msg);
                }
                doc.ExtractionDuration = ts.Elapsed;
            }

            return doc;
        }

        

        public async Task<string> DetectLanguageAsync(string text)
        {
            var httpRequestMessage = new HttpRequestMessage();

            httpRequestMessage.Method = HttpMethod.Put;
            httpRequestMessage.RequestUri = new Uri(_config.Get().Uri + "language/string");
            httpRequestMessage.Headers.Accept.Clear();
            httpRequestMessage.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("text/plain"));
                
            HttpContent httpContent = new StringContent(text, Encoding.UTF8, "text/plain");
            httpRequestMessage.Content = httpContent;

            var response = await _client.SendAsync(httpRequestMessage);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadAsStringAsync();
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

                if (msg.StatusCode == HttpStatusCode.UnsupportedMediaType)
                {
                    return TikaRawResult.FromError(ErrorState.UnsupportedFileType);
                }
                if (msg.StatusCode != HttpStatusCode.OK)
                {
                    throw new IndexExtractionException("No valid response from Tika Service!", filePath.FullName, ExtractionFailureType.TikaReturnedNoResults);
                }

                var content = await msg.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(content))
                {
                    throw new IndexExtractionException("No valid response from Tika Service!", filePath.FullName, ExtractionFailureType.TikaReturnedNoResults);
                }
                return TikaRawResult.FromContent(content);
            }
            catch(Exception ex)
            {
                throw new IndexExtractionException($"Sending file '{filePath}' to Tika failed!", ex);
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

            if (array[MetaTypes.Author] != null)
                doc.Author = array[MetaTypes.Author].ToString();

            if (array[MetaTypes.Keywords] != null)
                doc.Keywords = KeywordParser.Parse(array[MetaTypes.Keywords].ToString());

            if (array[MetaTypes.Title.ToLower()] != null)
                doc.Title = array[MetaTypes.Title.ToLower()].ToString();

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