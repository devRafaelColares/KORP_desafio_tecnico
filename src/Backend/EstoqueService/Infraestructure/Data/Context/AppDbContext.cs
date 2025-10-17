using System.Reflection;
using EstoqueService.Models;
using Microsoft.EntityFrameworkCore;

namespace EstoqueService.Infraestructure.Data.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public virtual DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = null!;
    public virtual DbSet<Produto> Produtos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_100_CI_AI");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}