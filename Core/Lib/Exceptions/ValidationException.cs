namespace Lens.Core.Lib.Exceptions;

public class ValidationException : PublicException
{ 
    public ValidationException(string message) : base(message)
    {

    }

    public ValidationException() : base()
    {
    }

    public ValidationException(string errorMessage, string? errorCode = null) : base(errorMessage, errorCode)
    {
    }

    public ValidationException(string message, Exception innerException, string? errorCode = null) : base(message, innerException, errorCode)
    {
    }
}
