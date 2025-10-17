using EstoqueService.Configurations;

namespace EstoqueService.Core.Requests;
public abstract class PagedRequest
{
    public int PageNumber { get; set; } = ApiConstants.DefaultPageNumber;
    public int PageSize { get; set; } = ApiConstants.DefaultPageSize;
}