namespace FaturamentoService.Core.Responses.Notas;

public class ImprimirNotaFiscalResponse
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }
    public NotaFiscalResponse? NotaFiscal { get; set; }
    public List<BaixaProdutoResultado>? ResultadosBaixaEstoque { get; set; }
}