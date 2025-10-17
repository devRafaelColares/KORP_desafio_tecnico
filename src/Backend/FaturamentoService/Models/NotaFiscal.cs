using FaturamentoService.Core.Enums;

namespace FaturamentoService.Models;

public class NotaFiscal
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public TipoStatusNF Status { get; set; } = TipoStatusNF.Aberta; // "Aberta" ou "Fechada"
    public DateTime DataEmissao { get; set; } = DateTime.UtcNow;

    public ICollection<ItemNotaFiscal>? Itens { get; set; }
}