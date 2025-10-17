namespace EstoqueService.Core.Requests.Produto;

public class ProdutoUpdateRequest
{
    public string? Descricao { get; set; }
    public string? CodigoSKU { get; set; }
    public decimal? Preco { get; set; }
    public int? Saldo { get; set; }
}