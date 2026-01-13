using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace INVEST.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NivelQualidade",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NivelQualidade", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Setores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposIndicadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposIndicadores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Acoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AnoEntrada = table.Column<short>(type: "smallint", nullable: false),
                    Estatal = table.Column<bool>(type: "boolean", nullable: false),
                    SetorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Acoes_Setores_SetorId",
                        column: x => x.SetorId,
                        principalTable: "Setores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QualidadeSetor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SetorId = table.Column<int>(type: "integer", nullable: false),
                    NivelQualidadeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualidadeSetor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualidadeSetor_NivelQualidade_NivelQualidadeId",
                        column: x => x.NivelQualidadeId,
                        principalTable: "NivelQualidade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QualidadeSetor_Setores_SetorId",
                        column: x => x.SetorId,
                        principalTable: "Setores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QualidadeIndicador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoIndicadorId = table.Column<int>(type: "integer", nullable: false),
                    NivelQualidadeId = table.Column<int>(type: "integer", nullable: false),
                    ValorMinimo = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorMaximo = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualidadeIndicador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualidadeIndicador_NivelQualidade_NivelQualidadeId",
                        column: x => x.NivelQualidadeId,
                        principalTable: "NivelQualidade",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QualidadeIndicador_TiposIndicadores_TipoIndicadorId",
                        column: x => x.TipoIndicadorId,
                        principalTable: "TiposIndicadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tickers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    AcaoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickers_Acoes_AcaoId",
                        column: x => x.AcaoId,
                        principalTable: "Acoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Indicador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TickerId = table.Column<int>(type: "integer", nullable: false),
                    TipoIndicadorId = table.Column<int>(type: "integer", nullable: false),
                    ValorDecimal = table.Column<decimal>(type: "numeric", nullable: true),
                    ValorBool = table.Column<bool>(type: "boolean", nullable: true),
                    ValorShort = table.Column<short>(type: "smallint", nullable: true),
                    DataRegistro = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indicador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Indicador_Tickers_TickerId",
                        column: x => x.TickerId,
                        principalTable: "Tickers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Indicador_TiposIndicadores_TipoIndicadorId",
                        column: x => x.TipoIndicadorId,
                        principalTable: "TiposIndicadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Acoes_SetorId",
                table: "Acoes",
                column: "SetorId");

            migrationBuilder.CreateIndex(
                name: "IX_Indicador_TickerId",
                table: "Indicador",
                column: "TickerId");

            migrationBuilder.CreateIndex(
                name: "IX_Indicador_TipoIndicadorId",
                table: "Indicador",
                column: "TipoIndicadorId");

            migrationBuilder.CreateIndex(
                name: "IX_QualidadeIndicador_NivelQualidadeId",
                table: "QualidadeIndicador",
                column: "NivelQualidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_QualidadeIndicador_TipoIndicadorId",
                table: "QualidadeIndicador",
                column: "TipoIndicadorId");

            migrationBuilder.CreateIndex(
                name: "IX_QualidadeSetor_NivelQualidadeId",
                table: "QualidadeSetor",
                column: "NivelQualidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_QualidadeSetor_SetorId",
                table: "QualidadeSetor",
                column: "SetorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickers_AcaoId",
                table: "Tickers",
                column: "AcaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Indicador");

            migrationBuilder.DropTable(
                name: "QualidadeIndicador");

            migrationBuilder.DropTable(
                name: "QualidadeSetor");

            migrationBuilder.DropTable(
                name: "Tickers");

            migrationBuilder.DropTable(
                name: "TiposIndicadores");

            migrationBuilder.DropTable(
                name: "NivelQualidade");

            migrationBuilder.DropTable(
                name: "Acoes");

            migrationBuilder.DropTable(
                name: "Setores");
        }
    }
}
