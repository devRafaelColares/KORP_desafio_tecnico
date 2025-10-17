using FaturamentoService.Core.Enums;
using FaturamentoService.Models;
using FaturamentoService.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FaturamentoService.Infraestructure.Data.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        try
        {
            if (!await context.NotasFiscais.AnyAsync())
            {
                Console.WriteLine("🌱 Seed: Notas Fiscais...");
                await SeedNotasFiscaisAsync(context);
            }

            if (!await context.ItensNotaFiscal.AnyAsync())
            {
                Console.WriteLine("🌱 Seed: Itens de Nota Fiscal...");
                await SeedItensNotaFiscalAsync(context);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro durante o seed: {ex.Message}");
            throw;
        }
    }

    private static async Task SeedNotasFiscaisAsync(AppDbContext context)
    {
        var notas = new List<NotaFiscal>
        {
            new() { Numero = "NF-0001", Status = TipoStatusNF.Aberta, DataEmissao = DateTime.UtcNow },
            new() { Numero = "NF-0002", Status = TipoStatusNF.Aberta, DataEmissao = DateTime.UtcNow },
            new() { Numero = "NF-0003", Status = TipoStatusNF.Fechada, DataEmissao = DateTime.UtcNow.AddDays(-1) }
        };

        await context.NotasFiscais.AddRangeAsync(notas);
        await context.SaveChangesAsync();
    }

    private static async Task SeedItensNotaFiscalAsync(AppDbContext context)
    {
        var notas = await context.NotasFiscais.ToListAsync();

        var itens = new List<ItemNotaFiscal>
        {
            new() { NotaFiscalId = notas[0].Id, ProdutoId = 1, Quantidade = 2 },
            new() { NotaFiscalId = notas[0].Id, ProdutoId = 2, Quantidade = 1 },
            new() { NotaFiscalId = notas[1].Id, ProdutoId = 3, Quantidade = 1 },
            new() { NotaFiscalId = notas[2].Id, ProdutoId = 4, Quantidade = 1 },
            new() { NotaFiscalId = notas[2].Id, ProdutoId = 5, Quantidade = 2 }
        };

        await context.ItensNotaFiscal.AddRangeAsync(itens);
        await context.SaveChangesAsync();
    }
}