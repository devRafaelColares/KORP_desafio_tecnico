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
}
