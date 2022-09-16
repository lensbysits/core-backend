namespace Lens.Core.Lib.Extensions;

public static class IntExtensions
{
    public static IEnumerable<int> GetDigits(this int value)
    {
        return value.ToString().AsEnumerable().Select(ch => (int)ch - '0');
    }

    public static IEnumerable<int> GetDigits(this long value)
    {
        return value.ToString().AsEnumerable().Select(ch => (int)ch - '0');
    }
}
