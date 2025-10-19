using FaturamentoService.Core.Requests.Notas;
using FaturamentoService.Core.Responses.Notas;
using FaturamentoService.Core.Responses;

namespace FaturamentoService.Core.Interfaces;

public interface INotaFiscalService
{
    Task<Response<NotaFiscalResponse>> CreateAsync(NotaFiscalCreateRequest request);
    Task<Response<NotaFiscalResponse>> GetByIdAsync(int id);
    Task<Response<ImprimirNotaFiscalResponse>> PrintAsync(int id, ImprimirNotaFiscalRequest? request = null);
    Task<Response<object>> DeleteAsync(int id);
    Task<PagedResponse<List<NotaFiscalResponse>>> GetAllAsync(int pageNumber, int pageSize);
}