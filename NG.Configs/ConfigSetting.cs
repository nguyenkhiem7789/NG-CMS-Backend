using NG.Extensions;

namespace NG.Configs;

using Extensions;

using Microsoft.Extensions.Configuration;

public static class ConfigSetting
{
    public static void Init(IConfiguration configuration)
    {
        Configs = new Dictionary<ConfigSettingEnum, object>();
        var keys = Enum.GetValues(typeof(ConfigSettingEnum));
        foreach (ConfigSettingEnum key in keys)
        {
            if (!Configs.ContainsKey(key))
            {
                string keyConfig = key.GetDisplayName();
                var config = configuration.GetSection(keyConfig);
                if (config != null)
                {
                    string valueConfig = config.Value;
                    object value = null;
                    var order = (ConfigSettingTypeEnum)key.GetOrder();
                    switch (order)
                    {
                        case ConfigSettingTypeEnum.Bool:
                            value = valueConfig.AsInt() == 1;
                            break;
                        case ConfigSettingTypeEnum.Int:
                            value = valueConfig.AsInt();
                            break;
                        default:
                            value = valueConfig;
                            break;
                    }
                    if(value != null) 
                        Configs.Add(key, value);
                }
            }
        }
    }

    public static IDictionary<ConfigSettingEnum, object> Configs;
}
