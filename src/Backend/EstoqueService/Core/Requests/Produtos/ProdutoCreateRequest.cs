namespace EstoqueService.Core.Requests.Produto;

public class ProdutoCreateRequest
{
    public string Descricao { get; set; } = null!;
    public string CodigoSKU { get; set; } = null!;
    public decimal Preco { get; set; }
    public int Saldo { get; set; }
}