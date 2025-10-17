using EstoqueService.Core.Enums;
using EstoqueService.Models;
using EstoqueService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace EstoqueService.Infrastructure.Data.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        try
        {
            if (!await context.Produtos.AnyAsync())
            {
                Console.WriteLine("🌱 Seed: Produtos...");
                await SeedProdutosAsync(context);
            }

            if (!await context.MovimentacoesEstoque.AnyAsync())
            {
                Console.WriteLine("🌱 Seed: Movimentações...");
                await SeedMovimentacoesAsync(context);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro durante o seed: {ex.Message}");
            throw;
        }
    }

    private static async Task SeedProdutosAsync(AppDbContext context)
    {
        var produtos = new List<Produto>
        {
            new() { Descricao = "Notebook Dell Inspiron", CodigoSKU = "NTB-DEL-001", Preco = 3500.00m, Saldo = 10 },
            new() { Descricao = "Mouse Logitech M170", CodigoSKU = "MSE-LOG-002", Preco = 75.50m, Saldo = 50 },
            new() { Descricao = "Teclado Mecânico Redragon", CodigoSKU = "TCL-RED-003", Preco = 220.00m, Saldo = 20 },
            new() { Descricao = "Monitor LG 24''", CodigoSKU = "MON-LG-004", Preco = 899.99m, Saldo = 15 },
            new() { Descricao = "Headset HyperX Cloud", CodigoSKU = "HST-HYP-005", Preco = 399.90m, Saldo = 8 }
        };

        await context.Produtos.AddRangeAsync(produtos);
        await context.SaveChangesAsync();
    }

    private static async Task SeedMovimentacoesAsync(AppDbContext context)
    {
        var produtos = await context.Produtos.ToListAsync();

        var movimentacoes = new List<MovimentacaoEstoque>
        {
            new() { ProdutoId = produtos[0].Id, Quantidade = 10, Tipo = TipoMovimentacoesEstoque.Entrada, Observacao = "Estoque inicial Notebook Dell" },
            new() { ProdutoId = produtos[1].Id, Quantidade = 50, Tipo = TipoMovimentacoesEstoque.Entrada, Observacao = "Estoque inicial Mouse Logitech" },
            new() { ProdutoId = produtos[2].Id, Quantidade = 20, Tipo = TipoMovimentacoesEstoque.Entrada, Observacao = "Estoque inicial Teclado Redragon" },
            new() { ProdutoId = produtos[3].Id, Quantidade = 15, Tipo = TipoMovimentacoesEstoque.Entrada, Observacao = "Estoque inicial Monitor LG" },
            new() { ProdutoId = produtos[4].Id, Quantidade = 8, Tipo = TipoMovimentacoesEstoque.Entrada, Observacao = "Estoque inicial Headset HyperX" }
        };

        await context.MovimentacoesEstoque.AddRangeAsync(movimentacoes);
        await context.SaveChangesAsync();
    }
}
