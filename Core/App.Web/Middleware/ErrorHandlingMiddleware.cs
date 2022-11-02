using CorrelationId.Abstractions;
using Lens.Core.Lib.Configuration;
using Lens.Core.Lib.Exceptions;
using Lens.Core.Lib.Extensions;
using Lens.Core.Lib.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace Lens.Core.App.Web;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ApiExceptionHandlingConfig config;
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
        {typeof(ArgumentException), HttpStatusCode.UnprocessableEntity},
        {typeof(ArgumentNullException), HttpStatusCode.UnprocessableEntity},
        {typeof(InvalidDataException), HttpStatusCode.UnprocessableEntity},
    };
    protected readonly ILogger _logger;

    public ErrorHandlingMiddleware(
        RequestDelegate next, 
        ILogger<ErrorHandlingMiddleware> logger, 
        IWebHostEnvironment webHostEnvironment, 
        ICorrelationContextAccessor correlationContext,
        IOptions<ApiExceptionHandlingConfig> exceptionHandlingConfig)
    {
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
        this.correlationContext = correlationContext;
        this.next = next;
        this.config = exceptionHandlingConfig?.Value ?? throw new ArgumentNullException(nameof(exceptionHandlingConfig));
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch(PublicException ex)
        {
            await HandleExceptionAsync(context, ex, ex.HttpStatusCode, (isDevelopment, message, exceptionType) =>
            {
                return isDevelopment
                ? this.GenerateDevelopmentException(ex, message, ex.ErrorCode, exceptionType)
                : this.GenerateDetailedException(ex, message, ex.ErrorCode, exceptionType);
            });
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, null, (isDevelopment, message, exceptionType) =>
            {
                return isDevelopment
               ? this.GenerateDevelopmentException(ex, message, null, exceptionType)
               : this.config.DisableEnhancedApiProtection
                    ? this.GenerateDetailedException(ex, message, null, exceptionType)
                    : this.GenerateGenericException();
            });
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

    private Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode? statusCode, Func<bool, string, Type, ErrorResultModel> generateResponse)
    {
        var exceptionMessage = GetExceptionMessage(exception);
        var exceptionType = exception.GetType();
        var httpStatusCodeForException = statusCode ?? GetHttpStatusCodeForException(exceptionType);

        LogException(exception, exceptionMessage, exceptionType, httpStatusCodeForException);

        var result = generateResponse(_webHostEnvironment.IsDevelopment(), exceptionMessage, exceptionType);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)httpStatusCodeForException;
        return context.Response.WriteAsync(JsonSerializer.Serialize(result));
    }

    private void LogException(Exception exception, string exceptionMessage, Type exceptionType, HttpStatusCode httpStatusCodeForException)
    {
        var serializedData = JsonSerializer.Serialize(exception.Data);
        if (httpStatusCodeForException != HttpStatusCode.InternalServerError)
        {
            // log as warning the expected exceptions
            _logger.LogWarning(exception, $"Exception: {exceptionType.Name} (expected): " +
                $"HTTP Status: {(int)httpStatusCodeForException} ({httpStatusCodeForException}): {exceptionMessage} " +
                $"(data: {serializedData})");
        }
        else
        {
            // log as error the unexpected exceptions
            httpStatusCodeForException = HttpStatusCode.InternalServerError;
            _logger.LogError(exception, $"Exception: {exceptionType.Name} (unexpected): " +
                $"HTTP Status: {(int)httpStatusCodeForException} ({httpStatusCodeForException}): {exceptionMessage} " +
                $"(data: {serializedData})");
        }
    }

    private ErrorResultModel GenerateDevelopmentException(Exception exception, string exceptionMessage, string? errorCode, Type exceptionType)
    {
        return new ErrorResultModel(this.correlationContext.CorrelationContext.CorrelationId, errorCode)
        {
            Message = exceptionMessage,
            ErrorType = exceptionType.Name,
            ErrorDetails = exception.GetFullExceptionData(),
            Stacktrace = exception.StackTrace,
            Data = exception.Data,
            DataDetails = exception.GetSerializableDataDictionary(true)
        };
    }

    private ErrorResultModel GenerateGenericException()
    {
        return new ErrorResultModel(this.correlationContext.CorrelationContext.CorrelationId, this.config.GenericErrorCode)
        {
            Message = this.config.GenericErrorMessage
        };
    }

    private ErrorResultModel GenerateDetailedException(Exception exception, string exceptionMessage, string? errorCode, Type exceptionType)
    {
        return new ErrorResultModel(this.correlationContext.CorrelationContext.CorrelationId, errorCode)
        {
            Message = exceptionMessage,
            ErrorType = exceptionType.Name,
            Data = exception.Data,
            DataDetails = exception.GetSerializableDataDictionary(true)
        };
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
