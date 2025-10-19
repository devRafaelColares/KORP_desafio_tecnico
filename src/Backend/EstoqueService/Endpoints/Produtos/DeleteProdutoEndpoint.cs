using EstoqueService.Contracts;
using EstoqueService.Core.Interfaces;
using EstoqueService.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace EstoqueService.Endpoints.Produtos;

/// <summary>
/// Endpoint para exclusão de produto
/// </summary>
public class DeleteProdutoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/produtos/{id:int}", HandleAsync)
            .WithName("DeleteProduto")
            .WithTags("Produtos")
            .WithSummary("Exclui um produto")
            .WithDescription("Remove um produto do sistema pelo id informado.")
            .Produces<Response<bool>>(StatusCodes.Status200OK)
            .Produces<Response<bool>>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IProdutoService service,
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
                title: "Erro interno ao excluir produto"
            );
        }
    }
}