using FaturamentoService.Core.Enums;

namespace FaturamentoService.Core.Requests.Notas;

public class NotaFiscalUpdateRequest
{
    public TipoStatusNF? Status { get; set; }
}