using System.ComponentModel.DataAnnotations;

namespace Lens.Core.Lib.Exceptions;

public class ValidationException : System.ComponentModel.DataAnnotations.ValidationException
{
    public ValidationException(string message) : base(message)
    {

    }

    public ValidationException() : base()
    {
    }

    public ValidationException(ValidationResult validationResult, ValidationAttribute? validatingAttribute, object? value) : base(validationResult, validatingAttribute, value)
    {
    }

    public ValidationException(string? errorMessage, ValidationAttribute? validatingAttribute, object? value) : base(errorMessage, validatingAttribute, value)
    {
    }

    public ValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
