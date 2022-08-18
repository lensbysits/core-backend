namespace Lens.Core.Lib.Services;

public interface IUserContext
{
    string Email { get; }
    Guid EmployeeId { get; }
    string Username { get; }

    T ClaimValue<T>(string claim);
    IEnumerable<T> ClaimValues<T>(string claim);
    bool HasClaim(string claim);
    bool IsInRole(string role);
}