using CoreLib.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;

namespace CoreApp.Web.Services
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _accessor;
        private ClaimsPrincipal UserClaims => _accessor.HttpContext?.User;

        public UserContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        //TODO: Move to business specific extension methods
        public Guid EmployeeId => ClaimValue<Guid>("employeeId");
        
        public string Username => UserClaims?.Identity?.Name ?? "anonymous";
        
        public string Email => ClaimValue<string>("email");
        
        public T ClaimValue<T>(string claim)
        {
            var value = UserClaims?.Claims?.FirstOrDefault(c => c.Type == claim)?.Value;
            if (value == null) return default;

            TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));
            return (T)tc.ConvertFrom(value);
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
}
