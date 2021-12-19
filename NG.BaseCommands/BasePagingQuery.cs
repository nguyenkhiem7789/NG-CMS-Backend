using ProtoBuf;

namespace NG.BaseCommands;

[ProtoContract]
public abstract class BasePagingQuery : BaseCommand
{
    [ProtoMember(1)] public abstract int PageIndex { get; set; }
    [ProtoMember(2)] public abstract int PageSize { get; set; }
    public int Offset => PageIndex * PageSize;
    
    protected BasePagingQuery() {}

    protected BasePagingQuery(string processUid, int pageIndex = 0, int pageSize = 30) : base(processUid)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
}