using System.Reflection;
using EstoqueService.Contracts;

namespace EstoqueService.Endpoints;

public static class EndpointExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        var v1Group = app.MapGroup("/api/v1");

        ConfigurePublicEndpoints(v1Group);

        var adminGroup = v1Group.RequireAuthorization("AdminPolicy");
        ConfigureBusinessEndpoints(adminGroup);
    }

    private static void ConfigurePublicEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/healthcheck", () => Results.Ok(new { message = "OK" }))
            .WithTags("HealthCheck")
            .WithOpenApi()
            .AllowAnonymous()
            .WithName("HealthCheck")
            .WithSummary("Verifica o status da API")
            .WithDescription("Endpoint para monitoramento da saúde da API.");
    }

    private static void ConfigureBusinessEndpoints(RouteGroupBuilder group)
    {
        MapEndpointsFromAssembly(group, Assembly.GetExecutingAssembly());
    }

    private static void MapEndpointsFromAssembly(IEndpointRouteBuilder app, Assembly assembly)
    {
        var endpointTypes = assembly.GetExportedTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in endpointTypes)
        {
            var endpointInstance = (IEndpoint)Activator.CreateInstance(type)!;

            endpointInstance.Map(app);
        }
    }
}