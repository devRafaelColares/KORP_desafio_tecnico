using EstoqueService.Contracts;
using EstoqueService.Core.Interfaces;
using EstoqueService.Core.Responses;
using EstoqueService.Core.Responses.Produto;
using Microsoft.AspNetCore.Mvc;

namespace EstoqueService.Endpoints.Produtos;

public class GetProdutoByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/produtos/{id:int}", HandleAsync)
            .WithName("GetProdutoById")
            .WithTags("Produtos")
            .WithSummary("Obter produto por id")
            .Produces<Response<ProdutoResponse>>(StatusCodes.Status200OK)
            .Produces<Response<ProdutoResponse>>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IProdutoService service,
        [FromRoute] int id)
    {
        var response = await service.GetByIdAsync(id);
        return response.Code switch
        {
            200 => Results.Ok(response),
            404 => Results.NotFound(response),
            _ => Results.Problem(detail: response.Message, statusCode: response.Code)
        };
    }
}