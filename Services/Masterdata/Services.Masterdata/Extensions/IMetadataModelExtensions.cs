using Lens.Core.Lib.Models;
using System.Text.Json;

namespace Lens.Services.Masterdata.Extensions
{
    public static class IMetadataModelExtensions
    {
        public static T? GetModel<T>(this IMetadataModel metadataModel, string domain)
        {
            if (metadataModel.MetadataDictionary?.TryGetValue(domain, out var metadata) ?? false)
            {
                var json = metadata.ToString();
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return default;
        }
    }
}
