using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstoqueService.Migrations
{
    /// <inheritdoc />
    public partial class _Initial_EstoqueDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Identificador do produto")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false, comment: "Descrição do produto"),
                    CodigoSKU = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, comment: "Código SKU do produto"),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m, comment: "Preço unitário"),
                    Saldo = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "Saldo disponível em estoque"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true, comment: "Token de concorrência para controle otimista")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovimentacoesEstoque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Identificador da movimentação")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<int>(type: "int", nullable: false, comment: "FK para produto"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()", comment: "Data da movimentação"),
                    Quantidade = table.Column<int>(type: "int", nullable: false, comment: "Quantidade movimentada"),
                    Tipo = table.Column<int>(type: "int", nullable: false, comment: "Tipo da movimentação: 0=Entrada,1=Saida"),
                    Observacao = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true, comment: "Observações da movimentação")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimentacoesEstoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movimentacoes_Produtos",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movimentacoes_Data",
                table: "MovimentacoesEstoque",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_Movimentacoes_ProdutoId",
                table: "MovimentacoesEstoque",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "UX_Produtos_CodigoSKU",
                table: "Produtos",
                column: "CodigoSKU",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimentacoesEstoque");

            migrationBuilder.DropTable(
                name: "Produtos");
        }
    }
}
