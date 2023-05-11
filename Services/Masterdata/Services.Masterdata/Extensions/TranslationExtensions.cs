using Ganss.Xss;
using Lens.Core.Data.EF.Translation.Models;
using System.Text.Json;

namespace Lens.Services.Masterdata;

public static class TranslationExtensions
{
    public static void Sanitize(this TranslationUpdateModel model, IHtmlSanitizer htmlSanitizer)
    {
        if (model.Translations != null && model.Translations.Any())
        {
            var translationString = model.Translations.ToString();
            if (!string.IsNullOrEmpty(translationString))
            {
                translationString = htmlSanitizer.Sanitize(translationString);

                JsonSerializerOptions options = new(JsonSerializerDefaults.Web);
                model.Translations = JsonSerializer.Deserialize<IEnumerable<TranslationModel>>(translationString, options);
            }
        }
    }
}
