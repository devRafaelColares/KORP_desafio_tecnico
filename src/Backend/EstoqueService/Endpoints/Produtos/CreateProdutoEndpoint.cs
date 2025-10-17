using EstoqueService.Contracts;
using EstoqueService.Core.Interfaces;
using EstoqueService.Core.Requests.Produto;
using EstoqueService.Core.Responses;
using EstoqueService.Core.Responses.Produto;
using Microsoft.AspNetCore.Mvc;

namespace EstoqueService.Endpoints.Produtos;

/// <summary>
/// Endpoint para criação de novos produtos
/// </summary>
public class CreateProdutoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/produtos", HandleAsync)
            .WithName("CreateProduto")
            .WithTags("Produtos")
            .WithSummary("Cadastra um novo produto")
            .WithDescription("Cria um novo produto no sistema com informações básicas e saldo inicial.")
            .Produces<Response<ProdutoResponse>>(StatusCodes.Status201Created)
            .Produces<Response<ProdutoResponse>>(StatusCodes.Status400BadRequest)
            .Produces<Response<ProdutoResponse>>(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IProdutoService service,
        [FromServices] IProdutoRepository repository,
        [FromBody] ProdutoCreateRequest request
    )
    {
        try
        {
            // Validações de negócio
            if (string.IsNullOrWhiteSpace(request.Descricao))
                return Results.BadRequest(new Response<ProdutoResponse>(null, 400, "Descrição do produto é obrigatória."));

            if (string.IsNullOrWhiteSpace(request.CodigoSKU))
                return Results.BadRequest(new Response<ProdutoResponse>(null, 400, "Código SKU é obrigatório."));

            if (request.Preco <= 0)
                return Results.BadRequest(new Response<ProdutoResponse>(null, 400, "Preço deve ser maior que zero."));

            if (request.Saldo < 0)
                return Results.BadRequest(new Response<ProdutoResponse>(null, 400, "Saldo inicial não pode ser negativo."));

            // Validar duplicidade de SKU (regra de negócio crítica)
            var existingSku = await repository.GetBySkuAsync(request.CodigoSKU);
            if (existingSku != null)
                return Results.Conflict(new Response<ProdutoResponse>(null, 409, $"Já existe um produto com o SKU '{request.CodigoSKU}'."));

            // Criar produto
            var response = await service.CreateAsync(request);

            return response switch
            {
                { Code: 201 } => Results.Created($"/produtos/{response.Data?.Id}", response),
                { Code: 400 } => Results.BadRequest(response),
                { Code: 409 } => Results.Conflict(response),
                _ => Results.Problem(
                    detail: response.Message ?? "Erro inesperado ao criar produto.",
                    statusCode: response.Code,
                    title: "Erro ao criar produto"
                )
            };
        }
        catch (Exception ex)
        {
            // Log exception para rastreabilidade
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Erro interno ao criar produto"
            );
        }
    }
}