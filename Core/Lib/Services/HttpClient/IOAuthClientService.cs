namespace Lens.Core.Lib.Services;

public interface IOAuthClientService
{
    Task<string?> GetBearerToken(string clientName);
}