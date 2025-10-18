using FaturamentoService.Core.Interfaces;
using System.Net;
using System.Net.Http.Json;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using EstoqueService.Core.Requests.Movimentacoes;

namespace FaturamentoService.Infrastructure.Clients;

public class EstoqueClientResilient(HttpClient http) : IEstoqueClient
{
    private readonly HttpClient _http = http;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r =>
                r.StatusCode == HttpStatusCode.RequestTimeout ||
                r.StatusCode == HttpStatusCode.ServiceUnavailable ||
                r.StatusCode == HttpStatusCode.GatewayTimeout)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    Console.WriteLine($"[EstoqueClient] Tentativa {retryCount} após {timespan.TotalSeconds}s. Status: {outcome.Result?.StatusCode}");
                });
    private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy = Policy
            .HandleResult<HttpResponseMessage>(r =>
                !r.IsSuccessStatusCode &&
                r.StatusCode != HttpStatusCode.BadRequest)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (result, duration) =>
                {
                    Console.WriteLine($"[EstoqueClient] Circuit breaker ABERTO por {duration.TotalSeconds}s. Status: {result.Result?.StatusCode}");
                },
                onReset: () =>
                {
                    Console.WriteLine("[EstoqueClient] Circuit breaker RESETADO.");
                },
                onHalfOpen: () =>
                {
                    Console.WriteLine("[EstoqueClient] Circuit breaker MEIO-ABERTO (testando recuperação).");
                });

    public async Task<bool> ProdutoExistsAsync(int produtoId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"/produtos/{produtoId}";
        try
        {
            var response = await _retryPolicy.ExecuteAsync(async () =>
                await _http.GetAsync(requestUri, cancellationToken));

            return response.StatusCode == HttpStatusCode.OK;
        }
        catch (BrokenCircuitException)
        {
            Console.WriteLine("[EstoqueClient] Circuit breaker aberto - serviço indisponível");
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> BaixarEstoqueAsync(MovimentacaoBatchRequest request, CancellationToken cancellationToken = default)
    {
        var requestUri = "/estoque-service/v1/estoque/movimentacoes/batch";

        try
        {
            var response = await _circuitBreakerPolicy.ExecuteAsync(async () =>
                await _retryPolicy.ExecuteAsync(async () =>
                    await _http.PostAsJsonAsync(requestUri, request, cancellationToken)));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine("[EstoqueClient] Baixa de estoque realizada com sucesso");
                return true;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine($"[EstoqueClient] Baixa recusada: {errorContent}");
                return false;
            }

            response.EnsureSuccessStatusCode();
            return false;
        }
        catch (BrokenCircuitException ex)
        {
            Console.WriteLine($"[EstoqueClient] Circuit breaker aberto - EstoqueService indisponível: {ex.Message}");
            throw new HttpRequestException("EstoqueService temporariamente indisponível. Circuit breaker aberto.", ex);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[EstoqueClient] Falha de comunicação após retries: {ex.Message}");
            throw;
        }
    }
}