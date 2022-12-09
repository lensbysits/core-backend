namespace Lens.Core.Data.EF.Configuration
{
    public class AuditSettings
    {
        /// <summary>
        /// Gets or sets a value indicating that we should add a command interceptor to wrap each query in a user context.
        /// </summary>
        public bool EnableRlsSupport { get; set; }
    }
}
