using CloudSales.Core.Shared;

namespace CloudSales.Api.Contracts;

public record PaginatedResponse<TResponse>(IEnumerable<TResponse> Items, int PageNo, int PageSize, int TotalCount) where TResponse : notnull
{
    internal static PaginatedResponse<TResponse> CreateFrom<TDto>(EntityPage<TDto> dto, Func<TDto, TResponse> mapper) where TDto : notnull
    {
        return new PaginatedResponse<TResponse>(dto.Items.Select(mapper), dto.PageNo, dto.PageSize, dto.TotalCount);
    }
    internal static PaginatedResponse<TResponse> CreateFrom<TDto>(IEnumerable<TDto> items, Func<TDto, TResponse> mapper) where TDto : notnull
    {
        return new PaginatedResponse<TResponse>(items.Select(mapper), 1, items.Count(), items.Count());
    }
}
