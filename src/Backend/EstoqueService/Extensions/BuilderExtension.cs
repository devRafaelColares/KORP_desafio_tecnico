using EstoqueService.Configurations;
using EstoqueService.Services;
using EstoqueService.Core.Interfaces;
using EstoqueService.Infraestructure.Data.Context;
using EstoqueService.Infraestructure.Data.UnitOfWork;
using EstoqueService.Infraestructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EstoqueService.Extensions;

public static class BuilderExtension
{
    public static void AddConfiguration(this WebApplicationBuilder builder)
    {
        ApiConstants.ConnectionString =
            builder
            .Configuration
            .GetConnectionString("DefaultConnection") ?? string.Empty;

        ApiConstants.FrontendUrl = builder.Configuration.GetValue<string>("FrontendUrl") ?? string.Empty;
        ApiConstants.BackendUrlEstoqueService = builder.Configuration.GetValue<string>("BackendUrlEstoqueService") ?? string.Empty;
    }

    public static void AddDataContexts(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(ApiConstants.ConnectionString),
            ServiceLifetime.Scoped);
    }

    public static void AddDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(n => n.FullName);
        });
    }

    public static void AddCrossOrigin(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(
            options => options.AddPolicy(
                ApiConfiguration.CorsPolicyName,
                policy => policy
                    .WithOrigins([
                        ApiConstants.BackendUrlEstoqueService,
                        ApiConstants.FrontendUrl
                    ])
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
            ));
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();

        builder.Services.AddScoped<IProdutoService, ProdutoService>();
    }

    public static void AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"))
            .AddPolicy("UserPolicy", policy => policy.RequireRole("User", "Admin"));
    }
}