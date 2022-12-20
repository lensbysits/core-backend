using AutoMapper;
using Lens.Core.Lib.Models;
using Lens.Services.Masterdata.EF.Entities;
using Lens.Services.Masterdata.Models;
using LinqKit;
using System.Text.Json;

namespace Lens.Services.Masterdata.EF;

internal class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        // Masterdata
        CreateMap<MasterdataType, MasterdataTypeListModel>();
        CreateMap<MasterdataType, MasterdataTypeModel>();
        CreateMap<MasterdataTypeCreateModel, MasterdataType>()
            .AfterMap(UpdateMetadata);
        CreateMap<MasterdataTypeUpdateModel, MasterdataType>()
            .AfterMap(UpdateMetadata);

        CreateMap<Entities.Masterdata, MasterdataModel>();
        CreateMap<MasterdataCreateModel, Entities.Masterdata>();
        CreateMap<MasterdataUpdateModel, Entities.Masterdata>();
    }

    private void UpdateMetadata(IMetadataModel source, MasterdataType destination)
    {
        // if nothing was passed in, or we have no clue on the domain, we do nothing with the metadata
        if (source.Metadata is null || string.IsNullOrEmpty(source.Domain))
        {
            return;
        }

        Dictionary<string, dynamic>? finalMetadataDictionary = null;

        // if the database already contains metadata
        if (destination?.MetadataJson != null) 
        {
            finalMetadataDictionary = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(destination.MetadataJson);
            if (finalMetadataDictionary != null)
            {
                // We assume you passed in a complete metadata object (dictionary<string, dynamic)
                // We merge this nicely with the given metadata
                if (source.Domain == IMetadataModel.AllDomains)
                {
                    // for each passed in keyValue
                    string sourceMetadataJson = source.Metadata.ToString();
                    var sourceDictionary = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(sourceMetadataJson);
                    sourceDictionary.ForEach(keyValue =>
                    {
                        UpdateMetadataDictionary(keyValue.Key, keyValue.Value, finalMetadataDictionary);
                    });
                }
                // an metadata object was passed in for a specific domain
                // we only update that object
                else
                {
                    UpdateMetadataDictionary(source.Domain, source.Metadata, finalMetadataDictionary);
                }
            }
        }
        else
        {
            if (source.Domain != IMetadataModel.AllDomains)
            {
                finalMetadataDictionary = new Dictionary<string, dynamic>() { { source.Domain, source.Metadata } };
            }
            else
            {
                finalMetadataDictionary = source.MetadataDictionary;
            }
        }

        if (finalMetadataDictionary != null)
        {
            destination!.MetadataJson = JsonSerializer.Serialize(finalMetadataDictionary);
        }
    }

    private static void UpdateMetadataDictionary(string key, dynamic value, Dictionary<string, dynamic> metadataDictionary)
    {
        // if the key exists
        if (metadataDictionary.ContainsKey(key))
        {
            // and if passed in value is NOT null
            if (value is not null)
            {
                // update the value
                metadataDictionary[key] = value;
            }
            // if the passed in value is null
            else
            {
                // remove the value
                metadataDictionary.Remove(key);
            }
        }
        // if the key does not exists in the current metadata dictionary
        else
        {
            // and if passed in value is NOT null
            if (value is not null)
            {
                // add the passed in key value
                metadataDictionary.Add(key, value);
            }
        }
    }
}
