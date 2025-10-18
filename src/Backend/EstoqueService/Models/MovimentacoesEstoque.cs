using EstoqueService.Core.Enums;

namespace EstoqueService.Models;

public class MovimentacaoEstoque
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public int Quantidade { get; set; }
    public TipoMovimentacoesEstoque Tipo { get; set; } = TipoMovimentacoesEstoque.Saida; // "Entrada" ou "Saida"
    public string? Observacao { get; set; }

        public Produto? Produto { get; set; }
}