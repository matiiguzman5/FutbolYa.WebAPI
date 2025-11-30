using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolYa.WebAPI.Migrations
{
    public partial class CambiarPartidoIdAReservaId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Partidos_PartidoId",
                table: "Calificaciones");

            migrationBuilder.RenameColumn(
                name: "PartidoId",
                table: "Calificaciones",
                newName: "ReservaId");

            migrationBuilder.RenameIndex(
                name: "IX_Calificaciones_PartidoId",
                table: "Calificaciones",
                newName: "IX_Calificaciones_ReservaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Reservas_ReservaId",
                table: "Calificaciones",
                column: "ReservaId",
                principalTable: "Reservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Reservas_ReservaId",
                table: "Calificaciones");

            migrationBuilder.RenameColumn(
                name: "ReservaId",
                table: "Calificaciones",
                newName: "PartidoId");

            migrationBuilder.RenameIndex(
                name: "IX_Calificaciones_ReservaId",
                table: "Calificaciones",
                newName: "IX_Calificaciones_PartidoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Partidos_PartidoId",
                table: "Calificaciones",
                column: "PartidoId",
                principalTable: "Partidos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
