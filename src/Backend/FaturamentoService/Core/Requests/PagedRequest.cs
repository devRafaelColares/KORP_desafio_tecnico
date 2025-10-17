using FaturamentoService.Configurations;

namespace FaturamentoService.Core.Requests;
public abstract class PagedRequest
{
    public int PageNumber { get; set; } = ApiConstants.DefaultPageNumber;
    public int PageSize { get; set; } = ApiConstants.DefaultPageSize;
}