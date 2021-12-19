using ProtoBuf;

namespace NG.AccountCommands.Commands;

[ProtoContract]
public class ActionDefineAddCommand
{
    [ProtoMember(1)] public string Name { get; set; }
    [ProtoMember(2)] public string Group { get; set; }
    [ProtoMember(3)] public bool IsRoot { get; set; }
    [ProtoMember(4)] public string Id { get; set; }
}