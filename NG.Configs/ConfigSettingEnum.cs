namespace NG.Configs;

using System.ComponentModel.DataAnnotations;

public enum ConfigSettingEnum
{
    AppName,
    AppVersion,
    [Display(Name = "MessageSerializeType", Order = (int)ConfigSettingTypeEnum.Int)] 
    MessageSerializeType = 2,
    
    [Display(Name = "IsTracking", Order = (int)ConfigSettingTypeEnum.Bool)]
    IsTracking = 7,
    
    [Display(Name = "BusMaxRetry", Order = (int)ConfigSettingTypeEnum.Int)]
    BusMaxRetry = 8,
    
    HttpPort,
    HttpType,
    DataProtectionRedisKey,
    IsWeb,
    
    [Display(Name = "RedisHostIps")] RedisHostIps,
    [Display(Name = "RedisPassword")] RedisPassword,
    
}