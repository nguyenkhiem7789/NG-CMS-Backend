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
    
}