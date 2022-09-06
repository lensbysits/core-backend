using System.Text.Json;
using System.Text.RegularExpressions;

namespace Lens.Core.Lib.Extensions;

public static class JsonElementExtensions
{
    private static Regex regexGuid = new Regex(@"^[({]?[a-fA-F0-9]{8}[-]?([a-fA-F0-9]{4}[-]?){3}[a-fA-F0-9]{12}[})]?$", RegexOptions.Compiled);
    private static Regex regexDateTimeISO8601 = new Regex(@"^([\+-]?\d{4}(?!\d{2}\b))((-)((0[1-9]|1[0-2])(\3([12]\d|0[1-9]|3[01]))?|W([0-4]\d|5[0-2])(-[1-7])?|(00[1-9]|0[1-9]\d|[12]\d{2}|3([0-5]\d|6[1-6])))([T\s]((([01]\d|2[0-3])((:?)[0-5]\d)?|24\:?00)([\.,]\d+(?!:))?)?(\17[0-5]\d([\.,]\d+)?)?([zZ]|([\+-])([01]\d|2[0-3]):?([0-5]\d)?)?)?)?$", RegexOptions.Compiled);
    private static Regex regexDateOnly = new Regex(@"^[0-9]{4}-[0-9]{2}-[0-9]{2}$", RegexOptions.Compiled);

    public static object? GetValueAsObject(this JsonElement jsonElement)
    {
        var rawVal = jsonElement.GetRawText();

        if (rawVal == null || string.IsNullOrWhiteSpace(rawVal))
        {
            return null;
        }

        rawVal = rawVal.Trim('#').Trim();

        try
        {
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.Number:
                    // check for decimal
                    // check for long
                    // check for int
                    if (ShouldBeDecimal(rawVal))
                    {
                        return jsonElement.GetDecimal();
                    }
                    else if (ShouldBeWholeNumber(rawVal))
                    {
                        var val = jsonElement.GetInt64();

                        if (val < Int32.MaxValue)
                        {
                            return (int)val;
                        }
                    }
                    return rawVal;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.String:
                    // check for Guid
                    // check for Date
                    // check for Date Time (UTC vs timezone???)
                    // check for string
                    var stringVal = jsonElement.GetString() ?? string.Empty;
                    if (regexGuid.IsMatch(stringVal))
                    {
                        return jsonElement.GetGuid();
                    }
                    else if (regexDateOnly.IsMatch(stringVal))
                    {
                        return jsonElement.GetDateTime();
                    }
                    else if (regexDateTimeISO8601.IsMatch(stringVal))
                    {
                        return jsonElement.GetDateTime();
                    }
                    return jsonElement.GetString();
                case JsonValueKind.Array:
                    return jsonElement.EnumerateArray()
                        .Select(o => GetValueAsObject(o))
                        .ToArray();
                case JsonValueKind.Object:
                case JsonValueKind.Undefined:
                default:
                    return rawVal;
            }
        }
        catch (System.FormatException e)
        {
            throw new System.FormatException($"Invalid format conversion for {jsonElement.ValueKind} with raw value: {rawVal}", e);
        }
    }

    private static bool ShouldBeDecimal(this string input)
    {
        if (String.IsNullOrEmpty(input))
        {
            return false;
        }

        int len = input.Length;
        bool containsDecimalDelimiter = false;
        for (int i = 0; i < len; i++)
        {
            if ((input[i] ^ '0') > 9 && input[i] != '-')
            {
                if (input[i] == '.' || input[i] == ',')
                {
                    containsDecimalDelimiter = true;
                }
                else
                {
                    return false;
                }
            }
        }
        return containsDecimalDelimiter;
    }

    private static bool ShouldBeWholeNumber(this string input)
    {
        if (String.IsNullOrEmpty(input))
        {
            return false;
        }

        int len = input.Length;
        for (int i = 0; i < len; i++)
        {
            if ((input[i] ^ '0') > 9 && input[i] != '-')
                return false;
        }
        return true;
    }
}
