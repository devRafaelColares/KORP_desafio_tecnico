using FaturamentoService.Core.Interfaces;
using FaturamentoService.Core.Responses;
using FaturamentoService.Core.Responses.Notas;
using Microsoft.AspNetCore.Mvc;
using FaturamentoService.Configurations;
using FaturamentoService.Contracts;

namespace FaturamentoService.Endpoints.Notas;

public class GetAllNotasFiscaisEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/notas-fiscais", HandleAsync)
            .WithName("GetAllNotasFiscais")
            .WithTags("NotasFiscais")
            .WithSummary("Lista todas as notas fiscais")
            .WithDescription("Retorna todas as notas fiscais cadastradas, com paginação.")
            .Produces<PagedResponse<List<NotaFiscalResponse>>>(StatusCodes.Status200OK)
            .Produces<PagedResponse<List<NotaFiscalResponse>>>(StatusCodes.Status400BadRequest)
            .Produces<PagedResponse<List<NotaFiscalResponse>>>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] INotaFiscalService service,
        [FromQuery] int pageNumber = ApiConstants.DefaultPageNumber,
        [FromQuery] int pageSize = ApiConstants.DefaultPageSize
    )
    {
        try
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return Results.BadRequest(new PagedResponse<List<NotaFiscalResponse>?>(null, 400, "Parâmetros de paginação inválidos."));

            if (pageSize > 100)
                return Results.BadRequest(new PagedResponse<List<NotaFiscalResponse>?>(null, 400, "Tamanho da página não pode exceder 100 registros."));

            var pagedResponse = await service.GetAllAsync(pageNumber, pageSize);

            return pagedResponse switch
            {
                { Code: 404 } => Results.NotFound(pagedResponse),
                { Code: 400 } => Results.BadRequest(pagedResponse),
                { Code: 200 } => Results.Ok(pagedResponse),
                _ => Results.Problem(
                        detail: pagedResponse.Message ?? "Erro inesperado ao listar notas fiscais.",
                        statusCode: pagedResponse.Code,
                        title: "Erro ao listar notas fiscais"
                    )
            };
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Erro interno ao listar notas fiscais"
            );
        }
    }
}