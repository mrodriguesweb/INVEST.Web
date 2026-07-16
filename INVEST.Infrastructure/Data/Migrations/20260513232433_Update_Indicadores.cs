using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace INVEST.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Indicadores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Indicador_Tickers_TickerId",
                table: "Indicador");

            migrationBuilder.DropForeignKey(
                name: "FK_Indicador_TiposIndicadores_TipoIndicadorId",
                table: "Indicador");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Indicador",
                table: "Indicador");

            migrationBuilder.RenameTable(
                name: "Indicador",
                newName: "Indicadores");

            migrationBuilder.RenameIndex(
                name: "IX_Indicador_TipoIndicadorId",
                table: "Indicadores",
                newName: "IX_Indicadores_TipoIndicadorId");

            migrationBuilder.RenameIndex(
                name: "IX_Indicador_TickerId",
                table: "Indicadores",
                newName: "IX_Indicadores_TickerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Indicadores",
                table: "Indicadores",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Indicadores_Tickers_TickerId",
                table: "Indicadores",
                column: "TickerId",
                principalTable: "Tickers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Indicadores_TiposIndicadores_TipoIndicadorId",
                table: "Indicadores",
                column: "TipoIndicadorId",
                principalTable: "TiposIndicadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Indicadores_Tickers_TickerId",
                table: "Indicadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Indicadores_TiposIndicadores_TipoIndicadorId",
                table: "Indicadores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Indicadores",
                table: "Indicadores");

            migrationBuilder.RenameTable(
                name: "Indicadores",
                newName: "Indicador");

            migrationBuilder.RenameIndex(
                name: "IX_Indicadores_TipoIndicadorId",
                table: "Indicador",
                newName: "IX_Indicador_TipoIndicadorId");

            migrationBuilder.RenameIndex(
                name: "IX_Indicadores_TickerId",
                table: "Indicador",
                newName: "IX_Indicador_TickerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Indicador",
                table: "Indicador",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Indicador_Tickers_TickerId",
                table: "Indicador",
                column: "TickerId",
                principalTable: "Tickers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Indicador_TiposIndicadores_TipoIndicadorId",
                table: "Indicador",
                column: "TipoIndicadorId",
                principalTable: "TiposIndicadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
