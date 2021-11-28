namespace NG.HttpClientBase;

public interface IAuthenToken
{
    Task<string> GetToken();
    Task SaveToken(string sessionId, string token, TimeSpan timeOut);
}