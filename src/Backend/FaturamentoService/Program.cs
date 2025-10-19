using EstoqueService.Configurations;
using FaturamentoService.Endpoints;
using FaturamentoService.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.AddConfiguration();
builder.AddDataContexts();
builder.AddCrossOrigin();
builder.AddDocumentation();
builder.AddAuthentication();
builder.AddHttpClientService();
builder.AddServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.ConfigureDevEnvironment();
    await app.SeedDatabase();
}

app.UseCors(ApiConfiguration.CorsPolicyName);
app.UseAuthorization();

app.MapEndpoints();

app.Run();