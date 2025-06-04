using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolYa.WebAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Puntaje = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calificaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Partidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ubicacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nivel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxJugadores = table.Column<int>(type: "int", nullable: false),
                    OrganizadorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contraseña = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Posicion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FotoPerfil = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartidoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_UsuarioId",
                table: "Calificaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_OrganizadorId",
                table: "Partidos",
                column: "OrganizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_PartidoId",
                table: "Usuarios",
                column: "PartidoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Usuarios_UsuarioId",
                table: "Calificaciones",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Usuarios_OrganizadorId",
                table: "Partidos",
                column: "OrganizadorId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Usuarios_OrganizadorId",
                table: "Partidos");

            migrationBuilder.DropTable(
                name: "Calificaciones");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Partidos");
        }
    }
}
