using System.ComponentModel;

namespace EstoqueService.Core.Enums;

public enum TipoMovimentacoesEstoque
{
    [Description("Entrada")]
    Entrada = 1,
    [Description("Saída")]
    Saida = 2
}