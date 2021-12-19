namespace NG.BaseReadModels;

public class RefSqlPaging
{
    public int PageIndex { get; private set; }
    public int PageSize { get; set; }
    public int Offset => PageIndex * PageSize;
    public int TotalRow { get; set; }
    public RefSqlPaging(int pageIndex) : this(pageIndex, 30) {}

    public RefSqlPaging(int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        if (PageSize <= 0) PageSize = 30;
        if (PageSize > 100000) PageSize = 100000;
        if (PageIndex <= 0) PageIndex = 0;
    }
    
}