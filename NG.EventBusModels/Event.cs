﻿namespace NG.EventBusModels;

public abstract class Event : IEvent
{
    public abstract string EventId { get; set; }
    public abstract int DelayTime { get; set; }
    public abstract int Version { get; set; }
    public abstract EventTypeEnum EventType { get; }
    public abstract bool IsTrigger { get; set; }
    public abstract string ObjectId { get; set; }
    public abstract string ProcessUid { get; set; }
    public abstract DateTime ProcessDate { get; set; }
    public DateTime ProcessDateUtc => ProcessDate.ToUniversalTime();
    public abstract string LoginUid { get; set; }
}