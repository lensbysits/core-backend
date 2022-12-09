namespace Lens.Core.Data.EF.RowLevelSecurity.Providers
{
    public interface IRowLevelSecurityIdentityProvider
    {
        bool HasAuthenticatedUser { get; }

        /// <summary>
        /// Gets the unique id that is used to execute the SQL query in the user's context.
        /// </summary>
        /// <returns></returns>
        public string GetUserRowLevelSecurityIdentity();
    }
}
