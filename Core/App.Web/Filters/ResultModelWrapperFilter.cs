using Lens.Core.Lib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections;

namespace Lens.Core.App.Web.Filters;

public class ResultModelWrapperFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        var ignore = context.Filters.Any(filter => filter is IgnoreResultModelWrapperAttribute);
        if (ignore) return;

        switch (context.Result)
        {
            case ObjectResult objectResult:
            {
                if (objectResult.Value != null)
                {
                    var result = VerifyResult(objectResult.Value);
                    context.Result = new ObjectResult(result);
                }
                break;
            }
            default:
                return;
        }
    }

    private static object VerifyResult(object value)
    {
        // Get the type of object that is being returned.
        var valueType = value.GetType();
        var resultType = valueType;
        Type? returnType;
        object? result;

        // If the generic type is an IResultModel<> then we're good and return the given object
        if (value is IResultModel)
            return value;

        // If the result is some kind of IEnumerable
        if (value is IEnumerable || valueType.IsArray)
        {
            // Get the type of the objects in the array;
            if (valueType.IsArray)
            {
                var array = value as Array;
                resultType = array!.Length > 0 ?
                    (value as Array)?.GetValue(0)?.GetType() :
                    typeof(object);
            }
            // Else get the type of the objects in the IEnumerable
            else
            {
                resultType = resultType.GetGenericArguments()[0];
            }

            // Create a typed ResultListModel from the resultType found earlier.
            returnType = typeof(ResultListModel<>).MakeGenericType(resultType!);
            result = Activator.CreateInstance(returnType, value);

            // Return the ResultListModel<TResultType>
            return result ?? new object();
        }

        // return an instance of ResultModel<TResultType> with the given value;
        returnType = typeof(ResultModel<>).MakeGenericType(resultType);
        result = Activator.CreateInstance(returnType);
        returnType.GetProperty(nameof(IResultModel<object>.Value))?.SetValue(result, value);
        return result ?? new object();
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
}
