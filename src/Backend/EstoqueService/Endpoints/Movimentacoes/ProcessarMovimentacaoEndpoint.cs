using EstoqueService.Core.Interfaces;
using EstoqueService.Core.Requests.Estoque;
using EstoqueService.Core.Responses.Estoque;
using EstoqueService.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using EstoqueService.Contracts;

namespace EstoqueService.Endpoints.Movimentacoes;

public class ProcessarMovimentacaoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/movimentacoes-estoque", HandleAsync)
            .WithName("ProcessarMovimentacaoEstoque")
            .WithTags("MovimentacoesEstoque")
            .WithSummary("Processa movimentação individual de estoque")
            .Produces<Response<BaixaProdutoResultado>>(StatusCodes.Status200OK)
            .Produces<Response<BaixaProdutoResultado>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IMovimentacaoEstoqueService service,
        [FromBody] MovimentacaoEstoqueRequest request)
    {
        try
        {
            var response = await service.ProcessarMovimentacaoAsync(request);

            return response.Code switch
            {
                200 => Results.Ok(response),
                400 => Results.BadRequest(response),
                _ => Results.Problem(detail: response.Message, statusCode: response.Code)
            };
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Erro interno ao processar movimentação de estoque"
            );
        }
    }
}