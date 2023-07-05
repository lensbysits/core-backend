namespace Lens.Core.Data.EF.Providers;

public class RawSqlProvider
{
    private static RawSqlProvider instance = null!;
    private Dictionary<string, string> createCommands;
    private List<string> dropCommands;

    private RawSqlProvider()
    {
        this.createCommands = new();
        this.dropCommands = new();
    }

    public static RawSqlProvider Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new RawSqlProvider();
            }

            return instance;
        }
    }

    public void AddCreateCommand(string fileName, string contents)
    {
        if (!this.createCommands.ContainsKey(fileName))
        {
            this.createCommands.Add(fileName, contents);
        }
    }

    public void AddDropCommand(string contents)
    {
        if (!this.dropCommands.Contains(contents))
        {
            this.dropCommands.Add(contents);
        }
    }

    public ICollection<string> GetSqlCommands()
    {
        var commands = this.dropCommands
                        .Concat(this.createCommands.Select(c => c.Value))
                        .SelectMany(v => v.Split("GO", StringSplitOptions.RemoveEmptyEntries))
                        .ToList();
        return commands;
    }
}
