using System.IO;

namespace Lens.Core.Blob.Models
{
    public class BlobDownloadResultModel
    {
        public BlobDownloadResultModel(Stream content, string mimeType, int? contentLength)
        {
            Stream = content;
            ContentType = mimeType;
            ContentLength = contentLength ?? 0;
        }

        public Stream Stream { get; private set; }
        public string ContentType { get; private set; }
        public int ContentLength { get; private set; }
    }
}
