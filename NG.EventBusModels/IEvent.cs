namespace NG.EventBusModels;

public interface IEvent
{
    string EventId { get; set; }
    int DelayTime { get; set; }
    int Version { get; set; }
    EventTypeEnum EventType { get; }
    public bool IsTrigger { get; set; }
}