using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NG.Extensions;


public static class Extension
{

    public static int AsInt(this object? item, int defaultInt = default(int))
    {
        if (item == null) return defaultInt;
        if (!int.TryParse(item.ToString(), out var result)) return defaultInt;
        return result;
    }

    public static int AsEnumToInt(this object? item, int defaultInt = default(int))
    {
        if (item == null) return defaultInt;
        return (int)item;
    }

    public static long AsEnumToLong(this object? item, long defaultLong = default(long))
    {
        if (item == null) return defaultLong;
        return (long)item;
    }

    public static string AsEmpty(this object? item)
    {
        return item == null ? string.Empty : item.ToString().Trim();
    }

    public static DateTime GetCurrentDate()
    {
        return DateTime.Now;
    }

    public static DateTime GetCurrentDateUtc()
    {
        return DateTime.Now;
    }

    public static string AsString(this object? item, string defaultString = default(string))
    {
        if (item == null || item.Equals(DBNull.Value)) return defaultString;
        var value = item.ToString().Trim();
        return string.IsNullOrEmpty(value) ? defaultString : value;
    }

    public static string AsArrayJoin(this string[] strings)
    {
        if (strings?.Length > 0)
        {
            return string.Join(",", strings);
        }

        return string.Empty;
    }

    public static string AsArrayJoin(this List<string> strings)
    {
        if (strings?.Count > 0)
        {
            return string.Join(",", strings);
        }

        return string.Empty;
    }

    public static long AsUnixTimeStamp(this DateTime item)
    {
        try
        {
            return (long)item.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
        catch (Exception)
        {
            return (long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
    
    public static long AsLong(this object item, long defaultInt = default(long))
    {
        if (item == null)
            return defaultInt;

        if (!long.TryParse(item.ToString(), out var result))
            return defaultInt;

        return result;
    }
}