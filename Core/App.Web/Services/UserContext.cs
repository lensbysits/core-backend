using Lens.Core.Lib.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;

namespace Lens.Core.App.Web.Services
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
