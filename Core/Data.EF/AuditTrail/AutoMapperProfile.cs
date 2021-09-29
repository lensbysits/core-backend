using AutoMapper;
using Lens.Core.Data.EF.AuditTrail.Entities;
using Lens.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Lens.Core.Data.EF.AuditTrail
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<EntityChangeModel, EntityChange>()
                .ForMember(dest => dest.Changes, options =>
                options.MapFrom((src, dest) => JsonSerializer.Serialize(src.Changes ?? Array.Empty<EntityChangeProperty>())))
            .ReverseMap()
                .ForMember(change => change.Changes, options =>
                options.MapFrom((src, dest) => JsonSerializer.Deserialize<IEnumerable<EntityChangeProperty>>(src.Changes)));
        }
    }
}
