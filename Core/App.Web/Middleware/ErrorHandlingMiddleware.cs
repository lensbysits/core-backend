using Lens.Core.Lib.Exceptions;
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
    private static readonly Dictionary<string, HttpStatusCode> _exceptionTypes = new()
    {
        {typeof(NotFoundException).Name, HttpStatusCode.NotFound},
        {typeof(BadRequestException).Name, HttpStatusCode.BadRequest},
        {typeof(NotAllowedException).Name, HttpStatusCode.MethodNotAllowed},
        {typeof(UnauthorizedException).Name, HttpStatusCode.Unauthorized},
        {typeof(ForbiddenException).Name, HttpStatusCode.Forbidden},
        {typeof(FormatException).Name, HttpStatusCode.BadRequest},
        {typeof(DivideByZeroException).Name, HttpStatusCode.BadRequest},
        {typeof(NullReferenceException).Name, HttpStatusCode.BadRequest},
    };
    protected readonly ILogger _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
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
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var exceptionMessage = GetExceptionMessage(exception);
        
        var basicExceptionResult = JsonSerializer.Serialize(new 
        { 
            message = exceptionMessage, data = exception.Data 
        });
        var fullExceptionResult = JsonSerializer.Serialize(new 
        { 
            message = exceptionMessage, data = exception.Data, exception = exception.ToString() 
        });

        if (_exceptionTypes.TryGetValue(exception.GetType().Name, out HttpStatusCode exceptionCode))
        {
            // log as warning the expected exceptions
            _logger.LogWarning($"Expected exception ({(int)exceptionCode})-{exceptionCode}: {basicExceptionResult}"); 
        }
        else
        {
            // log as error the unexpected exceptions
            exceptionCode = HttpStatusCode.InternalServerError; 
            _logger.LogError(exception, $"Unexpected exception ({(int)exceptionCode})-{exceptionCode}");
        }

        string result = _webHostEnvironment.IsDevelopment() ? fullExceptionResult : basicExceptionResult;
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)exceptionCode;
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
