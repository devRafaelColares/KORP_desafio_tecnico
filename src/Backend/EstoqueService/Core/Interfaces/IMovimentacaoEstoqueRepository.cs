using EstoqueService.Models;

namespace EstoqueService.Core.Interfaces;

public interface IMovimentacaoEstoqueRepository
{
    Task AddAsync(MovimentacaoEstoque movimentacao);
    Task AddRangeAsync(IEnumerable<MovimentacaoEstoque> movimentacoes);
}