using Microsoft.Extensions.Logging;
using NG.BaseApplication.Interfaces;
using NG.BaseCommands;
using NG.Configs;
using NG.EnumDefine;
using NG.EventBus;
using NG.EventBusModels;
using NG.Extensions;
using NG.NGException;
using ILogger = DnsClient.Internal.ILogger;

namespace NG.BaseApplication.Implements;

public class BaseService : BaseEventHandler, IBaseService
{
    protected readonly IContextService ContextService;
    private readonly IRabbitMqConnection _rabbitMqConnection;
    private new readonly ILogger<BaseService> _logger;
    
    public BaseService(IContextService contextService, ILogger<BaseService> logger) : base(logger)
    {
        ContextService = contextService;
        _rabbitMqConnection = ContextService.GetService<IRabbitMqConnection>();
        _logger = logger;
    }

    protected async Task<BaseCommandResponse> ProcessCommand(Func<BaseCommandResponse, Task> processFunc)
    {
        BaseCommandResponse response = new BaseCommandResponse();
        try
        {
            await processFunc(response);
            NotifyEvent();
            Trigger();
        }
        catch (NgException e)
        {
            response.SetFail(e.Message);
            LogError(e, e.Message);
        }
        catch (Exception e)
        {
            if (e.Data.Contains(Constant.ErrorCodeEnum) &&
                Enum.TryParse(e.Data[Constant.ErrorCodeEnum].AsString(), out ErrorCodeEnum _))
            {
                response.SetFail((ErrorCodeEnum)e.Data[Constant.ErrorCodeEnum]);
            }
            else
            {
                if (ConfigSettingEnum.IsDevEnvironment.GetConfig().AsInt() == 1)
                {
                    response.SetFail(e.Message);
                }
                else
                {
                    response.SetFail(ErrorCodeEnum.InternalExceptions);
                }
            }
            LogError(e, e.Message);
        }

        return response;
    }

    protected async Task<BaseCommandResponse<T>> ProcessCommand<T>(Func<BaseCommandResponse<T>, Task> processFunc)
    {
        BaseCommandResponse<T> response = new BaseCommandResponse<T>();
        try
        {
            await processFunc(response);
            NotifyEvent();
            Trigger();
        }
        catch (NgException e)
        {
            response.SetFail(e.Message);
            LogError(e, e.Message);
        }
        catch (Exception e)
        {
            if (e.Data.Contains(Constant.ErrorCodeEnum) &&
                Enum.TryParse(e.Data[Constant.ErrorCodeEnum].AsString(), out ErrorCodeEnum _))
            {
                response.SetFail((ErrorCodeEnum)e.Data[Constant.ErrorCodeEnum]);
            }
            else
            {
                if (ConfigSettingEnum.IsDevEnvironment.GetConfig().AsInt() == 1)
                {
                    response.SetFail(e.Message);
                }
                else
                {
                    response.SetFail(EnumDefine.ErrorCodeEnum.InternalExceptions);
                }
            }

            LogError(e, e.Message);
        }

        return response;
    }

    protected async Task ProcessEvent(Func<Task> processFunc)
    {
        try
        {
            await processFunc();
            NotifyEvent();
            Trigger();
        }
        catch (NgException e)
        {
            LogError(e, e.Message);
        }
        catch (Exception e)
        {
            if (e.Data.Contains(Constant.ErrorCodeEnum))
            {
                var t = e.Data[Constant.ErrorCodeEnum];
                LogError(t.ToString());
            }
            LogError(e);
        }
    }

    protected void NotifyEvent()
    {
        var events = EventsGet();
        if (!(events?.Length > 0)) return;
        _rabbitMqConnection.Send(
            (ConfigSettingEnum.RabitMqExChange.GetConfig(), ConfigSettingEnum.RabitMqRoutingRoot.GetConfig()), events);
        EventRemoveAll();
    }

    protected void Notify(IEvent @event)
    {
        if (@event != null)
        {
            _rabbitMqConnection.Notify(ConfigSettingEnum.RabitMqExChangeNotify.GetConfig(), @event);
        }
    }

    protected void Trigger()
    {
        var events = EventsGet(true);
        if(!(events?.Length > 0)) return;
        _rabbitMqConnection.Notify(ConfigSettingEnum.RabitMqExChangeTrigger.GetConfig(), events);
    }
    
    protected void LogError(Exception exception, string message)
    {
        _logger.LogError(exception, message);
    }

    protected void LogError(Exception exception)
    {
        _logger.LogError(exception, exception.Message);
    }

    protected void LogInformation(Exception exception, string message)
    {
        _logger.LogInformation(exception, message);
    }

    protected void LogInformation(string message)
    {
        _logger.LogInformation(message);
    }

    protected void LogWarning(Exception exception, string message)
    {
        _logger.LogWarning(exception, message);
    }

    protected void LogWarning(string message)
    {
        _logger.LogWarning(message);
    }
}