using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolYa.WebAPI.Migrations
{
    public partial class relacionusuariopartido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Usuarios_OrganizadorId",
                table: "Partidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Partidos_PartidoId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_PartidoId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "PartidoId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "MaxJugadores",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "Nivel",
                table: "Partidos");

            // ✅ Eliminamos defaultValue: 0
            migrationBuilder.AlterColumn<int>(
                name: "OrganizadorId",
                table: "Partidos",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PartidoUsuario",
                columns: table => new
                {
                    JugadoresId = table.Column<int>(type: "int", nullable: false),
                    PartidosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartidoUsuario", x => new { x.JugadoresId, x.PartidosId });
                    table.ForeignKey(
                        name: "FK_PartidoUsuario_Partidos_PartidosId",
                        column: x => x.PartidosId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartidoUsuario_Usuarios_JugadoresId",
                        column: x => x.JugadoresId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartidoUsuario_PartidosId",
                table: "PartidoUsuario",
                column: "PartidosId");

            // ✅ FK correcta sin defaultValue
            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Usuarios_OrganizadorId",
                table: "Partidos",
                column: "OrganizadorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

    }
}
