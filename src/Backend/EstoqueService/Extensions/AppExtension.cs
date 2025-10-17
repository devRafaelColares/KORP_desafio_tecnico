
using EstoqueService.Infrastructure.Data.Context;
using EstoqueService.Infrastructure.Data.Seeders;

namespace EstoqueService.Extensions;
public static class AppExtension
{
    public static void ConfigureDevEnvironment(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.DocumentTitle = "Estoque Service API";
        });
    }

    public static async Task SeedDatabase(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await DatabaseSeeder.SeedAsync(context);
        }
    }
}
