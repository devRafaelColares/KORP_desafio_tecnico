using FaturamentoService.Core.Interfaces;
using FaturamentoService.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using FaturamentoService.Contracts;

namespace FaturamentoService.Endpoints.Notas;

public class DeleteNotaFiscalEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/notas-fiscais/{id:int}", HandleAsync)
            .WithName("DeleteNotaFiscal")
            .WithTags("NotasFiscais")
            .WithSummary("Excluir nota fiscal")
            .WithDescription("Remove uma nota fiscal pelo identificador informado.")
            .Produces<Response<object>>(StatusCodes.Status200OK)
            .Produces<Response<object>>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] INotaFiscalService service,
        [FromRoute] int id)
    {
        try
        {
            var response = await service.DeleteAsync(id);

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
                title: "Erro interno ao excluir nota fiscal"
            );
        }
    }
}