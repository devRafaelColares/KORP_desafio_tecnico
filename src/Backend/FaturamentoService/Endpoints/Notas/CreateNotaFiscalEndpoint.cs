using FaturamentoService.Core.Interfaces;
using FaturamentoService.Core.Requests.Notas;
using FaturamentoService.Core.Responses;
using FaturamentoService.Core.Responses.Notas;
using Microsoft.AspNetCore.Mvc;
using FaturamentoService.Contracts;

namespace FaturamentoService.Endpoints.Notas;

public class CreateNotaFiscalEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/notas", HandleAsync)
            .WithName("CreateNotaFiscal")
            .WithTags("NotasFiscais")
            .WithSummary("Cadastra uma nota fiscal com itens")
            .Produces<Response<NotaFiscalResponse>>(StatusCodes.Status201Created)
            .Produces<Response<NotaFiscalResponse>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] INotaFiscalService service,
        [FromBody] NotaFiscalCreateRequest request)
    {
        try
        {
            var response = await service.CreateAsync(request);
            return response.Code switch
            {
                201 => Results.Created($"/notas/{response.Data?.Id}", response),
                400 => Results.BadRequest(response),
                _ => Results.Problem(detail: response.Message, statusCode: response.Code)
            };
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}