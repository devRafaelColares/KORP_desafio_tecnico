using EstoqueService.Contracts;
using EstoqueService.Core.Interfaces;
using EstoqueService.Core.Requests.Movimentacoes;
using EstoqueService.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace EstoqueService.Endpoints.Movimentacoes;

/// <summary>
/// Endpoint CRÍTICO para baixa de estoque em lote
/// Chamado pelo FaturamentoService ao imprimir nota fiscal
/// </summary>
public class ProcessarBaixaLoteEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/estoque/movimentacoes/batch", HandleAsync)
            .WithName("ProcessarBaixaLote")
            .WithTags("Movimentacoes")
            .WithSummary("Processa baixa de estoque em lote (usado na impressão de NF)")
            .WithDescription("Valida saldo, baixa estoque e registra movimentações de forma transacional.")
            .Produces<Response<bool>>(StatusCodes.Status200OK)
            .Produces<Response<bool>>(StatusCodes.Status400BadRequest)
            .Produces<Response<bool>>(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IMovimentacaoEstoqueService service,
        [FromBody] MovimentacaoBatchRequest request
    )
    {
        try
        {
            var response = await service.ProcessarBaixaEmLoteAsync(request);

            return response.Code switch
            {
                200 => Results.Ok(response),
                400 => Results.BadRequest(response),
                409 => Results.Conflict(response),
                _ => Results.Problem(
                    detail: response.Message ?? "Erro inesperado ao processar baixa.",
                    statusCode: response.Code,
                    title: "Erro ao processar baixa de estoque"
                )
            };
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Erro crítico ao processar baixa de estoque"
            );
        }
    }
}