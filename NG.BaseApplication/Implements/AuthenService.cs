using NG.BaseApplication.Interfaces;
using NG.BaseReadModels;
using NG.Cache;
using NG.EnumDefine;

namespace NG.BaseApplication.Implements;

public class AuthenService : IAuthenService
{
    private readonly IRedisStorage _redisStorage;

    public AuthenService(IRedisStorage redisStorage)
    {
        _redisStorage = redisStorage;
    }
    
    public async Task SetLoginInfo(string key, AccountLoginInfo accountLoginInfo)
    {
        TimeSpan timeOut = accountLoginInfo.InitDate.AddMinutes(accountLoginInfo.MinuteExpire) - DateTime.Now;
        if (timeOut.TotalMilliseconds <= 0)
        {
            timeOut = new TimeSpan(0, accountLoginInfo.MinuteExpire, 0);
        }

        await _redisStorage.StringSet(key, accountLoginInfo, timeOut);
    }

    public async Task ChangeLoginInfo(string key, AccountLoginInfo accountLoginInfo)
    {
        TimeSpan timeOut = accountLoginInfo.InitDate.AddMinutes(accountLoginInfo.MinuteExpire) - DateTime.Now;
        if (timeOut.TotalMilliseconds <= 0)
        {
            timeOut = new TimeSpan(0, accountLoginInfo.MinuteExpire, 0);
        }

        await _redisStorage.StringSet(key, accountLoginInfo, timeOut);
    }

    public async Task<AccountLoginInfo> GetLoginInfo(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;
        return await _redisStorage.StringGet<AccountLoginInfo>(key);
    }

    public async Task RemoveLoginInfo(string key)
    {
        await _redisStorage.KeyDelete(key);
    }

    public async Task<bool> LoginInfoCheckExist(string key)
    {
        return await _redisStorage.KeyExist(key);
    }

    public async Task<long> OtpLimit(string userId)
    {
        string key = $"OtpLimit_{userId}";
        long retry = await _redisStorage.StringIncrement(key, 1, new TimeSpan(1, 0, 0));
        return retry;
    }

    public async Task OtpLimitReset(string userId)
    {
        string key = $"OtpLimit_{userId}";
        await _redisStorage.KeyDelete(key);
    }

    public async Task ReCaptChaLimitReset(string userName)
    {
        string key = $"OtpLimit_{userName}";
        await _redisStorage.KeyDelete(key);
    }

    public async Task SetOtpType(string clientId, OtpTypeEnum otpType)
    {
        string key = $"OtpType_{clientId}";
        await _redisStorage.StringSet(key, (int)otpType, new TimeSpan(1, 0, 0));
    }

    public async Task<bool> CheckOtpType(string clientId, OtpTypeEnum otpType)
    {
        string key = $"OtpType_{clientId}";
        var value = await _redisStorage.StringGet<int>(key);
        return value == (int)otpType;
    }
}