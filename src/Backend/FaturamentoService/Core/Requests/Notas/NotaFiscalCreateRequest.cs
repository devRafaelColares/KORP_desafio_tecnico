namespace FaturamentoService.Core.Requests.Notas;

public class NotaFiscalCreateRequest
{
    public string Numero { get; set; } = null!;
    public List<ItemNotaFiscalCreateRequest> Itens { get; set; } = new();
}