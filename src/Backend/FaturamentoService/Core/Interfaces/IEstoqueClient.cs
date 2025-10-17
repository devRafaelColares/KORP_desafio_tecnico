using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaturamentoService.Core.Interfaces;

public record EstoqueBaixaItem(int ProdutoId, int Quantidade);

public interface IEstoqueClient
{
    Task<bool> ProdutoExistsAsync(int produtoId, CancellationToken cancellationToken = default);
    Task<bool> BaixarEstoqueAsync(IEnumerable<EstoqueBaixaItem> itens, CancellationToken cancellationToken = default); // { changed code }
}