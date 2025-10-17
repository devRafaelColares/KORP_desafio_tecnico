using EstoqueService.Core.Enums;

namespace EstoqueService.Core.Responses.Movimentacoes;

public class MovimentacaoEstoqueResponse
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public TipoMovimentacoesEstoque Tipo { get; set; }
    public DateTime Data { get; set; }
    public string? Observacao { get; set; }
}