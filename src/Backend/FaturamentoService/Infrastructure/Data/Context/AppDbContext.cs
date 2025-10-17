using System.Reflection;
using FaturamentoService.Models;
using Microsoft.EntityFrameworkCore;

namespace FaturamentoService.Infrastructure.Data.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public virtual DbSet<ItemNotaFiscal> ItensNotaFiscal { get; set; } = null!;
    public virtual DbSet<NotaFiscal> NotasFiscais { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_100_CI_AI");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}