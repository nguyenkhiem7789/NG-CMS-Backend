namespace NG.Grpc.Service.Extensions;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Configuration;

public class ServiceBinderWithServiceResolutionFromServiceCollection : ServiceBinder
{
    private readonly IServiceCollection services;

    public ServiceBinderWithServiceResolutionFromServiceCollection(IServiceCollection services)
    {
        this.services = services;
    }

    public override IList<object> GetMetadata(MethodInfo method, Type contractType, Type serviceType)
    {
        var resolvedServiceType = serviceType;
        if (serviceType.IsInterface)
            resolvedServiceType = services.SingleOrDefault(x => x.ServiceType == serviceType)?.ImplementationType ?? serviceType;

        return base.GetMetadata(method, contractType, resolvedServiceType);
    }
}