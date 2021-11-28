namespace NG.HttpClientBase;

using NG.EnumDefine;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

public static class HttpClientRegister
{
    public static IServiceCollection RegisterHttpClient(this IServiceCollection services)
    {
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(30)
            });
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(90);
        
        HttpClientHandler clientHandler = new HttpClientHandler();
        
        services.AddHttpClient(HttpClientNameEnum.Default.ToString(), client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");
            client.Timeout = TimeSpan.FromSeconds(60);
        });
        
        services.AddHttpClient(HttpClientNameEnum.Retry.ToString(), client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");
                client.Timeout = TimeSpan.FromSeconds(60);
            })
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(timeoutPolicy);
        
        return services;
    } 
    
}