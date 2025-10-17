using FaturamentoService.Configurations;
using Newtonsoft.Json;

namespace FaturamentoService.Core.Responses;
/// <summary>
/// Representa uma resposta paginada para requisições que retornam listas de dados.
/// </summary>
/// <typeparam name="TData">O tipo dos dados retornados.</typeparam>
public class PagedResponse<TData> : Response<TData>
{
    /// <summary>
    /// Construtor da resposta paginada com parâmetros obrigatórios.
    /// </summary>
    /// <param name="data">Os dados retornados na resposta.</param>
    /// <param name="totalCount">O número total de registros disponíveis.</param>
    /// <param name="currentPage">A página atual da resposta (padrão: 1).</param>
    /// <param name="pageSize">O tamanho da página (padrão definido em ApiConstants).</param>
    [JsonConstructor]
    public PagedResponse(
        TData data,
        int totalCount,
        int currentPage = 1,
        int pageSize = ApiConstants.DefaultPageSize
    ) : base(data)
    {
        Data = data;
        TotalCount = totalCount;
        CurrentPage = currentPage;
        PageSize = pageSize;
    }

    /// <summary>
    /// Construtor da resposta paginada para situações onde apenas um código de status é necessário.
    /// </summary>
    /// <param name="data">Os dados retornados na resposta.</param>
    /// <param name="code">Código de status da resposta (padrão definido em ApiConstants).</param>
    public PagedResponse(
        TData? data,
        int code = ApiConstants.DefaultStatusCode,
        string? message = null)
        : base(data, code, message)
    {
    }

    /// <summary>
    /// Obtém ou define a página atual da resposta.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Obtém o número total de páginas baseado no tamanho da página e na quantidade total de registros.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Obtém ou define o tamanho da página (quantidade de itens por página).
    /// </summary>
    public int PageSize { get; set; } = ApiConstants.DefaultPageSize;

    /// <summary>
    /// Obtém ou define o número total de registros disponíveis.
    /// </summary>
    public int TotalCount { get; set; }
}