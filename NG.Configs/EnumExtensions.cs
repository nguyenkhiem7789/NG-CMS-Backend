namespace NG.Configs;

using System.ComponentModel.DataAnnotations;
using System.Reflection;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum? enumValue)
    {
        try
        {
            if (enumValue == null) return string.Empty;
            var configName = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName();
            if (string.IsNullOrEmpty(configName)) return enumValue.ToString();
            return configName;
        }
        catch (Exception e)
        {
            return enumValue?.ToString() ?? e.Message;
        }
    }

    public static int GetOrder(this Enum enumValue)
    {
        var orderConfig = enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()?
            .GetOrder().GetValueOrDefault();
        return orderConfig.GetValueOrDefault(0);
    }

    public static string GetConfig(this ConfigSettingEnum enumValue)
    {
        if (ConfigSetting.Configs.TryGetValue(enumValue, out var configValue))
            return configValue.ToString() ?? "";
        return string.Empty;
    }
}