using NG.Common;
using NG.EnumDefine;
using ProtoBuf;

namespace NG.AccountReadModels;

[ProtoContract]
public class RRole : AccountBaseReadModel
{
    [ProtoMember(1)]
    public string Name { get; set; }
    [ProtoMember(2)]
    public StatusEnum Status { get; set; }
    [ProtoMember(3)]
    public string DepartmentId { get; set; }
    [ProtoMember(4)]
    public bool IsAdministrator { get; set; }
    [ProtoMember(5)]
    public string GroupAdmin { get; set; }

    public string[] GroupAdmins
    {
        get
        {
            if (!string.IsNullOrEmpty(GroupAdmin))
            {
                return Serialize.JsonDeserializeObject<string[]>(GroupAdmin);
            }

            return null;
        }
    }
}