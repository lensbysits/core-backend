using Lens.Core.Blob.Models;
using Lens.Core.Lib.Models;
using Microsoft.AspNetCore.Http;

namespace Lens.Core.Blob.Services;

public interface IBlobManagementService
{
    Task<BlobInfoModel> AddBlob(Guid entityId, IFormFile file, string? relativePath = null);
    Task<BlobInfoModel> AddBlob(BlobInfoInputModel blobInfoItem, IFormFile file, string? relativePath = null);
    Task<IEnumerable<BlobInfoModel>> AddBlob(IEnumerable<BlobInfoInputModel> blobInfoItems, IFormFile file, string? relativePath = null);
    Task<(Stream blob, BlobInfoModel blobInfo)> GetBlob(Guid blobInfoId);
    Task<ResultListModel<BlobInfoModel>> GetBlobList(Guid entityId, QueryModel? queryModel = null);
    Task DeleteBlob(Guid blobInfoId);
}
