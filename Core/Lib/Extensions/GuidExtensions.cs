﻿namespace Lens.Core.Lib.Extensions;

public static class GuidExtensions
{
    /// <summary>
    /// From NHibernate: https://github.com/nhibernate/nhibernate-core/blob/3087d48640eb64f573c0011e9a9b1567afe6adde/src/NHibernate/Id/GuidCombGenerator.cs
    /// The <c>comb</c> algorithm is designed to make the use of GUIDs as Primary Keys, Foreign Keys, 
    /// and Indexes nearly as efficient as ints.
    /// See: <a href="http://www.informit.com/articles/article.asp?p=25862">article</a>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Guid ToSequentialId(this Guid source)
    {
        byte[] guidArray = source.ToByteArray();

        DateTime baseDate = new DateTime(1900, 1, 1);
        DateTime now = DateTime.Now;

        // Get the days and milliseconds which will be used to build the byte string 
        TimeSpan days = new TimeSpan(now.Ticks - baseDate.Ticks);
        TimeSpan msecs = now.TimeOfDay;

        // Convert to a byte array 
        // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
        byte[] daysArray = BitConverter.GetBytes(days.Days);
        byte[] msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

        // Reverse the bytes to match SQL Servers ordering 
        Array.Reverse(daysArray);
        Array.Reverse(msecsArray);

        // Copy the bytes into the guid 
        Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
        Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

        return new Guid(guidArray);
    }
}
