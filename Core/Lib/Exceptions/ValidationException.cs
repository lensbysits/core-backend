using System.Net;

namespace Lens.Core.Lib.Exceptions;

public class ValidationException : PublicException
{
    public ValidationException(string message) : base(message)
    {
        this.HttpStatusCode = HttpStatusCode.UnprocessableEntity;
    }

    public ValidationException() : base()
    {
        this.HttpStatusCode = HttpStatusCode.UnprocessableEntity;
    }

    public ValidationException(string errorMessage, string? errorCode = null) : base(errorMessage, errorCode)
    {
        this.HttpStatusCode = HttpStatusCode.UnprocessableEntity;
    }

    public ValidationException(string message, Exception innerException, string? errorCode = null) : base(message, innerException, errorCode)
    {
        this.HttpStatusCode = HttpStatusCode.UnprocessableEntity;
    }
}
