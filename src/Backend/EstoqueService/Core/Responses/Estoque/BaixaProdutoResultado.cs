namespace EstoqueService.Core.Responses.Estoque;

public class BaixaProdutoResultado
{
    public int ProdutoId { get; set; }
    public bool Sucesso { get; set; }
    public string? Erro { get; set; }
}