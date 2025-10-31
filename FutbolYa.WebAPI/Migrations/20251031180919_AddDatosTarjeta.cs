using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolYa.WebAPI.Migrations
{
    public partial class AddDatosTarjeta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DatosTarjetas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservaId = table.Column<int>(type: "int", nullable: false),
                    HashToken = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    HashNumero = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Ultimos4 = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    HashCvv = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    NombreTitular = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaExpiracion = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    FechaRegistroUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatosTarjetas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatosTarjetas_Reservas_ReservaId",
                        column: x => x.ReservaId,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatosTarjetas_ReservaId",
                table: "DatosTarjetas",
                column: "ReservaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatosTarjetas");
        }
    }
}
