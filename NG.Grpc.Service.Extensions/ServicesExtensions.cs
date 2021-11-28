namespace NG.Grpc.Service.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Grpc.Server;

public static class ServicesExtensions
{
    public static IServiceCollection ConfigCodeFirstGrpc(this IServiceCollection services)
    {
        services.AddCodeFirstGrpc(config =>
        {
            config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
        });
        services.TryAddSingleton(BinderConfiguration.Create(binder: new ServiceBinderWithServiceResolutionFromServiceCollection(services)));
        services.AddCodeFirstGrpcReflection();
        return services;
    }
}