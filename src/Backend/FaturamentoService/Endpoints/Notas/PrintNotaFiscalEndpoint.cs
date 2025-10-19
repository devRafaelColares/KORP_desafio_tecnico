using FaturamentoService.Core.Interfaces;
using FaturamentoService.Core.Requests.Notas;
using FaturamentoService.Core.Responses.Notas;
using FaturamentoService.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using FaturamentoService.Contracts;

namespace FaturamentoService.Endpoints.Notas;

public class PrintNotaFiscalEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/notas-fiscais/{id:int}/imprimir", HandleAsync)
            .WithName("PrintNotaFiscal")
            .WithTags("NotasFiscais")
            .WithSummary("Imprimir nota fiscal: valida saldo, baixa estoque e fecha nota")
            .Produces<Response<ImprimirNotaFiscalResponse>>(StatusCodes.Status200OK)
            .Produces<Response<ImprimirNotaFiscalResponse>>(StatusCodes.Status400BadRequest)
            .Produces<Response<ImprimirNotaFiscalResponse>>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status503ServiceUnavailable);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] INotaFiscalService service,
        [FromRoute] int id,
        [FromBody] ImprimirNotaFiscalRequest? request // opcional
    )
    {
        var response = await service.PrintAsync(id, request);

        return response.Code switch
        {
            200 => Results.Ok(response),
            400 => Results.BadRequest(response),
            404 => Results.NotFound(response),
            503 => Results.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable),
            _ => Results.Problem(detail: response.Message, statusCode: response.Code)
        };
    }
}