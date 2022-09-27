using System.Net;

namespace Lens.Core.Lib.Exceptions
{
    /// <summary>
    /// Defines an exception that is allowed to be returned to the front-end.
    /// Exceptions that inherit this class are not anonymized by the exception handling middleware (e.g. validation exceptions)
    /// </summary>
    public class PublicException : Exception
    {
        /// <summary>
        /// Contains the error code that can be used for multilingual exceptions in the frontend.
        /// </summary>
        public string? ErrorCode { get; private set; }

        /// <summary>
        /// Defines the statuscode that is returned by the middleware when this exception occurs.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; protected set; }

        public PublicException() : base()
        {
            this.HttpStatusCode = HttpStatusCode.BadRequest;
        }

        public PublicException(string errorMessage, string? errorCode = null) : base(errorMessage)
        {
            this.HttpStatusCode = HttpStatusCode.BadRequest;
            this.ErrorCode = errorCode;
        }

        public PublicException(string errorMessage, Exception innerException, string? errorCode = null) : base(errorMessage, innerException)
        {
            this.ErrorCode = errorCode;
        }
    }
}
