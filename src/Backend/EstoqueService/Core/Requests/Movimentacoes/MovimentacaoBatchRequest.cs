namespace EstoqueService.Core.Requests.Movimentacoes;

public class MovimentacaoBatchRequest
{
    public List<ItemBaixaEstoque> Itens { get; set; } = new();
    public string? Observacao { get; set; }
}

public class ItemBaixaEstoque
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
}