namespace Lens.Services.Masterdata.Models;

public class TranslationModel
{
    public string Language { get; set; }
    public bool IsDefault { get; set; }
    public IEnumerable<TranslatedField> Values { get; set; }
}

public class TranslatedField
{
    public string Field { get; set; }
    public string Value { get; set; }
}