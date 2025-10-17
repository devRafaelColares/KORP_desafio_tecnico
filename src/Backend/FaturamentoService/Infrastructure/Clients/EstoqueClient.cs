using FaturamentoService.Core.Interfaces;
using FaturamentoService.Configurations;
using System.Net;

namespace FaturamentoService.Infrastructure.Clients;

public class EstoqueClient : IEstoqueClient
{
    private readonly HttpClient _http;
    public EstoqueClient(HttpClient http) => _http = http;

    public async Task<bool> ProdutoExistsAsync(int produtoId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"/estoque-service/v1/produtos/{produtoId}";
        try
        {
            var res = await _http.GetAsync(requestUri, cancellationToken);
            return res.StatusCode == HttpStatusCode.OK;
        }
        catch
        {
            // Em caso de timeout/erro de comunicação considerar não existente -> caller decide
            return false;
        }
    }
}