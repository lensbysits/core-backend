namespace Lens.Core.Lib.Configuration;

public class ApiExceptionHandlingConfig
{
    /// <summary>
    /// Disables the anonymization of non public exception.
    /// WARNING: This leaves your api vulnerable to malicious users that try to gain insight in how the backend works.
    /// </summary>
    /// <returns>True if non public exceptions are anonymized, otherwise false.</returns>
    public bool DisableEnhancedApiProtection { get; init; }

    public string GenericErrorMessage { get; init; }
    public string? GenericErrorCode { get; init; }

    public ApiExceptionHandlingConfig()
    {
        this.GenericErrorMessage = "Unexpected error occurred";
    }
}
