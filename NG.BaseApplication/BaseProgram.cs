using NG.BaseApplication.Implements;
using NG.Cache;
using NG.Configs;
using NG.Extensions;
using NG.Grpc.Service.Extensions;
using NG.HttpClientBase;
using NG.NGGrpcClient;

namespace NG.BaseApplication;

using BaseApplication.Interfaces;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

public class BaseProgram
{
    public static void Run(string[] args, Func<IServiceCollection, IServiceCollection> registerServiceFunc,
        Action<WebApplication> registerRoutingUrl)
    {
        try
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate:
                    "[{Level} {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] {Message} {Properties}{NewLine}{Exception}")
                .WriteTo.File(
                    @"log\log.txt",
                    fileSizeLimitBytes: 1_000_000,
                    rollOnFileSizeLimit: true,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1),
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();
            
            var builder = WebApplication.CreateBuilder(args);
            ConfigSetting.Init(builder.Configuration);
            int httpPort = ConfigSettingEnum.HttpPort.GetConfig().AsInt();
            if (httpPort <= 0)
            {
                throw new Exception("Port binding invalid");
            }

            int httpType = ConfigSettingEnum.HttpType.GetConfig().AsInt();
            if (httpType != 2)
            {
                httpType = 1;
            }
            
            builder.Host.UseContentRoot(Directory.GetCurrentDirectory());
            builder.WebHost.UseSerilog();
            builder.WebHost.UseKestrel(options =>
            {
                options.AllowSynchronousIO = true;
                if (httpType == 1)
                {
                    options.ListenAnyIP(httpPort,
                        listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
                }
                else if (httpType == 2)
                {
                    options.ListenAnyIP(httpPort,
                        listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
                }
            });
            
            builder.Services.AddLogging(p => p.AddConfiguration(builder.Configuration).AddSerilog());
            
            // Config Redis
            var connection = new Redisconnection(ConfigSettingEnum.RedisHostIps.GetConfig(),
                ConfigSettingEnum.RedisPassword.GetConfig());
            connection.MakeConnection().Wait();
            builder.Services.AddSingleton(provider => connection);
            string redisCacheEnvironment = ConfigSettingEnum.DataProtectionRedisKey.GetConfig();
            builder.Services.AddSingleton<IRedisStorage>(
                provider => new RedisStorage(connection, redisCacheEnvironment));
            
            // Http
            builder.Services.RegisterHttpClient();
            builder.Services.RegisterGrpcClient();
            builder.Services.ConfigCodeFirstGrpc();
            
            var isWeb = ConfigSettingEnum.IsWeb.GetConfig().AsInt() == 1;
            if (isWeb)
            {
                builder.Services.AddControllersWithViews()
                    .AddFluentValidation(p => p.DisableDataAnnotationsValidation = true);
            }
            else
            {
                builder.Services.AddControllers();
            }
            
            // when you need access to the HttpContext inside a service.
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<IContextService>(p =>
            {
                IHttpContextAccessor httpContextAccessor = p.GetRequiredService<IHttpContextAccessor>();
                return new ContextService(httpContextAccessor, p);
            });
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, $"Host terminated unexpectedly: {ex.Message}");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}