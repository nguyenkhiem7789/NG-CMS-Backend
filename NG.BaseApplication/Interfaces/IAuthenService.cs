using NG.EnumDefine;

namespace NG.BaseApplication.Interfaces;

using BaseReadModels;

public interface IAuthenService
{
    Task SetLoginInfo(string key, AccountLoginInfo accountLoginInfo);
    Task ChangeLoginInfo(string key, AccountLoginInfo accountLoginInfo);
    Task<AccountLoginInfo> GetLoginInfo(string key);
    Task RemoveLoginInfo(string key);
    Task<bool> LoginInfoCheckExist(string key);
    Task<long> OtpLimit(string userId);
    Task OtpLimitReset(string userId);
    Task ReCaptChaLimitReset(string userName);
    Task SetOtpType(string clientId, OtpTypeEnum otpType);
    Task<bool> CheckOtpType(string clientId, OtpTypeEnum otpType);
}