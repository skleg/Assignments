namespace CloudSales.Core.Shared;

public record EntityPage<T>(List<T> Items, int TotalCount, int PageNo, int PageSize);
