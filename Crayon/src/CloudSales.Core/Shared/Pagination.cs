namespace CloudSales.Core.Shared;

public class Pagination
{
    public const int MinPageSize = 10;

    public int Offset { get; }
    public int PageNo { get; }
    public int PageSize { get; }
    public bool IsValid { get; }

    public Pagination(int pageNo, int pageSize)
    {
        if (pageNo <= 0)
            return;
        if (pageSize < MinPageSize)
            return;

        PageNo = pageNo;
        PageSize = pageSize;
        Offset = pageNo > 1 ? (pageNo - 1) * pageSize : 0;
        IsValid = true;
    }
}