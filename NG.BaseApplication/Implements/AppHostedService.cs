using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NG.BaseApplication.Interfaces;
using NG.EventBus;
using Serilog;
using ILogger = Serilog.ILogger;

namespace NG.BaseApplication.Implements;

public class AppHostedService : IAppHostedService
{
    private readonly ILogger<AppHostedService> _logger;
    
    private IServiceProvider Service { get; }

    private AppHostedService(IServiceProvider services, ILogger<AppHostedService> logger)
    {
        Service = services;
        _logger = logger;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosted Service is starting.");
        using var scope = Service.CreateScope();
        var scopedProcessingService = scope.ServiceProvider.GetService(typeof(IEventProcessor));
        if (scopedProcessingService != null)
        {
            IEventProcessor eventProcessor = (IEventProcessor)scopedProcessingService;
            eventProcessor.Register();
            eventProcessor.Start();
        } 
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosted service is stopping.");
        return Task.CompletedTask;
    }
}