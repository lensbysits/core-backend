using System;

namespace CoreLib.Services
{
    public interface IUserContext
    {
        string Email { get; }
        Guid EmployeeId { get; }
        string Username { get; }

        T ClaimValue<T>(string claim);
        bool HasClaim(string claim);
        bool IsInRole(string role);
    }
}