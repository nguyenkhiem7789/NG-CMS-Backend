using NG.BaseReadModels;
using ProtoBuf;

namespace NG.AccountReadModels;

[ProtoContract]
public class AccountBaseReadModel : BaseReadModel
{
    [ProtoMember(1)] public override long NumericalOrder { get; set; }
    [ProtoMember(2)] public override string Id { get; set; }
    [ProtoMember(3)] public override string code { get; set; }
    [ProtoMember(4)] public override string CreatedUid { get; set; }
    [ProtoMember(5)] public override DateTime CreatedDate { get; set; }
    [ProtoMember(6)] public override DateTime CreatedDateUtc { get; set; }
    [ProtoMember(7)] public override string UpdatedUid { get; set; }
    [ProtoMember(8)] public override DateTime UpdatedDate { get; set; }
    [ProtoMember(9)] public override DateTime UpdatedDateUtc { get; set; }
    [ProtoMember(10)] public override int Version { get; set; }
    [ProtoMember(11)] public override string LoginUid { get; set; }
}