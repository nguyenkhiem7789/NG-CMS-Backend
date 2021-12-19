using NG.AccountCommands.Commands;
using NG.BaseCommands;
using ProtoBuf;

namespace NG.AccountCommands;

[ProtoContract]
[ProtoInclude(100, typeof(RoleAddCommand))]
[ProtoInclude(200, typeof(RoleChangeCommand))]
[ProtoInclude(300, typeof(RoleActionDefineMappingChangeCommand))]
[ProtoInclude(400, typeof(RoleActionDefineMappingAdminChangeCommand))]
public class AccountBaseCommand : BaseCommand
{
    [ProtoMember(101)] public override string ObjectId { get; set; }
    [ProtoMember(102)] public override string ProcessUid { get; set; }
    [ProtoMember(103)] public override DateTime ProcessDate { get; set; }
    [ProtoMember(104)] public override string LoginUid { get; set; }
    [ProtoMember(105)] public override bool IsCache { get; set; }
}