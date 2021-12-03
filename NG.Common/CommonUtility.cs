namespace NG.Common;

using System.Text.RegularExpressions;

public class CommonUtility
{
    public static string UserGetShortName(string fullName)
    {
        if (string.IsNullOrEmpty(fullName)) return "";
        var shortName = "";
        Regex regex = new Regex(@"\b(\w)");
        var matches = regex.Matches(fullName);
        if (matches.Count > 1)
        {
            shortName = matches[0].Value + "" + matches[matches.Count - 1].Value;
        }
        else
        {
            shortName = string.Join("", matches.Select(s => s.Value));
        }

        return shortName.ToUpper();
    }

    public static string GenerateGuid()
    {
        return Guid.NewGuid().ToString("N");
    }
    
}