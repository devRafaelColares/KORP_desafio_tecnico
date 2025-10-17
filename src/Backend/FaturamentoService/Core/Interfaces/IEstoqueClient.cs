namespace FaturamentoService.Core.Interfaces;
public interface IEstoqueClient
{
    Task<bool> ProdutoExistsAsync(int produtoId, CancellationToken cancellationToken = default);
}