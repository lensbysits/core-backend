using System;
using System.Collections.Generic;

namespace Lens.Core.Lib.Services
{
    public interface IUserContext
    {
        string Email { get; }
        Guid EmployeeId { get; }
        string Username { get; }

        T ClaimValue<T>(string claim);
        ICollection<T> ClaimValueAsCollection<T>(string claim);

        bool HasClaim(string claim);
        bool IsInRole(string role);
    }
}