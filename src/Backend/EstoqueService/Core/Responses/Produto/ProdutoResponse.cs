namespace EstoqueService.Core.Responses.Produto;

public class ProdutoResponse
{
    public int Id { get; set; }
    public string Descricao { get; set; } = null!;
    public string CodigoSKU { get; set; }  = null!;
    public decimal Preco { get; set; }
    public int Saldo { get; set; }
}