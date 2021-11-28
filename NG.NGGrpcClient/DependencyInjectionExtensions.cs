using Microsoft.Extensions.DependencyInjection;

namespace NG.NGGrpcClient;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection RegisterGrpcClient(this IServiceCollection services)
    {
        return services;
    }
}