using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolYa.WebAPI.Migrations
{
    public partial class AgregarRendimientos2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rendimientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartidoId = table.Column<int>(type: "int", nullable: false),
                    EvaluadorId = table.Column<int>(type: "int", nullable: false),
                    EvaluadoId = table.Column<int>(type: "int", nullable: false),
                    Actitud = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pase = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Defensa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrabajoEquipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Puntualidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rendimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rendimientos_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rendimientos_Usuarios_EvaluadoId",
                        column: x => x.EvaluadoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rendimientos_Usuarios_EvaluadorId",
                        column: x => x.EvaluadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rendimientos_EvaluadoId",
                table: "Rendimientos",
                column: "EvaluadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Rendimientos_EvaluadorId",
                table: "Rendimientos",
                column: "EvaluadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Rendimientos_PartidoId",
                table: "Rendimientos",
                column: "PartidoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rendimientos");
        }
    }
}
