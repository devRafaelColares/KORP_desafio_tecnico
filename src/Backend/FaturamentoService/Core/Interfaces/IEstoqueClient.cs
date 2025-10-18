using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EstoqueService.Core.Requests.Movimentacoes;

namespace FaturamentoService.Core.Interfaces;

public record EstoqueBaixaItem(int ProdutoId, int Quantidade);

public interface IEstoqueClient
{
    Task<bool> ProdutoExistsAsync(int produtoId, CancellationToken cancellationToken = default);
    Task<bool> BaixarEstoqueAsync(MovimentacaoBatchRequest request, CancellationToken cancellationToken = default);
}