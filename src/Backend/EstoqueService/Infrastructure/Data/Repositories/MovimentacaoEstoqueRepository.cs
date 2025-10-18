using EstoqueService.Core.Interfaces;
using EstoqueService.Models;
using EstoqueService.Infrastructure.Data.Context;

namespace EstoqueService.Infrastructure.Data.Repositories;

public class MovimentacaoEstoqueRepository(AppDbContext context) : IMovimentacaoEstoqueRepository
{
    private readonly AppDbContext _context = context;

    public async Task AddAsync(MovimentacaoEstoque movimentacao)
    {
        await _context.MovimentacoesEstoque.AddAsync(movimentacao);
    }

    public async Task AddRangeAsync(IEnumerable<MovimentacaoEstoque> movimentacoes)
    {
        await _context.MovimentacoesEstoque.AddRangeAsync(movimentacoes);
    }
}