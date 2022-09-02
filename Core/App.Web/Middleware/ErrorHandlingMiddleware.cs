using CorrelationId.Abstractions;
using Lens.Core.Lib.Exceptions;
using Lens.Core.Lib.Extensions;
using Lens.Core.Lib.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Lens.Core.App.Web;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ICorrelationContextAccessor correlationContext;
    private static readonly Dictionary<Type, HttpStatusCode> _exceptionTypes = new()
    {
        {typeof(NotFoundException), HttpStatusCode.NotFound},
        {typeof(BadRequestException), HttpStatusCode.BadRequest},
        {typeof(NotAllowedException), HttpStatusCode.MethodNotAllowed},
        {typeof(UnauthorizedException), HttpStatusCode.Unauthorized},
        {typeof(ForbiddenException), HttpStatusCode.Forbidden},
        {typeof(FormatException), HttpStatusCode.BadRequest},
        {typeof(DivideByZeroException), HttpStatusCode.BadRequest},
        {typeof(NullReferenceException), HttpStatusCode.BadRequest},
        {typeof(InvalidCastException), HttpStatusCode.BadRequest},
        {typeof(InvalidOperationException), HttpStatusCode.BadRequest},
        {typeof(ValidationException), HttpStatusCode.UnprocessableEntity},
        {typeof(ArgumentException), HttpStatusCode.UnprocessableEntity},
        {typeof(ArgumentNullException), HttpStatusCode.UnprocessableEntity},
        {typeof(InvalidDataException), HttpStatusCode.UnprocessableEntity},
    };
    protected readonly ILogger _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment webHostEnvironment, ICorrelationContextAccessor correlationContext)
    {
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
        this.correlationContext = correlationContext;
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    #region Private Methods
    private static HttpStatusCode GetHttpStatusCodeForException(Type exceptionType)
    {
        foreach (var kv in _exceptionTypes)
        {
            if (exceptionType.Equals(kv.Key) || exceptionType.IsSubclassOf(kv.Key))
            {
                return kv.Value;
            }
        }

        return HttpStatusCode.InternalServerError;
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var exceptionMessage = GetExceptionMessage(exception);
        var exceptionType = exception.GetType();
        var serializedData = JsonSerializer.Serialize(exception.Data);
        var correlationId = correlationContext.CorrelationContext.CorrelationId;
        HttpStatusCode httpStatusCodeForException = GetHttpStatusCodeForException(exceptionType);

        if (httpStatusCodeForException != HttpStatusCode.InternalServerError)
        {
            // log as warning the expected exceptions
            _logger.LogWarning(exception, $"Exception: {exceptionType.Name} (expected): HTTP Status: {(int)httpStatusCodeForException} ({httpStatusCodeForException}): {exceptionMessage} (data: {serializedData})");
        }
        else
        {
            // log as error the unexpected exceptions
            httpStatusCodeForException = HttpStatusCode.InternalServerError;
            _logger.LogError(exception, $"Exception: {exceptionType.Name} (unexpected): HTTP Status: {(int)httpStatusCodeForException} ({httpStatusCodeForException}): {exceptionMessage} (data: {serializedData})");
        }


        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)httpStatusCodeForException;
        string result;

        if (_webHostEnvironment.IsDevelopment())
        {
            result = JsonSerializer.Serialize(new ErrorResultModel
            {
                Message = exceptionMessage,
                ErrorType = exceptionType.Name,
                ErrorDetails = exception.GetFullExceptionData(),
                Stacktrace = exception.StackTrace,
                CorrelationId = correlationId,
                Data = exception.Data,
                DataDetails = exception.GetSerializableDataDictionary(true)                
            });
        }
        else
        {
            result = JsonSerializer.Serialize(new ErrorResultModel
            {
                Message = exceptionMessage,
                ErrorType = exceptionType.Name,
                CorrelationId = correlationId,
                Data = exception.Data,
                DataDetails = exception.GetSerializableDataDictionary(true)
            });
        }
        return context.Response.WriteAsync(result);
    }

    /// <summary>
    /// Get exception message recursively including the inner exception messages, too.
    /// </summary>
    private string GetExceptionMessage(Exception e)
    {
        var message = e.Message;
        if (e.InnerException != null)
        {
            message = $"{message}{Environment.NewLine}{GetExceptionMessage(e.InnerException)}";
        }

        return message;
    }
    #endregion
}
