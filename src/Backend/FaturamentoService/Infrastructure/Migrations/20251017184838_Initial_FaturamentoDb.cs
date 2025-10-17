using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FaturamentoService.Migrations
{
    /// <inheritdoc />
    public partial class Initial_FaturamentoDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotasFiscais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Identificador da nota fiscal")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, comment: "Número da nota fiscal"),
                    Status = table.Column<int>(type: "int", nullable: false, comment: "Status da nota fiscal: 0=Aberta, 1=Fechada"),
                    DataEmissao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()", comment: "Data de emissão da nota fiscal")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasFiscais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItensNotaFiscal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Identificador do item da nota fiscal")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotaFiscalId = table.Column<int>(type: "int", nullable: false, comment: "FK para nota fiscal"),
                    ProdutoId = table.Column<int>(type: "int", nullable: false, comment: "Id do produto cadastrado no EstoqueService"),
                    Quantidade = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "Quantidade do produto na nota fiscal")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensNotaFiscal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensNotaFiscal_NotaFiscal",
                        column: x => x.NotaFiscalId,
                        principalTable: "NotasFiscais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensNotaFiscal_NotaFiscalId",
                table: "ItensNotaFiscal",
                column: "NotaFiscalId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensNotaFiscal_ProdutoId",
                table: "ItensNotaFiscal",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "UX_NotaFiscal_Numero",
                table: "NotasFiscais",
                column: "Numero",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensNotaFiscal");

            migrationBuilder.DropTable(
                name: "NotasFiscais");
        }
    }
}
