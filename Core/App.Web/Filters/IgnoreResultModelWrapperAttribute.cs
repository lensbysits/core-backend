using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Lens.Core.App.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class IgnoreResultModelWrapperAttribute : Attribute, IFilterMetadata
    {
    }
}
