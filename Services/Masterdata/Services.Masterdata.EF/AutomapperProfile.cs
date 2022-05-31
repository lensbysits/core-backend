using AutoMapper;
using Lens.Services.Masterdata.EF.Entities;
using Lens.Services.Masterdata.Models;

namespace Lens.Services.Masterdata.EF;

internal class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        // Masterdata
        CreateMap<MasterdataType, MasterdataTypeListModel>();
        CreateMap<MasterdataType, MasterdataTypeModel>();
        CreateMap<MasterdataTypeCreateModel, MasterdataType>();
        CreateMap<MasterdataTypeUpdateModel, MasterdataType>();

        CreateMap<Entities.Masterdata, MasterdataModel>();
        CreateMap<MasterdataCreateModel, Entities.Masterdata>();
        CreateMap<MasterdataUpdateModel, Entities.Masterdata>();
    }
}
