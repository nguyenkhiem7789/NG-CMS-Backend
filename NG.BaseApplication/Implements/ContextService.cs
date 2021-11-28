using NG.BaseReadModels;

namespace NG.BaseApplication.Implements;

using BaseApplication.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;

public class ContextService : IContextService
{
    private const string Lang = "lang";
    public const string SessionCode = "VNNSS";
    private const string Authorization = "Authorization";

    private readonly ILogger<ContextService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthenService _authenService;
    private readonly IServiceProvider _serviceProvider;

    public ContextService(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
    {
        _serviceProvider = httpContextAccessor?.HttpContext?.RequestServices ?? serviceProvider;
        _httpContextAccessor = httpContextAccessor;
        _logger = GetService<ILogger<ContextService>>();
        _authenService = GetService<IAuthenService>();
    }

    private string GetCurrentIpAddress()
    {
        
    }

    public Task<string> GetIp()
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsAuthenticated()
    {
        throw new NotImplementedException();
    }

    public Task<(string, int)> CreateToken(string userName, bool remember, AccountLoginInfo accountLoginInfo, string oldRefreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetUserName()
    {
        throw new NotImplementedException();
    }

    public Task<string> SessionKeyGet()
    {
        throw new NotImplementedException();
    }

    public Task SessionKeySet(string sessionId)
    {
        throw new NotImplementedException();
    }

    public Task Logout()
    {
        throw new NotImplementedException();
    }

    public Task<AccountLoginInfo> UserInfo()
    {
        throw new NotImplementedException();
    }

    public Task RemoveLoginInfo()
    {
        throw new NotImplementedException();
    }

    public string LanguageId { get; }
    public string LanguageDefaultId { get; }
    public string LanguageIdBackend { get; }
    public string Token { get; }
    public string DateFormat { get; }
    public string DateTimeFormat { get; }
    public string NumberFormat { get; }
    public string ClientId()
    {
        throw new NotImplementedException();
    }

    public Task<long> OtpLimit(string userId, bool isSend)
    {
        throw new NotImplementedException();
    }

    public Task<long> OtpLimitByIP(bool isSend, string ip)
    {
        throw new NotImplementedException();
    }

    public Task OtpVerifySuccessCountByUser(string userId, bool isSend)
    {
        throw new NotImplementedException();
    }

    public Task OtpVerifySuccessCountByIp(bool isSend, string ip)
    {
        throw new NotImplementedException();
    }

    public void LogError(Exception exception, string message)
    {
        throw new NotImplementedException();
    }

    public Task VerifyOtp(bool isCMS)
    {
        throw new NotImplementedException();
    }

    public Task OtpIncrement()
    {
        throw new NotImplementedException();
    }

    public T GetService<T>()
    {
        throw new NotImplementedException();
    }
}