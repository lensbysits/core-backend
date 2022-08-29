using Microsoft.AspNetCore.Mvc.Filters;

namespace Lens.Core.App.Web.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class IgnoreResultModelWrapperAttribute : Attribute, IFilterMetadata
{
}
