using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolYa.WebAPI.Migrations
{
    public partial class FixPartidoOrganizador : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxJugadores",
                table: "Partidos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxJugadores",
                table: "Partidos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
