using EstoqueService.Core.Enums;

namespace EstoqueService.Core.Responses.Estoque;

public class BaixaProdutoResultado
{
    public int ProdutoId { get; set; }
    public int QuantidadeMovimentada { get; set; }
    public int SaldoFinal { get; set; }
    public TipoMovimentacoesEstoque Tipo { get; set; }
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }
    public string? Erro { get; set; }
}