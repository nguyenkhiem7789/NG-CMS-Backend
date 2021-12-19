using NG.EventBusModels;

namespace NG.EventBus;

public interface IRabbitMqConnection
{
    void Notify(string exchange, IEvent message);
    (string, string)[] Send((string, string) topic, IEvent[] messages);
    void Notify(string exchange, IEvent[] messages);
    Task SubscribeAsync(string[] topics, Func<EventBusMessage, Task> processFunc);
    Task SubscribeNotifyAsync(string[] exchangesTrigger, Func<EventBusMessage, Task> processFunc);
    void RegisterExchangeAndQueue(string exchange, string[] routingKeys, string queues);
    void RegisterExchangeTrigger(string[] exchanges);
}