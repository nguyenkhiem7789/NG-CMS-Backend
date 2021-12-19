using NG.EnumDefine;
using ProtoBuf;

namespace NG.AccountReadModels;

[ProtoContract]
public class RActionDefine : AccountBaseReadModel
{
    [ProtoMember(1)] public string Name { get; set; }
    [ProtoMember(2)] public string Group { get; set; }
    [ProtoMember(3)] public bool IsRoot { get; set; }
    [ProtoMember(4)] public StatusEnum Status { get; set; }
    [ProtoMember(5)] public new string Id { get; set; }
}