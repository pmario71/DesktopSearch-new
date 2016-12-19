using DesktopSearch.Core.Contracts;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSearch.Core.Tagging
{
    public class Tagger
    {

        public async Task<TagDescriptor> ReadAsync(FileInfo file)
        {
            return await Task.Run<TagDescriptor>(() =>
            {
                using (var src = file.OpenRead())
                {
                    using (PdfReader reader = new PdfReader(src))
                    {
                        var info = reader.Info;

                        var td = new TagDescriptor(info);
                        return td;
                    }
                }
            });
        }

        public async Task WriteAsync(FileInfo file, TagDescriptor tagDescriptor)
        {
            await Task.Run(() =>
            {
                using (var src = file.OpenRead())
                using (PdfReader reader = new PdfReader(src))
                using (var stamper = new PdfStamper(reader, new FileStream(file.FullName, FileMode.Create)))
                {
                    stamper.MoreInfo = tagDescriptor.Tags;
                }
            });
        }

        /// <summary>
        /// Not cleary if operation makes sense on Tagger class
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAvailableTags(string filter)
        {
            throw new NotImplementedException();
        }
    }
}
