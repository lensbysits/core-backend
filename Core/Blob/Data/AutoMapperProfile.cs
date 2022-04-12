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
            CreateMap<BlobInfoMergeModel, BlobInfo>();
            CreateMap<BlobMetadataModel, BlobInfo>();
        }
    }
}
