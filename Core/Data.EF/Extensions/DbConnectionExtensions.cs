using System.Data.Common;

namespace Lens.Core.Data.EF.Extensions;

public static class DbConnectionExtensions
{
    public static string ConnectionStringWithoutPassword(this DbConnection connection, string passwordPropertyName = "Password", string replacementText = "<<hidden_password>>")
    {
        var connectionString = connection.ConnectionString;
        var passwordStartIndex = connectionString.IndexOf(passwordPropertyName);
        if(passwordStartIndex == -1)
        {
            return connectionString;
        }
        else
        {
            passwordStartIndex += (passwordPropertyName.Length + 1);
        }

        var passwordEndIndex = connectionString.IndexOf(';', passwordStartIndex);
        var password = connectionString.Substring(passwordStartIndex, passwordEndIndex - passwordStartIndex);
        return connectionString.Replace(password, replacementText);
    }
}
