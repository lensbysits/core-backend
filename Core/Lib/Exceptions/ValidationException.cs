using System;

namespace Lens.Core.Lib.Exceptions
{
    public class ValidationException : System.ComponentModel.DataAnnotations.ValidationException
    {
        public ValidationException(string message) : base(message)
        {

        }

        public ValidationException() : base()
        {
        }

        public ValidationException(System.ComponentModel.DataAnnotations.ValidationResult validationResult, System.ComponentModel.DataAnnotations.ValidationAttribute? validatingAttribute, object? value) : base(validationResult, validatingAttribute, value)
        {
        }

        public ValidationException(string? errorMessage, System.ComponentModel.DataAnnotations.ValidationAttribute? validatingAttribute, object? value) : base(errorMessage, validatingAttribute, value)
        {
        }

        public ValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
