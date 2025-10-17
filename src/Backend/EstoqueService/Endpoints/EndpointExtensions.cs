using EstoqueService.Configurations;
using EstoqueService.Contracts;
using EstoqueService.Endpoints.Produtos;

namespace EstoqueService.Endpoints;

public static class EndpointExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        string _urlBase = $"{ApiConstants.ApplicationName}/{ApiConstants.Version}";
        var v1Group = app.MapGroup(_urlBase);

        ConfigurePublicEndpoints(v1Group);

        // var adminGroup = v1Group.RequireAuthorization("AdminPolicy");
        // ConfigureBusinessEndpoints(adminGroup);
    }

    private static void ConfigurePublicEndpoints(RouteGroupBuilder group)
    {
        group.MapGroup("/healthcheck")
            .WithTags("HealthCheck")
            .WithOpenApi()
            .AllowAnonymous()
            .MapGet("/", () => Results.Ok(new { message = "OK" }))
            .WithName("HealthCheck")
            .WithSummary("Verifica o status da API")
            .WithDescription("Endpoint para monitoramento da saúde da API.");

        group.MapEndpoint<GetAllProdutosEndpoint>();
    }

    // private static void ConfigureBusinessEndpoints(RouteGroupBuilder group)
    // {
    // }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}