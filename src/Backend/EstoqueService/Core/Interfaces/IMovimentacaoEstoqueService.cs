using EstoqueService.Core.Requests.Movimentacoes;
using EstoqueService.Core.Responses;

namespace EstoqueService.Core.Interfaces;

public interface IMovimentacaoEstoqueService
{
    Task<Response<bool>> ProcessarBaixaEmLoteAsync(MovimentacaoBatchRequest request);
}