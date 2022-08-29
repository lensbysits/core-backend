using Lens.Core.Blob.Models;

namespace Lens.Core.Blob.Services;

public interface IBlobService
{
    Task<BlobMetadataModel> Upload(string relativePathAndName, Stream stream);
    Task<Stream> Download(string relativePathAndName);
    Task<BlobDownloadResultModel> DownloadWithMetadata(string relativePathAndName);
    Task<string[]> GetBlobs();
    Task<string> GetBlobUrl(string relativePathAndName);
    Task<bool> DeleteBlob(string relativePathAndName);
    Task MoveBlobWithinContainer(string sourceRelativePathAndName, string targetRelativePathAndName);
}