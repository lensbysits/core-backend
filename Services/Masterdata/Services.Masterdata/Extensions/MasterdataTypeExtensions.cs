using Ganss.Xss;
using Lens.Services.Masterdata.Models;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;

namespace Lens.Services.Masterdata;

public static class MasterdataTypeExtensions
{
    public static void Sanitize(this MasterdataTypeCreateModel model, IHtmlSanitizer htmlSanitizer)
    {
        model.Code = !string.IsNullOrEmpty(model.Code) ? htmlSanitizer.Sanitize(model.Code) : model.Code;
        model.Name = !string.IsNullOrEmpty(model.Name) ? htmlSanitizer.Sanitize(model.Name) : model.Name;
        model.Description = !string.IsNullOrEmpty(model.Description) ? htmlSanitizer.Sanitize(model.Description) : model.Description;
        if(model.Metadata != null)
        {
            var metadataString = model.Metadata.ToString();
            if (!string.IsNullOrEmpty(metadataString))
            {
                metadataString = htmlSanitizer.Sanitize(metadataString);
                model.Metadata = JsonSerializer.Deserialize<JsonElement>(metadataString);
            }
        }
    }
    public static void Sanitize(this MasterdataTypeUpdateModel model, IHtmlSanitizer htmlSanitizer)
    {
        model.Name = !string.IsNullOrEmpty(model.Name) ? htmlSanitizer.Sanitize(model.Name) : model.Name;
        model.Description = !string.IsNullOrEmpty(model.Description) ? htmlSanitizer.Sanitize(model.Description) : model.Description;
        if (model.Metadata.HasValue)
        {
            var metadataString = model.Metadata.ToString();
            if (!string.IsNullOrEmpty(metadataString))
            {
                metadataString = htmlSanitizer.Sanitize(metadataString);
                model.Metadata = JsonSerializer.Deserialize<JsonElement>(metadataString);
            }
        }
    }
    public static void Sanitize(this MasterdataCreateModel model, IHtmlSanitizer htmlSanitizer)
    {
        model.Key = !string.IsNullOrEmpty(model.Key) ? htmlSanitizer.Sanitize(model.Key) : model.Key;
        model.Value = !string.IsNullOrEmpty(model.Value) ? htmlSanitizer.Sanitize(model.Value) : model.Value;
        model.Name = !string.IsNullOrEmpty(model.Name) ? htmlSanitizer.Sanitize(model.Name) : model.Name;
        model.Description = !string.IsNullOrEmpty(model.Description) ? htmlSanitizer.Sanitize(model.Description) : model.Description;
        if (model.Metadata.HasValue)
        {
            var metadataString = model.Metadata.ToString(); 
            if (!string.IsNullOrEmpty(metadataString))
            {
                metadataString = htmlSanitizer.Sanitize(metadataString);
                model.Metadata = JsonSerializer.Deserialize<JsonElement>(metadataString);
            }
        }
        if (model.Tags != null)
        {
            model.Tags = SanitizeTags(model.Tags, htmlSanitizer);
        }
    }
    public static void Sanitize(this MasterdataUpdateModel model, IHtmlSanitizer htmlSanitizer)
    {
        model.Value = !string.IsNullOrEmpty(model.Value) ? htmlSanitizer.Sanitize(model.Value) : model.Value;
        model.Name = !string.IsNullOrEmpty(model.Name) ? htmlSanitizer.Sanitize(model.Name) : model.Name;
        model.Description = !string.IsNullOrEmpty(model.Description) ? htmlSanitizer.Sanitize(model.Description) : model.Description;
        if (model.Metadata.HasValue)
        {
            var metadataString = model.Metadata.ToString(); 
            if (!string.IsNullOrEmpty(metadataString))
            {
                metadataString = htmlSanitizer.Sanitize(metadataString);
                model.Metadata = JsonSerializer.Deserialize<JsonElement>(metadataString);
            }
        }
        if (model.Tags != null)
        {
            model.Tags = SanitizeTags(model.Tags, htmlSanitizer);
        }
    }

    private static string[] SanitizeTags(string[] tags, IHtmlSanitizer htmlSanitizer)
    {
        var tagsClean = new List<string>();
        foreach (var item in tags)
        {
            var itemClean = htmlSanitizer.Sanitize(item);
            if (!string.IsNullOrEmpty(itemClean))
            {
                tagsClean.Add(itemClean);
            }
        }
        return tagsClean.Distinct().ToArray();
    }
}
