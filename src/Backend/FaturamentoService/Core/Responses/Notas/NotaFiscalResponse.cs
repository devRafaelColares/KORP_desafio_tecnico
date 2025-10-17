using FaturamentoService.Core.Enums;
namespace FaturamentoService.Core.Responses.Notas;

public class NotaFiscalResponse
{
    public int Id { get; set; }
    public string Numero { get; set; } = null!;
    public TipoStatusNF Status { get; set; }
    public DateTime DataEmissao { get; set; }
    public List<ItemNotaFiscalResponse> Itens { get; set; } = new();
}