using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolYa.WebAPI.Migrations
{
    public partial class AgregarCamposPagoReserva : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
         
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaPago",
                table: "Reservas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetodoPago",
                table: "Reservas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "SedeConfirmoTransferencia",
                table: "Reservas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaPago",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "MetodoPago",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "SedeConfirmoTransferencia",
                table: "Reservas");
        }
    }
}
