namespace NG.HttpClientBase;

using System.Xml;
public interface IHttpClient
{
    Task<HttpResponseMessage> Get(string uri, string? authorizationToken = null, string authorizationMethod = "Bearer");
    Task<HttpResponseMessage> Get(string uri, IDictionary<string, string>? headers);

    Task<HttpResponseMessage> Post<T>(string uri, T item, string? authorizationToken = null,
        string authorizationMethod = "Bearer");
    Task<HttpResponseMessage> Post(string uri, MultipartFormDataContent content);
    Task<HttpResponseMessage> Post(string uri, FormUrlEncodedContent content);
    Task<HttpResponseMessage> Post(string uri, string content);
    Task<HttpResponseMessage> Post(string uri);
    Task<HttpResponseMessage> Post(string uri, IDictionary<string, string> headers);
    Task<HttpResponseMessage> Post<T>(string uri, T item, IDictionary<string, string> headers);
    Task<HttpResponseMessage> Delete(string uri, string? authorizationToken = null, string authorizationMethod = "Bearer");
    Task<HttpResponseMessage> Put<T>(string uri, T item, string? authorizationToken = null, string authorizationMethod = "Bearer");
    Task<HttpResponseMessage> Post(string uri, StringContent content);
    Task<HttpResponseMessage> Post(string uri, string action, XmlDocument doc);
    Task<HttpResponseMessage> PostSoap(string uri, string action, string content);
}