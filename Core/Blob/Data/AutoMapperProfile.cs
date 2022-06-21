using AutoMapper;
using Lens.Core.Blob.Data.Entities;
using Lens.Core.Blob.Models;

namespace Lens.Core.Blob.Data
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BlobInfo, BlobInfoModel>();
            CreateMap<BlobInfoBulkCreateModel, BlobInfo>();
            CreateMap<BlobInfoCreateModel, BlobInfo>();
            CreateMap<BlobInfoCreateModel, BlobInfoBulkCreateModel>();
            CreateMap<BlobMetadataModel, BlobInfo>();
            CreateMap<FileInfoModel, BlobInfo>();
        }
    }
}
