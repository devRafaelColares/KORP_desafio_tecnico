namespace FaturamentoService.Core.Responses.Notas;

public class ImprimirNotaFiscalResponse
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTime DataEmissao { get; set; }
    public List<ItemNotaFiscalResponse> Itens { get; set; } = new();
}