namespace Lens.Core.Lib.Utilities;

public static class StringUtilities
{
    private static readonly Random randomizer;

    static StringUtilities()
    {
        randomizer = new Random();
    }

    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ-_";
        var randomstring = new string(Enumerable.Repeat(chars, length)
            .Select(s => s[randomizer.Next(s.Length)]).ToArray());
        return randomstring;
    }
}
