using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolYa.WebAPI.Migrations
{
    public partial class MensajesOpcionales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ReservaId",
                table: "Mensajes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PartidoId",
                table: "Mensajes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_ReservaId",
                table: "Mensajes",
                column: "ReservaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Reservas_ReservaId",
                table: "Mensajes",
                column: "ReservaId",
                principalTable: "Reservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Reservas_ReservaId",
                table: "Mensajes");

            migrationBuilder.DropIndex(
                name: "IX_Mensajes_ReservaId",
                table: "Mensajes");

            migrationBuilder.AlterColumn<int>(
                name: "ReservaId",
                table: "Mensajes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PartidoId",
                table: "Mensajes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
