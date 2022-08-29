namespace Lens.Core.Data.Attributes;

/// <summary>
/// The attribute is designed for those properties of an auditable entity that we want to exclude from the audit trailing.
/// (eg: Image fields containing big data, or big JSONs)
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NonAuditAttribute : Attribute
{
}
