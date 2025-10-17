namespace EstoqueService.Core.Responses.Estoque;

public class BaixaEstoqueResponse
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }
    public List<BaixaProdutoResultado>? Resultados { get; set; }
}