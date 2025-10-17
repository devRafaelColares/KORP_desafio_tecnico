using FaturamentoService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaturamentoService.Infrastructure.Data.Mappings;

public class NotaFiscalMapping : IEntityTypeConfiguration<NotaFiscal>
{
    public void Configure(EntityTypeBuilder<NotaFiscal> builder)
    {
        builder.ToTable("NotasFiscais");

        // PK
        builder.HasKey(nf => nf.Id);
        builder.Property(nf => nf.Id)
               .ValueGeneratedOnAdd()
               .HasComment("Identificador da nota fiscal");

        // Propriedades
        builder.Property(nf => nf.Numero)
               .IsRequired()
               .HasMaxLength(50)
               .IsUnicode(false)
               .HasComment("Número da nota fiscal");

        builder.HasIndex(nf => nf.Numero)
               .IsUnique()
               .HasDatabaseName("UX_NotaFiscal_Numero");

        builder.Property(nf => nf.Status)
               .IsRequired()
               .HasConversion<int>() // Enum como int
               .HasComment("Status da nota fiscal: 0=Aberta, 1=Fechada");

        builder.Property(nf => nf.DataEmissao)
               .IsRequired()
               .HasColumnType("datetime2")
               .HasDefaultValueSql("GETDATE()")
               .HasComment("Data de emissão da nota fiscal");

        // Relacionamento com itens
        builder.HasMany(nf => nf.Itens)
               .WithOne(i => i.NotaFiscal)
               .HasForeignKey(i => i.NotaFiscalId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("FK_ItensNotaFiscal_NotaFiscal");
    }
}