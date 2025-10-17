using EstoqueService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstoqueService.Infraestructure.Data.Mappings;

public class MovimentacaoEstoqueMapping : IEntityTypeConfiguration<MovimentacaoEstoque>
{
    public void Configure(EntityTypeBuilder<MovimentacaoEstoque> builder)
    {
        builder.ToTable("MovimentacoesEstoque");

        // PK
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
               .ValueGeneratedOnAdd()
               .HasComment("Identificador da movimentação");

        // Propriedades
        builder.Property(m => m.ProdutoId)
               .IsRequired()
               .HasComment("FK para produto");

        builder.Property(m => m.Quantidade)
               .IsRequired()
               .HasComment("Quantidade movimentada");

        builder.Property(m => m.Data)
               .IsRequired()
               .HasColumnType("datetime2")
               .HasDefaultValueSql("GETDATE()")
               .HasComment("Data da movimentação");

        builder.Property(m => m.Observacao)
               .HasMaxLength(500)
               .IsUnicode(false)
               .HasComment("Observações da movimentação");

        // Enum Tipo -> armazenar como int
        builder.Property(m => m.Tipo)
               .HasConversion<int>()
               .IsRequired()
               .HasComment("Tipo da movimentação: 0=Entrada,1=Saida");

        // Relação com Produto
        builder.HasOne(m => m.Produto)
               .WithMany(p => p.Movimentacoes)
               .HasForeignKey(m => m.ProdutoId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("FK_Movimentacoes_Produtos");

        // Índices
        builder.HasIndex(m => m.ProdutoId)
               .HasDatabaseName("IX_Movimentacoes_ProdutoId");

        builder.HasIndex(m => m.Data)
               .HasDatabaseName("IX_Movimentacoes_Data");
    }
}