using EstoqueService.Contracts;
using EstoqueService.Core.Interfaces;
using EstoqueService.Core.Requests.Produto;
using EstoqueService.Core.Responses;
using EstoqueService.Core.Responses.Produto;
using Microsoft.AspNetCore.Mvc;

namespace EstoqueService.Endpoints.Produtos;

/// <summary>
/// Endpoint para atualização de produto
/// </summary>
public class UpdateProdutoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/produtos/{id:int}", HandleAsync)
            .WithName("UpdateProduto")
            .WithTags("Produtos")
            .WithSummary("Atualiza um produto")
            .WithDescription("Atualiza os dados de um produto existente.")
            .Produces<Response<ProdutoResponse>>(StatusCodes.Status200OK)
            .Produces<Response<ProdutoResponse>>(StatusCodes.Status400BadRequest)
            .Produces<Response<ProdutoResponse>>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IProdutoService service,
        [FromRoute] int id,
        [FromBody] ProdutoUpdateRequest request)
    {
        try
        {
            var response = await service.UpdateAsync(id, request);

            return response.Code switch
            {
                200 => Results.Ok(response),
                404 => Results.NotFound(response),
                400 => Results.BadRequest(response),
                _ => Results.Problem(detail: response.Message, statusCode: response.Code)
            };
        }
        catch (Exception ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Erro interno ao atualizar produto"
            );
        }
    }
}