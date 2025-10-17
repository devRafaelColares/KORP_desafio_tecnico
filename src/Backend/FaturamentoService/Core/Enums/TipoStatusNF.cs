using System.ComponentModel;

namespace FaturamentoService.Core.Enums;

public enum TipoStatusNF
{
    [Description("Aberta")]
    Aberta = 1,

    [Description("Fechada")]
    Fechada = 2
}