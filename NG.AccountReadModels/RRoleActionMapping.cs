using ProtoBuf;

namespace NG.AccountReadModels;

[ProtoContract]
public class RRoleActionMapping : AccountBaseReadModel
{
    [ProtoMember(1)] public string RoleId { get; set; }
    [ProtoMember(2)] public string ActionId { get; set; }
    [ProtoMember(3)] public string Attributes { get; set; }
    [ProtoMember(4)] public bool IsAdministrator { get; set; }
}