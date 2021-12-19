using NG.EnumDefine;
using ProtoBuf;

namespace NG.AccountCommands.Commands;

[ProtoContract]
public class RoleAddCommand : AccountBaseCommand
{
    [ProtoMember(1)] public string Name { get; set; }
    [ProtoMember(2)] public StatusEnum Status { get; set; }
    [ProtoMember(3)] public string DepartmentId { get; set; }
    [ProtoMember(4)] public bool IsAdministractor { get; set; }
    [ProtoMember(5)] public string Code { get; set; }
    [ProtoMember(6)] public string[] GroupAdmins { get; set; }
}

[ProtoContract]
public class RoleChangeCommand : AccountBaseCommand
{
    [ProtoMember(1)] public string Id { get; set; }
    [ProtoMember(2)] public string Name { get; set; }
    [ProtoMember(3)] public StatusEnum Status { get; set; }
    [ProtoMember(4)] public string DepartmentId { get; set; }
    [ProtoMember(5)] public bool IsAdministractor { get; set; }
    [ProtoMember(6)] public string[] GroupAdmins { get; set; }
}

[ProtoContract]
public class RoleActionDefineMappingChangeCommand : AccountBaseCommand
{
    [ProtoMember(1)] public string RoleId { get; set; }
    [ProtoMember(2)] public string ActionDefineId { get; set; }
    [ProtoMember(3)] public string Attributes { get; set; }
    [ProtoMember(4)] public bool IsAdministractor { get; set; }
}

[ProtoContract]
public class RoleActionDefineMappingAdminChangeCommand : AccountBaseCommand
{
    [ProtoMember(1)] public string RoleId { get; set; }
    [ProtoMember(2)] public string ActionDefineId { get; set; }
    [ProtoMember(3)] public string Attributes { get; set; }
    [ProtoMember(4)] public bool IsAdministractor { get; set; }
}

[Flags]
public enum ErrorTypeEnum
{
    RoleId = 1,
    RoleName = 2
}