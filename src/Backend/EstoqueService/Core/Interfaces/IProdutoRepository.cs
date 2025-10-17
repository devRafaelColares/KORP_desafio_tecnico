using EstoqueService.Models;

namespace EstoqueService.Core.Interfaces;

public interface IProdutoRepository
{
    Task<Produto?> GetByIdAsync(int id);
    Task<(List<Produto> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
    Task AddAsync(Produto produto);
    Task UpdateAsync(Produto produto);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> GetTotalCountAsync();
    Task<Produto?> GetBySkuAsync(string sku);
}