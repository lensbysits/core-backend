namespace Lens.Core.Lib.Extensions;

public static class DateTimeExtensions
{

    /// <summary>
    /// Gets the quarter of the year
    /// </summary>
    public static int? GetQuarter(this DateTime value)
    {
        return ((value.Month - 3) % 12) / 4;
    }

    /// <summary>
    /// Gets the quarter of the year
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int? GetQuarter(this DateTime? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        return GetQuarter(value.Value);
    }

    /// <summary>
    /// Returns the maximum date of the 2
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static DateTime Max(this DateTime value1, DateTime value2)
    {
        return (value1.ToUniversalTime() > value2.ToUniversalTime()) ? value1 : value2;
    }

    /// <summary>
    /// Returns the maximum date of the 2. 
    /// When 1 of the 2 is null, return the one who is not null.
    /// When both are null, return null.
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static DateTime? Max(this DateTime? value1, DateTime? value2)
    {
        if (!value1.HasValue)
        {
            if (!value2.HasValue)
            {
                return null;
            }
            else
            {
                return value2;
            }
        }
        else
        {
            if (!value2.HasValue)
            {
                return value1;
            }
            else
            {
                return Max(value1.Value, value2.Value);
            }
        }
    }

    /// <summary>
    /// Returns the minimum date of the 2
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static DateTime Min(this DateTime value1, DateTime value2)
    {
        return (value1.ToUniversalTime() < value2.ToUniversalTime()) ? value1 : value2;
    }

    /// <summary>
    /// Returns the minimum date of the 2. 
    /// When 1 of the 2 is null, return the one who is not null.
    /// When both are null, return null.
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static DateTime? Min(this DateTime? value1, DateTime? value2)
    {
        if (!value1.HasValue)
        {
            if (!value2.HasValue)
            {
                return null;
            }
            else
            {
                return value2;
            }
        }
        else
        {
            if (!value2.HasValue)
            {
                return value1;
            }
            else
            {
                return Min(value1.Value, value2.Value);
            }
        }
    }
}
