using Lens.Core.Blob.Data;
using Lens.Core.Blob.Data.Entities;
using Lens.Core.Blob.Models;
using Lens.Core.Data.EF;
using Lens.Core.Lib.Models;
using Lens.Core.Lib.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lens.Core.Blob.Services
{
    public class BlobManagementService : BaseService<BlobManagementService>, IBlobManagementService
    {
        private readonly IBlobService _blobService;
        private readonly BlobDbContext _blobDbContext;

        public BlobManagementService(
            IApplicationService<BlobManagementService> applicationService,
            IBlobService blobService,
            BlobDbContext blobDbContext) : base(applicationService)
        {
            _blobService = blobService;
            _blobDbContext = blobDbContext;
        }

        #region IBlobManagementService implementation
        public async Task<(Stream blob, BlobInfoModel blobInfo)> GetBlob(Guid blobInfoId)
        {
            var blobInfo = await _blobDbContext.BlobInfos.GetById(blobInfoId);
            var blob = await _blobService.Download(blobInfo.RelativePathAndName);
            return (blob, ApplicationService.Mapper.Map<BlobInfoModel>(blobInfo));
        }

        public async Task<ResultListModel<BlobInfoModel>> GetBlobList(Guid id)
        {
            var blobInfos = _blobDbContext.BlobInfos
                .Where(bi => bi.EntityId == id);

            return await blobInfos
                .ToResultList<BlobInfo, BlobInfoModel>(new QueryModel { NoLimit = true }, ApplicationService.Mapper);
        }

        public async Task<BlobInfoModel> AddBlob(Guid entityId, IFormFile file, string relativeSubfolder = null)
        {
            var value = new BlobInfoMergeModel
            {
                FilenameWithExtension = file.FileName,
                Size = (int)file.Length,
                ContentType = file.ContentType,
                EntityId = entityId
            };

            // add blob info
            var blobInfo = _blobDbContext.BlobInfos.Add(ApplicationService.Mapper.Map<BlobInfo>(value)).Entity;

            var fileInfo = new FileInfo(blobInfo.FilenameWithExtension);
            blobInfo.FilenameWithoutExtension = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
            blobInfo.FileExtension = fileInfo.Extension;
            if (string.IsNullOrEmpty(relativeSubfolder))
            {
                relativeSubfolder = $"uploads/";
            }
            blobInfo.RelativePathAndName = Path.Combine(relativeSubfolder, blobInfo.FilenameWithExtension);

            await _blobDbContext.SaveChangesAsync();

            // upload blob info
            try
            {
                BlobMetadataModel blobMetadata = await _blobService.Upload(blobInfo.RelativePathAndName, file.OpenReadStream());
                ApplicationService.Mapper.Map(blobMetadata, blobInfo);
                
                await _blobDbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                // rollback blob info entity if something went wrong when uploading the blob
                _blobDbContext.BlobInfos.Remove(blobInfo);
                await _blobDbContext.SaveChangesAsync();

                throw;
            }

            return ApplicationService.Mapper.Map<BlobInfoModel>(blobInfo);
        }

        public async Task DeleteBlob(Guid blobInfoId)
        {
            var blobEntity = await _blobDbContext.BlobInfos.GetById(blobInfoId);
            bool deleteSucces = await _blobService.DeleteBlob(blobEntity.RelativePathAndName);
            if (deleteSucces)
            {
                _blobDbContext.BlobInfos.Remove(blobEntity);
                await _blobDbContext.SaveChangesAsync();
            }
        }

        #endregion IBlobManagementService implementation
    }
}