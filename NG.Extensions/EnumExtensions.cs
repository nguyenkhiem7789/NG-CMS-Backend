using System.ComponentModel.DataAnnotations;
using System.Reflection;
namespace NG.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        try
        {
            if (enumValue == null) return string.Empty;
            var configName = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName();
            if (string.IsNullOrEmpty(configName))
            {
                return enumValue.ToString();
            }

            return configName;
        }
        catch (Exception e)
        {
            return enumValue.ToString();
        }
    }

    public static int GetOrder(this Enum enumValue)
    {
        var orderConfig = enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()?
            .GetCustomAttribute<DisplayAttribute>()?
            .GetOrder().GetValueOrDefault();
        return orderConfig.GetValueOrDefault(0);
    }
} 