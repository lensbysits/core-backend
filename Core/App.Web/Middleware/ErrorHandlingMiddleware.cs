using Lens.Core.Lib.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Lens.Core.Lib.Extensions;
using CorrelationId.Abstractions;

namespace Lens.Core.App.Web
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICorrelationContextAccessor correlationContext;
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
            {typeof(InvalidCastException).Name, HttpStatusCode.BadRequest},
            {typeof(InvalidOperationException).Name, HttpStatusCode.BadRequest},
            {typeof(ValidationException).Name, HttpStatusCode.UnprocessableEntity},
            {typeof(ArgumentException).Name, HttpStatusCode.UnprocessableEntity},
            {typeof(ArgumentNullException).Name, HttpStatusCode.UnprocessableEntity},
            {typeof(InvalidDataException).Name, HttpStatusCode.UnprocessableEntity},
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
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var exceptionMessage = GetExceptionMessage(exception);
            var exceptionType = exception.GetType().Name;
            var serializedData = JsonSerializer.Serialize(exception.Data);
            var correlationId = correlationContext.CorrelationContext.CorrelationId;

            if (_exceptionTypes.TryGetValue(exception.GetType().Name, out HttpStatusCode exceptionCode))
            {
                // log as warning the expected exceptions
                _logger.LogWarning(exception, $"Exception: {exceptionType} (expected): HTTP Status: {(int)exceptionCode} ({exceptionCode}): {exceptionMessage} (data: {serializedData})"); 
            }
            else
            {
                // log as error the unexpected exceptions
                exceptionCode = HttpStatusCode.InternalServerError; 
                _logger.LogError(exception, $"Exception: {exceptionType} (unexpected): HTTP Status: {(int)exceptionCode} ({exceptionCode}): {exceptionMessage} (data: {serializedData})");
            }

            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)exceptionCode;
            string result = string.Empty;

            if (_webHostEnvironment.IsDevelopment())
            {
                result = JsonSerializer.Serialize(new ErrorResponse
                {
                    IsError = true,
                    Message = exceptionMessage,
                    ErrorType = exceptionType,
                    ErrorDetails = exception.GetFullExceptionData(),
                    Stacktrace = exception.StackTrace,
                    CorrelationId = correlationId
                });
            }
            else
            {
                result = JsonSerializer.Serialize(new ErrorResponse
                {
                    IsError = true,
                    Message = exceptionMessage,
                    ErrorType = exceptionType,
                    CorrelationId = correlationId
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
}
