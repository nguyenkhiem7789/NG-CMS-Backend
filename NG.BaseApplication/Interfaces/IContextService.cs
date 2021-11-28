using NG.BaseReadModels;

namespace NG.BaseApplication.Interfaces;

public interface IContextService
{
    Task<string> GetIp();
    Task<bool> IsAuthenticated();

    Task<(string, int)> CreateToken(string userName, bool remember, AccountLoginInfo accountLoginInfo,
        string oldRefreshToken);

    Task<string> GetUserName();
    Task<string> SessionKeyGet();
    Task SessionKeySet(string sessionId);
    Task Logout();
    Task<AccountLoginInfo> UserInfo();
    Task RemoveLoginInfo();
    string LanguageId { get; }
    string LanguageDefaultId { get; }
    string LanguageIdBackend { get; }
    string Token { get; }
    string DateFormat { get; }
    string DateTimeFormat { get; }
    string NumberFormat { get; }

    string ClientId();

    Task<long> OtpLimit(string userId, bool isSend);
    Task<long> OtpLimitByIP(bool isSend, string ip);
    Task OtpVerifySuccessCountByUser(string userId, bool isSend);
    Task OtpVerifySuccessCountByIp(bool isSend, string ip);

    void LogError(Exception exception, string message);
    Task VerifyOtp(bool isCMS);

    Task OtpIncrement();
    T GetService<T>();
}