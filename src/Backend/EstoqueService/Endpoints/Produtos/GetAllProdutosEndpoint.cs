using EstoqueService.Contracts;
using EstoqueService.Core.Interfaces;
using EstoqueService.Core.Responses;
using EstoqueService.Core.Responses.Produto;
using Microsoft.AspNetCore.Mvc;
using EstoqueService.Configurations;

namespace EstoqueService.Endpoints.Produtos;

public class GetAllProdutosEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/produtos", HandleAsync)
            .WithName("GetAllProdutos")
            .WithTags("Produtos")
            .WithSummary("Lista todos os produtos")
            .WithDescription("Retorna todos os produtos cadastrados, com paginação.")
            .Produces<PagedResponse<List<ProdutoResponse>>>(StatusCodes.Status200OK)
            .Produces<PagedResponse<List<ProdutoResponse>>>(StatusCodes.Status400BadRequest)
            .Produces<PagedResponse<List<ProdutoResponse>>>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IProdutoService service,
        [FromQuery] int pageNumber = ApiConstants.DefaultPageNumber,
        [FromQuery] int pageSize = ApiConstants.DefaultPageSize
    )
    {
        try
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return Results.BadRequest(new PagedResponse<List<ProdutoResponse>?>(null, 400, "Parâmetros de paginação inválidos."));

            if (pageSize > 100)
                return Results.BadRequest(new PagedResponse<List<ProdutoResponse>?>(null, 400, "Tamanho da página não pode exceder 100 registros."));

            var pagedResponse = await service.GetAllAsync(pageNumber, pageSize);

            return pagedResponse switch
            {
                { Code: 404 } => Results.NotFound(pagedResponse),
                { Code: 400 } => Results.BadRequest(pagedResponse),
                { Code: 200 } => Results.Ok(pagedResponse),
                _ => Results.Problem(
                        detail: pagedResponse.Message ?? "Erro inesperado ao listar produtos.",
                        statusCode: pagedResponse.Code,
                        title: "Erro ao listar produtos"
                    )
            };
        }
        catch (Exception ex)
        {
            // Logar ex para rastreabilidade
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Erro interno ao listar produtos"
            );
        }
    }
}