using Lens.Core.Blob.Models;
using Lens.Core.Lib.Services;
using Microsoft.Extensions.Configuration;

namespace Lens.Core.Blob.Services;

public class AzureStorageBlobService : BaseService<AzureStorageBlobService>, IBlobService
{
    private readonly BlobSettings _blobServiceSettings;

    public AzureStorageBlobService(
        IApplicationService<AzureStorageBlobService> applicationService,
        IConfiguration configuration) : base(applicationService)
    {
        _blobServiceSettings = configuration.GetSection(nameof(BlobSettings)).Get<BlobSettings>();
    }

    public Task<bool> DeleteBlob(string relativePathAndName)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> Download(string relativePathAndName)
    {
        throw new NotImplementedException();
    }

    public Task<string[]> GetBlobs()
    {
        throw new NotImplementedException();
    }

    public Task<string> GetBlobUrl(string relativePathAndName)
    {
        throw new NotImplementedException();
    }

    public Task<BlobMetadataModel> Upload(string relativePathAndName, Stream stream)
    {
        throw new NotImplementedException();
    }
}
