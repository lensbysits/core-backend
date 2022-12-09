using Lens.Core.Data.EF.RowLevelSecurity.Providers;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace Lens.Core.Data.EF.RowLevelSecurity.Interceptors
{
    public class RowLevelSecurityInterceptor : DbCommandInterceptor
    {
        private readonly IRowLevelSecurityIdentityProvider rlsIdentityProvider;

        public RowLevelSecurityInterceptor(IRowLevelSecurityIdentityProvider? rlsIdentityProvider)
        {
            this.rlsIdentityProvider = rlsIdentityProvider ?? throw new ArgumentNullException(nameof(rlsIdentityProvider));
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            AddUserContextToQuery(command);
            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            AddUserContextToQuery(command);
            return new ValueTask<InterceptionResult<DbDataReader>>(result);
        }

        private void AddUserContextToQuery(DbCommand command)
        {
            if (rlsIdentityProvider.HasAuthenticatedUser)
            {
                
                var principleId = rlsIdentityProvider.GetUserRowLevelSecurityIdentity();
                if(Regex.IsMatch(principleId, "^(?!.*--)[a-zA-Z0-9-]+$"))
                {
                    command.CommandText = $"EXECUTE AS USER = '{principleId}'; {command.CommandText}; REVERT;";
                }
                else
                {
                    throw new InvalidOperationException("provided principle id contains invalid characters.");
                }
            }
        }
    }
}
