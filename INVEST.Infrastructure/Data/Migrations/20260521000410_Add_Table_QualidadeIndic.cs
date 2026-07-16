using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace INVEST.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Table_QualidadeIndic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QualidadeIndicador_NivelQualidade_NivelQualidadeId",
                table: "QualidadeIndicador");

            migrationBuilder.DropForeignKey(
                name: "FK_QualidadeIndicador_TiposIndicadores_TipoIndicadorId",
                table: "QualidadeIndicador");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QualidadeIndicador",
                table: "QualidadeIndicador");

            migrationBuilder.RenameTable(
                name: "QualidadeIndicador",
                newName: "QualidadeIndicadores");

            migrationBuilder.RenameIndex(
                name: "IX_QualidadeIndicador_TipoIndicadorId",
                table: "QualidadeIndicadores",
                newName: "IX_QualidadeIndicadores_TipoIndicadorId");

            migrationBuilder.RenameIndex(
                name: "IX_QualidadeIndicador_NivelQualidadeId",
                table: "QualidadeIndicadores",
                newName: "IX_QualidadeIndicadores_NivelQualidadeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QualidadeIndicadores",
                table: "QualidadeIndicadores",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QualidadeIndicadores_NivelQualidade_NivelQualidadeId",
                table: "QualidadeIndicadores",
                column: "NivelQualidadeId",
                principalTable: "NivelQualidade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QualidadeIndicadores_TiposIndicadores_TipoIndicadorId",
                table: "QualidadeIndicadores",
                column: "TipoIndicadorId",
                principalTable: "TiposIndicadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QualidadeIndicadores_NivelQualidade_NivelQualidadeId",
                table: "QualidadeIndicadores");

            migrationBuilder.DropForeignKey(
                name: "FK_QualidadeIndicadores_TiposIndicadores_TipoIndicadorId",
                table: "QualidadeIndicadores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QualidadeIndicadores",
                table: "QualidadeIndicadores");

            migrationBuilder.RenameTable(
                name: "QualidadeIndicadores",
                newName: "QualidadeIndicador");

            migrationBuilder.RenameIndex(
                name: "IX_QualidadeIndicadores_TipoIndicadorId",
                table: "QualidadeIndicador",
                newName: "IX_QualidadeIndicador_TipoIndicadorId");

            migrationBuilder.RenameIndex(
                name: "IX_QualidadeIndicadores_NivelQualidadeId",
                table: "QualidadeIndicador",
                newName: "IX_QualidadeIndicador_NivelQualidadeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QualidadeIndicador",
                table: "QualidadeIndicador",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QualidadeIndicador_NivelQualidade_NivelQualidadeId",
                table: "QualidadeIndicador",
                column: "NivelQualidadeId",
                principalTable: "NivelQualidade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QualidadeIndicador_TiposIndicadores_TipoIndicadorId",
                table: "QualidadeIndicador",
                column: "TipoIndicadorId",
                principalTable: "TiposIndicadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
