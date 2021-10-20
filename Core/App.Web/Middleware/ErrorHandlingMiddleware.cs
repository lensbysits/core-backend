using Lens.Core.Lib.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lens.Core.App.Web
{
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
            if (_exceptionTypes.TryGetValue(exception.GetType().Name, out HttpStatusCode exceptionCode))
            {
                _logger.LogWarning(exceptionMessage); // log as warning the expected errors 
            }
            else
            {
                //probably unexpected. Log Error
                exceptionCode = HttpStatusCode.InternalServerError; // 500 if unexpected
                _logger.LogError(exception, "Unexpected error");
            }

            var exceptionData = GetExceptionData(exception);
            string result = _webHostEnvironment.IsDevelopment()
                ? JsonSerializer.Serialize(new { error = exceptionMessage, data = exceptionData, exception = exception.ToString()})
                : JsonSerializer.Serialize(new { error = exceptionMessage, data = exceptionData });
            
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

        /// <summary>
        /// Get exception data recursively including the inner exception data, too.
        /// </summary>
        private string GetExceptionData(Exception e)
        {
            string data = GetExceptionDataCollection(e);
            if (e.InnerException != null)
            {
                data = $"{data}{GetExceptionData(e.InnerException)}";
            }

            return data;
        }

        /// <summary>
        /// Get exception data collection specifying key and value
        /// </summary>
        private static string GetExceptionDataCollection(Exception e)
        {
            string data = string.Empty;
            if (e.Data != null && e.Data.Count > 0)
            {
                foreach (DictionaryEntry entry in e.Data)
                {
                    string newLine = !string.IsNullOrEmpty(data) ? Environment.NewLine : string.Empty;
                    data = $"{data}{newLine}Key: {entry.Key} Value: {entry.Value}";
                }
            }

            return data;
        }
        #endregion
    }
}
