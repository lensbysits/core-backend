using Lens.Core.Blob.Models;
using Lens.Core.Lib.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Lens.Core.Blob.Services
{
    public interface IBlobManagementService
    {
        Task<BlobInfoModel> AddBlob(Guid entityId, IFormFile file, string relativeSubfolder = null);
        Task<(Stream blob, BlobInfoModel blobInfo)> GetBlob(Guid blobInfoId);
        Task<ResultListModel<BlobInfoModel>> GetBlobList(Guid id);
        Task DeleteBlob(Guid blobInfoId);
    }

}
