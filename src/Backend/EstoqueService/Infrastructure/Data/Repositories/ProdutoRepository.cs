using EstoqueService.Core.Interfaces;
using EstoqueService.Models;
using EstoqueService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace EstoqueService.Infrastructure.Data.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Produto?> GetByIdAsync(int id)
        => await _context.Produtos.FindAsync(id);

    public async Task<(List<Produto> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
    {
        var query = _context.Produtos.AsNoTracking().OrderBy(p => p.Id);
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, totalCount);
    }

    public async Task AddAsync(Produto produto)
    {
        await _context.Produtos.AddAsync(produto);
    }

    public Task UpdateAsync(Produto produto)
    {
        _context.Produtos.Update(produto);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var produto = await GetByIdAsync(id);
        if (produto != null)
            _context.Produtos.Remove(produto);
    }

    public async Task<bool> ExistsAsync(int id)
        => await _context.Produtos.AnyAsync(p => p.Id == id);

    public async Task<int> GetTotalCountAsync()
        => await _context.Produtos.CountAsync();

    public async Task<Produto?> GetBySkuAsync(string sku)
        => await _context.Produtos.FirstOrDefaultAsync(p => p.CodigoSKU == sku);
}
