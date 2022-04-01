using AutoMapper;
using Lens.Services.Masterdata.EF.Entities;
using Lens.Services.Masterdata.Models;

namespace Lens.Services.Masterdata.EF;

internal class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        // Masterdata
        CreateMap<MasterdataType, MasterdataTypeListBM>();
        CreateMap<MasterdataType, MasterdataTypeBM>();
        CreateMap<MasterdataTypeCreateBM, MasterdataType>();
        CreateMap<MasterdataTypeUpdateBM, MasterdataType>();

        CreateMap<Entities.Masterdata, MasterdataBM>();
        CreateMap<MasterdataCreateBM, Entities.Masterdata>();
        CreateMap<MasterdataUpdateBM, Entities.Masterdata>();
    }
}
