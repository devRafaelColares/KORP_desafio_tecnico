namespace EstoqueService.Core.Requests.Estoque;

public class BaixaEstoqueRequest
{
    public List<BaixaProdutoItem> Itens { get; set; } = new();
}