namespace Lens.Core.Data.EF.Configuration;

public class MigrationSettings
{
    public bool BreakOnMigrationException { get; set; }
    
    public bool EnableRawSqlDebug { get; set; }
}
