using FaturamentoService.Core.Interfaces;
using FaturamentoService.Infrastructure.Data.Context;
using FaturamentoService.Models;
using Microsoft.EntityFrameworkCore;

namespace FaturamentoService.Infrastructure.Data.Repositories;

public class NotaFiscalRepository : INotaFiscalRepository
{
    private readonly AppDbContext _context;
    public NotaFiscalRepository(AppDbContext context) => _context = context;

    public async Task AddAsync(NotaFiscal nota)
    {
        await _context.NotasFiscais.AddAsync(nota);
    }

    public async Task<NotaFiscal?> GetByIdAsync(int id)
    {
        return await _context.NotasFiscais
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public Task DeleteAsync(NotaFiscal nota)
    {
        _context.NotasFiscais.Remove(nota);
        return Task.CompletedTask;
    }

    public async Task<(List<NotaFiscal> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
    {
        var query = _context.NotasFiscais.Include(n => n.Itens).AsQueryable();
        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(n => n.DataEmissao)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, total);
    }
}