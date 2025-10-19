using FaturamentoService.Core.Interfaces;
using FaturamentoService.Core.Responses;
using FaturamentoService.Core.Responses.Notas;
using Microsoft.AspNetCore.Mvc;
using FaturamentoService.Contracts;

namespace FaturamentoService.Endpoints.Notas;

public class GetNotaFiscalByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/notas-fiscais/{id:int}", HandleAsync)
            .WithName("GetNotaFiscalById")
            .WithTags("NotasFiscais")
            .WithSummary("Buscar nota fiscal por ID")
            .WithDescription("Retorna os dados de uma nota fiscal específica pelo identificador.")
            .Produces<Response<NotaFiscalResponse>>(StatusCodes.Status200OK)
            .Produces<Response<NotaFiscalResponse>>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] INotaFiscalService service,
        [FromRoute] int id)
    {
        try
        {
            var response = await service.GetByIdAsync(id);

            return response.Code switch
            {
                200 => Results.Ok(response),
                404 => Results.NotFound(response),
                _ => Results.Problem(detail: response.Message, statusCode: response.Code)
            };
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Erro interno ao buscar nota fiscal"
            );
        }
    }
}