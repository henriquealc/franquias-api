using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Franquias.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPercentualRoyaltyEEntidadeRoyalty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PercentualRoyalty",
                table: "Franqueadoras",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Royalties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PeriodoInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PeriodoFim = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Faturamento = table.Column<decimal>(type: "TEXT", nullable: false),
                    PercentualAplicado = table.Column<decimal>(type: "TEXT", nullable: false),
                    ValorRoyalty = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    DataCalculo = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UnidadeFranqueadaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Royalties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Royalties_UnidadesFranqueadas_UnidadeFranqueadaId",
                        column: x => x.UnidadeFranqueadaId,
                        principalTable: "UnidadesFranqueadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Royalties_UnidadeFranqueadaId",
                table: "Royalties",
                column: "UnidadeFranqueadaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Royalties");

            migrationBuilder.DropColumn(
                name: "PercentualRoyalty",
                table: "Franqueadoras");
        }
    }
}
