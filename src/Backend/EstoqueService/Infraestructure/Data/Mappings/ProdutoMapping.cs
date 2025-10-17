using EstoqueService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstoqueService.Infraestructure.Data.Mappings;

public class ProdutoMapping : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        // PK
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
               .ValueGeneratedOnAdd()
               .HasComment("Identificador do produto");

        // Propriedades
        builder.Property(p => p.Descricao)
               .IsRequired()
               .HasMaxLength(255)
               .IsUnicode(false)
               .HasComment("Descrição do produto");

        builder.Property(p => p.CodigoSKU)
               .IsRequired()
               .HasMaxLength(100)
               .IsUnicode(false)
               .HasComment("Código SKU do produto");

        builder.HasIndex(p => p.CodigoSKU)
               .IsUnique()
               .HasDatabaseName("UX_Produtos_CodigoSKU");

        builder.Property(p => p.Preco)
               .HasColumnType("decimal(18,2)")
               .HasDefaultValue(0m)
               .HasComment("Preço unitário");

        builder.Property(p => p.Saldo)
               .IsRequired()
               .HasDefaultValue(0)
               .HasComment("Saldo disponível em estoque");

        // Concurrency token
        builder.Property(p => p.RowVersion)
               .IsRowVersion()
               .IsConcurrencyToken()
               .ValueGeneratedOnAddOrUpdate()
               .HasColumnType("rowversion")
               .HasComment("Token de concorrência para controle otimista");
    }
}