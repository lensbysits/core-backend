using Lens.Core.Lib.Services;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.Security.Claims;

namespace Lens.Core.App.Web.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _accessor;
    private ClaimsPrincipal? UserClaims => _accessor.HttpContext?.User;

    public UserContext(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string Username
    {
        get
        {
            var name = UserClaims?.Identity?.Name;
            if (name == null)
            {
                name = UserClaims?.FindFirst(ClaimTypes.Name)?.Value;
            }
            return name ?? "anonymous";
        }
    }

    public string? Email => ClaimValue<string>("email");
    
    public T? ClaimValue<T>(string claim)
    {
        var value = UserClaims?.Claims?.FirstOrDefault(c => c.Type == claim)?.Value;
        if (value == null) return default;

        TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));
        return (T?)tc.ConvertFrom(value);
    }

    public ICollection<T?> ClaimValues<T>(string claim)
    {
        if(UserClaims == null || UserClaims.Claims == null)
        {
            return new List<T?>();
        }

        TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));
        var values = UserClaims.Claims
            .Where(c => c.Type == claim)
            .Select(c => (T?)tc.ConvertFrom(c.Value))
            .Where(v => v != null)
            .ToList();

        return values;
    }

    public bool HasClaim(string claim)
    {
        return UserClaims != null && UserClaims.Claims.Any(c => c.Type == claim);
    }

    public bool IsInRole(string role)
    {
        return UserClaims != null && UserClaims.IsInRole(role);
    }    
}
