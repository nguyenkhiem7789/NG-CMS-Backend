using NG.Common;

namespace NG.HttpClientBase;

using System.Net.Http.Headers;
using System.Text;
using System.Xml;

public class HttpClient : IHttpClient
{
    private readonly System.Net.Http.HttpClient _client;

    public HttpClient(System.Net.Http.HttpClient client)
    {
        _client = client;
    }
    
    public async Task<HttpResponseMessage> Get(string uri, string? authorizationToken = null, string authorizationMethod = "Bearer")
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
        if (authorizationToken != null)
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
        }

        var response = await _client.SendAsync(requestMessage);
        return response;
    }

    public async Task<HttpResponseMessage> Get(string uri, IDictionary<string, string>? headers)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
        if (headers?.Count > 0)
        {
            foreach (var header in headers)
            {
                requestMessage.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await _client.SendAsync(requestMessage);
        return response;
    }

    public async Task<HttpResponseMessage> Post<T>(string uri, T item, string? authorizationToken = null, string authorizationMethod = "Bearer")
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
        string json = Serialize.JsonSerializeObject(item);
        requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
        if (authorizationToken != null)
        {
            requestMessage.Headers.Authorization = string.IsNullOrEmpty(authorizationMethod)
                ? new AuthenticationHeaderValue(authorizationToken)
                : new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
        }

        var response = await _client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
        return response;
    }

    public Task<HttpResponseMessage> Post(string uri, MultipartFormDataContent content)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> Post(string uri, FormUrlEncodedContent content)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> Post(string uri, string content)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> Post(string uri)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> Post(string uri, IDictionary<string, string> headers)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> Post<T>(string uri, T item, IDictionary<string, string> headers)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> Delete(string uri, string authorizationToken = null, string authorizationMethod = "Bearer")
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> Put<T>(string uri, T item, string authorizationToken = null, string authorizationMethod = "Bearer")
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> Post(string uri, StringContent content)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> Post(string uri, string action, XmlDocument doc)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> PostSoap(string uri, string action, string content)
    {
        throw new NotImplementedException();
    }
}