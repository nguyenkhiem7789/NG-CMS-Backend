using NG.Common;
using ProtoBuf;

namespace NG.EventBus;

[ProtoContract]
public class EventBusMessage
{
    [ProtoMember(1)] public string MessageId { get; set; }
    [ProtoMember(2)] public string Delay { get; set; }
    [ProtoMember(3)] public TimeSpan TimeToLive { get; set; }
    [ProtoMember(4)] public string CorrelationId { get; set; }
    [ProtoMember(5)] public SerializeTypeEnum SerializeType { get; set; }
    [ProtoMember(6)] public int Version { get; set; }
    [ProtoMember(7)] public string BodyType { get; set; }
    [ProtoMember(8)] public DateTime CreatedDate { get; set; }
    [ProtoMember(9)] public DateTime ProcessDate { get; set; }
    [ProtoMember(10)] public string TopicName { get; set; }
    [ProtoMember(11)] public byte[] ProtobufBody { get; set; }
    [ProtoMember(12)] public string JsonBody { get; set; }
    [ProtoMember(13)] public byte[] ByteBody { get; set; }

    public EventBusMessage() {}

    public EventBusMessage(string topicName, string messageId, object body, SerializeTypeEnum serializeType)
    {
        
    }

    public object? Obj
    {
        get
        {
            Type type = Type.GetType(BodyType);
            switch (SerializeType)
            {
               case SerializeTypeEnum.Json:
                   return Serialize.JsonDeserializeObject(JsonBody, type);
               case SerializeTypeEnum.Protobuf:
                   return Serialize.ProtoBufDeserialize(ProtobufBody, type);
               case SerializeTypeEnum.Bype:
                   return ByteBody;
               default:
                   return null;
            }
        }
    }

    public byte[] ToMessage()
    {
        return Serialize.ProtoBufSerialize(this);
    }

    public EventBusMessage Clone()
    {
        return new EventBusMessage()
        {
            BodyType = this.BodyType,
            CorrelationId = this.CorrelationId,
            CreatedDate = this.CreatedDate,
            Delay = this.Delay,
            MessageId = this.MessageId,
            ProcessDate = this.ProcessDate,
            SerializeType = this.SerializeType,
            TimeToLive = this.TimeToLive,
            Version = this.Version,
            TopicName = this.TopicName,
            ProtobufBody = this.ProtobufBody,
            JsonBody = this.JsonBody,
            ByteBody = this.ByteBody,
        };
    }

    public static EventBusMessage CreateMessageFromQueue(byte[] bytes)
    {
        return Serialize.ProtoBufDeserialize<EventBusMessage>(bytes);
    }

    public enum SerializeTypeEnum
    {
        Json = 1,
        Protobuf = 2,
        String = 3,
        Bype = 4
    }

    public enum EventStatusEnum
    {
        New = 0,
        Success = 1,
        Fail = -1,
        Retry = 2
    }

}