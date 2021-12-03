using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Routing;
using Microsoft.IdentityModel.Tokens;
using NG.BaseReadModels;
using NG.Common;
using NG.Configs;
using NG.Extensions;

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
    private AccountLoginInfo _userInfo;

    public ContextService(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
    {
        _serviceProvider = httpContextAccessor?.HttpContext?.RequestServices ?? serviceProvider;
        _httpContextAccessor = httpContextAccessor;
        _logger = GetService<ILogger<ContextService>>();
        _authenService = GetService<IAuthenService>();
    }

    private string _sessionKey;

    private string GetCurrentIpAddress()
    {
        var result = string.Empty;
        try
        {
            if (_httpContextAccessor?.HttpContext?.Request.Headers != null)
            {
                //the X-Forwarded-For (XFF) HTTP header field is a de facto standard for identifying the originating IP address of a client
                //connecting to a web server through an HTTP proxy or load balancer
                var forwardedHttpHeaderKey = "X-FORWARDED-FOR";
                var forwardedHeader = _httpContextAccessor.HttpContext.Request.Headers[forwardedHttpHeaderKey];
                if (!string.IsNullOrEmpty(forwardedHeader))
                    result = forwardedHeader.FirstOrDefault();
            }
            //if this header not exists try get connection remote IP address
            if (string.IsNullOrEmpty(result) &&
                _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress != null)
                result = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }
        catch
        {
            return string.Empty;
        }
        if (result != null && result.Equals("::1", StringComparison.InvariantCultureIgnoreCase))
            result = "127.0.0.1";
        if (!string.IsNullOrEmpty(result))
            result = result.Split(':').FirstOrDefault();

        return result;
    }

    public async Task<string> GetIp()
    {
        string ip = GetCurrentIpAddress();
        return await Task.FromResult(ip);
    }

    public async Task<bool> IsAuthenticated()
    {
        var isAuthenticated = _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true;
        return await Task.FromResult(isAuthenticated);
    }

    public async Task<(string, int)> CreateToken(string userName, bool remember, AccountLoginInfo accountLoginInfo, string oldRefreshToken)
    {
        if (!string.IsNullOrEmpty(oldRefreshToken))
        {
            await _authenService.RemoveLoginInfo(oldRefreshToken);
        }

        string uniqueName = $"SESSIONID{CommonUtility.GenerateGuid()}";
        _sessionKey = uniqueName;
        var minuteExpire = remember
            ? ConfigSettingEnum.LoginExpiresTime.GetConfig().AsInt() + 60
            : ConfigSettingEnum.LoginExpiresTime.GetConfig().AsInt();
        accountLoginInfo.MinuteExpire = minuteExpire;
        await _authenService.SetLoginInfo(uniqueName, accountLoginInfo);
        _userInfo = accountLoginInfo;
        if (accountLoginInfo.OtpVerify)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(ConfigSettingEnum.JwtTokenKey.GetConfig());
            string uniqueNameKey = JwtRegisteredClaimNames.UniqueName;
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(SessionCode, _sessionKey),
                    new Claim("MinuteExpire", minuteExpire.ToString()),
                    new Claim(uniqueNameKey, userName)
                }),
                Expires = Extension.GetCurrentDate().AddMinutes(minuteExpire),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
            };
            var token = tokenHandler.CreateToken(tokenDescription);
            string tokenValue = tokenHandler.WriteToken(token);
            return (tokenValue, minuteExpire);
        }

        return (string.Empty, 2);
    }

    public async Task<string> GetUserName()
    {
        string uniqueNameKey = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        var userName = _httpContextAccessor?.HttpContext?.User?.FindFirst(uniqueNameKey)?.Value;
        return await Task.FromResult(userName);
    }

    public async Task<string> SessionKeyGet()
    {
        if (string.IsNullOrEmpty(_sessionKey))
        {
            _sessionKey = _httpContextAccessor?.HttpContext?.User?.FindFirst(SessionCode)?.Value;
        }

        return await Task.FromResult(_sessionKey);
    }

    public Task SessionKeySet(string sessionId)
    {
        _sessionKey = sessionId;
        return Task.CompletedTask;
    }

    public async Task Logout()
    {
        await RemoveLoginInfo();
    }

    public async Task<AccountLoginInfo> UserInfo()
    {
        if (_userInfo == null)
        {
            string key = await SessionKeyGet();
            _userInfo = await _authenService.GetLoginInfo(key);
        }

        return _userInfo;
    }

    public async Task RemoveLoginInfo()
    {
        string key = await SessionKeyGet();
        await _authenService.RemoveLoginInfo(key);
    }

    public string LanguageId => _httpContextAccessor?.HttpContext?.GetRouteValue(Lang).AsString();
    public string LanguageDefaultId { get; }
    public string LanguageIdBackend => _httpContextAccessor?.HttpContext?.Request.Headers["language"];
    public string Token { get; }
    public string DateFormat => _httpContextAccessor?.HttpContext?.Request?.Headers["DateFormat"];
    public string DateTimeFormat => DateFormat + " HH:mm";
    public string NumberFormat => _httpContextAccessor?.HttpContext?.Request.Headers["NumberFormat"].AsString("0,0.##");
    public string ClientId()
    {
        if (ConfigSettingEnum.IsMobileApi.GetConfig().AsInt() == 1)
        {
            return _httpContextAccessor?.HttpContext?.Request.Headers["MobileClientId"];
        }

        var clientId = _httpContextAccessor?.HttpContext?.Request.Cookies[SessionCode];
        if (string.IsNullOrEmpty(clientId))
        {
            clientId = $"VNNCLI{CommonUtility.GenerateGuid()}";
            CookieOptions cookieOptions = new CookieOptions()
            {
                Path = "/",
                SameSite = SameSiteMode.Lax,
                HttpOnly = true,
                Expires = DateTimeOffset.Now.AddYears(100)
            };
            string cookieDomain = ConfigSettingEnum.CookieDomain.GetConfig();
            if (cookieDomain.Length > 0)
            {
                cookieOptions.Domain = cookieDomain;
            }
            _httpContextAccessor?.HttpContext?.Response.Cookies.Append(SessionCode, clientId, cookieOptions);
        }

        return clientId;
    }

    public async Task<long> OtpLimit(string userId, bool isSend)
    {
        return await _authenService.OtpLimit($"OtpVerifyFailCountByUser_{isSend}_{userId}");
    }

    public async Task<long> OtpLimitByIP(bool isSend, string ip)
    {
        return await _authenService.OtpLimit($"OtpVerifyFailCountByIp_{isSend}_{ip}");
    }

    public async Task OtpVerifySuccessCountByUser(string userId, bool isSend)
    {
        await _authenService.OtpLimitReset($"OtpVerifyFailCountByUser_{isSend}_{userId}");
    }

    public async Task OtpVerifySuccessCountByIp(bool isSend, string ip)
    {
        await _authenService.OtpLimitReset($"OtpVerifyFailCountByIp_{isSend}_{ip}");
    }

    public void LogError(Exception exception, string message)
    {
        _logger.LogError(exception, message);
    }

    public async Task VerifyOtp(bool isCMS)
    {
        string key = await SessionKeyGet();
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