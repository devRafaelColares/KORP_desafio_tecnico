using EstoqueService.Endpoints;
using EstoqueService.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.AddConfiguration();
builder.AddDataContexts();
builder.AddCrossOrigin();
builder.AddDocumentation();
builder.AddAuthentication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.ConfigureDevEnvironment();
}

app.UseCors("korp");
app.UseAuthorization();

app.MapEndpoints();

app.Run();