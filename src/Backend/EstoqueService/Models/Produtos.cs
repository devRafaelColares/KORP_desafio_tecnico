namespace EstoqueService.Models;

public class Produto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string CodigoSKU { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Saldo { get; set; }

    // Navegação para histórico de movimentações
    public ICollection<MovimentacaoEstoque>? Movimentacoes { get; set; }
}