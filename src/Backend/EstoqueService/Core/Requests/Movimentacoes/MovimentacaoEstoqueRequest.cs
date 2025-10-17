using EstoqueService.Core.Enums;

namespace EstoqueService.Core.Requests.Movimentacoes;

public class MovimentacaoEstoqueRequest
{
    public int Quantidade { get; set; }
    public TipoMovimentacoesEstoque Tipo { get; set; } // Entrada/Saida
    public string? Observacao { get; set; }
}