using FaturamentoService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaturamentoService.Infrastructure.Data.Mappings;

public class ItemNotaFiscalMapping : IEntityTypeConfiguration<ItemNotaFiscal>
{
    public void Configure(EntityTypeBuilder<ItemNotaFiscal> builder)
    {
        builder.ToTable("ItensNotaFiscal");

        // PK
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
               .ValueGeneratedOnAdd()
               .HasComment("Identificador do item da nota fiscal");

        // Propriedades
        builder.Property(i => i.NotaFiscalId)
               .IsRequired()
               .HasComment("FK para nota fiscal");

        builder.Property(i => i.ProdutoId)
               .IsRequired()
               .HasComment("Id do produto cadastrado no EstoqueService");

        builder.Property(i => i.Quantidade)
               .IsRequired()
               .HasDefaultValue(1)
               .HasComment("Quantidade do produto na nota fiscal");

        // Índices
        builder.HasIndex(i => i.NotaFiscalId)
               .HasDatabaseName("IX_ItensNotaFiscal_NotaFiscalId");

        builder.HasIndex(i => i.ProdutoId)
               .HasDatabaseName("IX_ItensNotaFiscal_ProdutoId");
    }
}