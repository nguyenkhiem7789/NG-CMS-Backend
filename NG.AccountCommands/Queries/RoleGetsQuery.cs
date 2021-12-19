using NG.BaseCommands;
using ProtoBuf;

namespace NG.AccountCommands.Queries;

[ProtoContract]
public class RoleGetsQuery : BasePagingQuery
{
    [ProtoMember(1)] public string Keyword { get; set; }
    [ProtoMember(100)] public override string ObjectId { get; set; }
    [ProtoMember(200)] public override string ProcessUid { get; set; }
    [ProtoMember(300)] public override DateTime ProcessDate { get; set; }
    [ProtoMember(400)] public override string LoginUid { get; set; }
    [ProtoMember(500)] public override bool IsCache { get; set; }
    [ProtoMember(600)] public override int PageIndex { get; set; }
    [ProtoMember(700)] public override int PageSize { get; set; }
}

[ProtoContract]
public class RoleByIdQuery : AccountBaseCommand
{
    
}

[ProtoContract]
public class RoleAutoCompleteQuery : AccountBaseCommand
{
    [ProtoMember(1)] public string Keyword { get; set; }
}