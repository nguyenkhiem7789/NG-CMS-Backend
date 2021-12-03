namespace NG.Configs;

using System.ComponentModel.DataAnnotations;

public enum ConfigSettingEnum
{
    AppName,
    AppVersion,
    [Display(Name = "MessageSerializeType", Order = (int)ConfigSettingTypeEnum.Int)] 
    MessageSerializeType = 2,
    [Display(Name = "JwtTokens:Key")] JwtTokenKey = 3,
    [Display(Name = "JwtTokens:Issuer")] JwtTokensIssuer = 4,
    [Display(Name = "JwtTokens:Audience")] JwtTokensAudience = 5,
    
    [Display(Name = "JwtTokens:Authority")] JwtTokensAuthority,
    
    [Display(Name = "IsTracking", Order = (int)ConfigSettingTypeEnum.Bool)]
    IsTracking = 7,
    
    [Display(Name = "BusMaxRetry", Order = (int)ConfigSettingTypeEnum.Int)]
    BusMaxRetry = 8,
    
    HttpPort,
    HttpType,
    DataProtectionRedisKey,
    IsWeb,
    LoginExpiresTime,
    RefreshTokenExpiresTime,
    IsMobileApi,
    
    [Display(Name = "RedisHostIps")] RedisHostIps,
    [Display(Name = "RedisPassword")] RedisPassword,
    [Display(Name = "CookieDomain")] CookieDomain,
    
}