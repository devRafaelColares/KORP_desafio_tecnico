using EstoqueService.Core.Requests.Movimentacoes;
using EstoqueService.Core.Responses;
using EstoqueService.Core.Responses.Estoque;


namespace EstoqueService.Core.Interfaces;

public interface IMovimentacaoEstoqueService
{
    Task<Response<BaixaProdutoResultado>> ProcessarMovimentacaoAsync(MovimentacaoEstoqueRequest request);
    Task<Response<List<BaixaProdutoResultado>>> ProcessarBaixaLoteAsync(MovimentacaoBatchRequest request);
}