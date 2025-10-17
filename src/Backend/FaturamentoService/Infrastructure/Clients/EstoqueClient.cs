using FaturamentoService.Core.Interfaces;
using System.Net;
using System.Net.Http.Json;

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
            return false;
        }
    }

    public async Task<bool> BaixarEstoqueAsync(IEnumerable<EstoqueBaixaItem> itens, CancellationToken cancellationToken = default)
    {
        // Espera-se que EstoqueService exponha um endpoint que processe a baixa em lote:
        // POST /estoque-service/v1/estoque/movimentacoes/batch
        var requestUri = "/estoque-service/v1/estoque/movimentacoes/batch";
        try
        {
            var res = await _http.PostAsJsonAsync(requestUri, itens, cancellationToken);
            if (res.StatusCode == HttpStatusCode.OK) return true;
            if (res.StatusCode == HttpStatusCode.BadRequest) return false; // ex: saldo insuficiente (detalhes no body)
            // outros status -> erro de infraestrutura
            res.EnsureSuccessStatusCode();
            return false;
        }
        catch
        {
            throw; // caller decide como tratar indisponibilidade
        }
    }
}