namespace NG.EventBus;

public interface IEventProcessor
{
    Task Register();
    Task Handle(EventBusMessage payload);
    Task Start();
}