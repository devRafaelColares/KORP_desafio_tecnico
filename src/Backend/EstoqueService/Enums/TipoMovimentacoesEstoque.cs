using System.ComponentModel;

namespace EstoqueService.Enums;

public enum TipoMovimentacoesEstoque
{
    [Description("Entrada")]
    Entrada = 1,
    [Description("Saída")]
    Saida = 2
}