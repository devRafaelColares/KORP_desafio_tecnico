using EstoqueService.Contracts;
using EstoqueService.Core.Interfaces;
using EstoqueService.Core.Responses;
using EstoqueService.Core.Responses.Produto;
using Microsoft.AspNetCore.Mvc;

namespace EstoqueService.Endpoints.Produtos;

/// <summary>
/// Endpoint para obter produto por SKU
/// </summary>
public class GetProdutoBySkuEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/produtos/sku/{sku}", HandleAsync)
            .WithName("GetProdutoBySku")
            .WithTags("Produtos")
            .WithSummary("Obter produto por SKU")
            .WithDescription("Retorna um produto cadastrado a partir do código SKU informado.")
            .Produces<Response<ProdutoResponse>>(StatusCodes.Status200OK)
            .Produces<Response<ProdutoResponse>>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IProdutoService service,
        [FromRoute] string sku)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(sku))
                return Results.BadRequest(new Response<ProdutoResponse>(null, 400, "SKU é obrigatório."));

            var response = await service.GetBySkuAsync(sku);

            return response.Code switch
            {
                200 => Results.Ok(response),
                404 => Results.NotFound(response),
                _ => Results.Problem(detail: response.Message, statusCode: response.Code)
            };
        }
        catch (Exception ex)
        {
            // Logar ex para rastreabilidade
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Erro interno ao buscar produto por SKU"
            );
        }
    }
}