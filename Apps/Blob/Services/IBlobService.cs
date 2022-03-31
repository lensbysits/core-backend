using System.IO;
using System.Threading.Tasks;

namespace Lens.App.Blob
{
    public interface IBlobService
    {
        Task<BlobMetadata> Upload(string blobInfoId, string relativePathAndName, Stream stream);
        Task<Stream> Download(string relativePathAndName);
        Task<string[]> GetBlobs();
        Task<string> GetBlobUrl(string relativePathAndName);
        Task<bool> DeleteBlob(string relativePathAndName);
    }
}
