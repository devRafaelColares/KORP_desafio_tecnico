using EstoqueService.Core.Requests.Produto;
using EstoqueService.Core.Responses.Produto;
using EstoqueService.Core.Responses;

namespace EstoqueService.Core.Interfaces;

public interface IProdutoService
{
    Task<Response<ProdutoResponse>> CreateAsync(ProdutoCreateRequest request);
    Task<Response<ProdutoResponse>> GetByIdAsync(int id);
    Task<PagedResponse<List<ProdutoResponse>>> GetAllAsync(int pageNumber, int pageSize);
    Task<Response<ProdutoResponse>> UpdateAsync(int id, ProdutoUpdateRequest request);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<ProdutoResponse>> GetBySkuAsync(string sku);
}