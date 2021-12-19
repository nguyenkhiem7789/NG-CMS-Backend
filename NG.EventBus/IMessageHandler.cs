using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using NG.EventBusModels;
using NG.Extensions;
using ILogger = DnsClient.Internal.ILogger;

namespace NG.EventBus;

public interface IMessageHandler
{
    void EventAdd(IEvent @event);
    void EventAdd(IEnumerable<IEvent> @events);
    void EventRemove(IEvent @event);
    void EventRemove(string id);
    IEvent[] EventsGet(bool isTrigger = false);
}

public interface IEventHandler<in TI> : IMessageHandler where TI : IEvent
{
    Task Handle(TI message, string topic);
}

public class BaseEventHandler
{
    private List<IEvent> _events;
    protected readonly ILogger<BaseEventHandler> _logger;

    public BaseEventHandler(ILogger<BaseEventHandler> logger)
    {
        _logger = logger;
    }

    public void EventAdd(IEvent @event)
    {
        _events ??= new List<IEvent>();
        _events.Add(@event);
    }

    public void EventAdd(IEnumerable<IEvent> @events)
    {
        _events ??= new List<IEvent>();
        _events.AddRange(@events);
    }

    public void EventRemove(IEvent @event)
    {
        _events.Remove(@event);
    }

    public void EventRemove(string id)
    {
        if (_events is null) return;
        var events = _events.Where(p => p.EventId == id);
        foreach (var @event in events)
        {
            EventRemove(@event);
        }
    }

    public void EventRemoveAll(bool isTrigger = false)
    {
        _events.RemoveAll(p => p.IsTrigger == isTrigger);
    }

    public IEvent[] EventsGet(bool isTrigger = false)
    {
        return _events?.Where(p => p.IsTrigger == isTrigger).ToArray();
    }

    protected void LogError(string message)
    {
        _logger.LogError(message);
    }

    protected void LogError(IEnumerable<string> message)
    {
        _logger.LogError(message.ToArray().AsArrayJoin());
    }
}