using Lens.Core.Blob.Data;
using Lens.Core.Blob.Data.Entities;
using Lens.Core.Blob.Models;
using Lens.Core.Data.EF;
using Lens.Core.Lib.Models;
using Lens.Core.Lib.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lens.Core.Blob.Services
{
    public class BlobManagementService : BaseService<BlobManagementService>, IBlobManagementService
    {
        private const string DefaultRelativePath = @"uploads/";
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

        public async Task<ResultListModel<BlobInfoModel>> GetBlobList(Guid entityId, QueryModel queryModel = null)
        {
            var blobInfos = _blobDbContext.BlobInfos
                .Where(bi => bi.EntityId == entityId);

            if (queryModel == null)
            {
                queryModel = new QueryModel { NoLimit = true };
            }

            return await blobInfos
                .GetByQueryModel(queryModel)
                .ToResultList<BlobInfo, BlobInfoModel>(queryModel, ApplicationService.Mapper);
        }

        public async Task<BlobInfoModel> AddBlob(Guid entityId, IFormFile file, string relativePath = null)
        {
            var blobInfoItem = new BlobInfoInputModel() { EntityId = entityId };
            return await AddBlob(blobInfoItem, file, relativePath);
        }

        public async Task<BlobInfoModel> AddBlob(BlobInfoInputModel blobInfoItem, IFormFile file, string relativePath = null)
        {
            var blobInfoCreateModel = GetInitialBlobInfo(file);
            ApplicationService.Mapper.Map(blobInfoItem, blobInfoCreateModel);

            return await AddBlob(blobInfoCreateModel, file, relativePath);
        }

        public async Task<IEnumerable<BlobInfoModel>> AddBlob(IEnumerable<BlobInfoInputModel> blobInfoItems, IFormFile file, string relativePath = null)
        {
            var blobInfoCreateModel = GetInitialBlobInfo(file);
            var blobInfoBulkCreateModel = ApplicationService.Mapper.Map<BlobInfoBulkCreateModel>(blobInfoCreateModel);
            blobInfoBulkCreateModel.BlobInfoItems = blobInfoItems;

            return await AddBlobForMultipleEntities(blobInfoBulkCreateModel, file, relativePath);
        }

        public async Task DeleteBlob(Guid blobInfoId)
        {
            var blobEntity = await _blobDbContext.BlobInfos.GetById(blobInfoId);
            bool canDeleteBlobInfo = true;
            
            if (!blobEntity.SkipFileDeletion)
            {
                canDeleteBlobInfo = await _blobService.DeleteBlob(blobEntity.RelativePathAndName);
            }
            
            if (canDeleteBlobInfo)
            {
                _blobDbContext.BlobInfos.Remove(blobEntity);
                await _blobDbContext.SaveChangesAsync();
            }
        }

        #endregion IBlobManagementService implementation

        #region Private Methods

        private async Task<BlobInfoModel> AddBlob(BlobInfoCreateModel value, IFormFile file, string relativePath)
        {
            var fileInfo = GetFileInfo(value.FilenameWithExtension, relativePath);

            // create blob info entity
            var blobInfo = _blobDbContext.BlobInfos.Add(ApplicationService.Mapper.Map<BlobInfo>(value)).Entity;
            ApplicationService.Mapper.Map(fileInfo, blobInfo);

            // upload blob
            BlobMetadataModel blobMetadata = await _blobService.Upload(blobInfo.RelativePathAndName, file.OpenReadStream());
            ApplicationService.Mapper.Map(blobMetadata, blobInfo);

            // save blob info entity
            await _blobDbContext.SaveChangesAsync();

            return ApplicationService.Mapper.Map<BlobInfoModel>(blobInfo);
        }

        private async Task<IEnumerable<BlobInfoModel>> AddBlobForMultipleEntities(BlobInfoBulkCreateModel value, IFormFile file, string relativePath)
        {
            var fileInfo = GetFileInfo(value.FilenameWithExtension, relativePath);

            // create blob info entities
            var blobInfoList = new List<BlobInfo>();
            foreach (var blobInfoItem in value.BlobInfoItems)
            {
                var blobInfo = _blobDbContext.BlobInfos.Add(ApplicationService.Mapper.Map<BlobInfo>(value)).Entity;
                ApplicationService.Mapper.Map(fileInfo, blobInfo);
                ApplicationService.Mapper.Map(blobInfoItem, blobInfo);

                blobInfoList.Add(blobInfo);
            }

            // upload blob
            BlobMetadataModel blobMetadata = await _blobService.Upload(fileInfo.RelativePathAndName, file.OpenReadStream());
            foreach (var blobInfo in blobInfoList)
            {
                ApplicationService.Mapper.Map(blobMetadata, blobInfo);
            }

            // save blob info entities
            await _blobDbContext.SaveChangesAsync();

            return blobInfoList.Select(bi =>
            {
                return ApplicationService.Mapper.Map<BlobInfoModel>(bi);
            });
        }

        private static BlobInfoCreateModel GetInitialBlobInfo(IFormFile file)
        {
            return new BlobInfoCreateModel
            {
                FilenameWithExtension = file.FileName,
                Size = (int)file.Length,
                ContentType = file.ContentType
            };
        }

        public static FileInfoModel GetFileInfo(string fileNameWithExtension, string relativePath)
        {
            var fileInfo = new FileInfo(fileNameWithExtension);
            var filenameWithoutExtension = fileInfo.Name[..^fileInfo.Extension.Length];
            var fileExtension = fileInfo.Extension;
            if (string.IsNullOrEmpty(relativePath))
            {
                relativePath = DefaultRelativePath;
            }
            var relativePathAndName = Path.Combine(relativePath, fileNameWithExtension);

            return new FileInfoModel
            {
                FileExtension = fileExtension,
                FilenameWithoutExtension = filenameWithoutExtension,
                RelativePathAndName = relativePathAndName
            };
        }

        #endregion Private Methods
    }
}