using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolYa.WebAPI.Migrations
{
    public partial class RemoveCamposExtrasDeCanchas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloquesMantenimiento",
                table: "Canchas");

            migrationBuilder.DropColumn(
                name: "DiasNoDisponibles",
                table: "Canchas");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Canchas");

            migrationBuilder.DropColumn(
                name: "EstadoEquipamiento",
                table: "Canchas");

            migrationBuilder.DropColumn(
                name: "HorarioApertura",
                table: "Canchas");

            migrationBuilder.DropColumn(
                name: "HorarioCierre",
                table: "Canchas");

            migrationBuilder.DropColumn(
                name: "LogReparaciones",
                table: "Canchas");

            migrationBuilder.DropColumn(
                name: "NotasEspeciales",
                table: "Canchas");

            migrationBuilder.DropColumn(
                name: "PrecioNocturno",
                table: "Canchas");

            migrationBuilder.DropColumn(
                name: "PrecioPremium",
                table: "Canchas");

            migrationBuilder.DropColumn(
                name: "ProximoMantenimiento",
                table: "Canchas");

            migrationBuilder.AddColumn<string>(
                name: "Ubicacion",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ubicacion",
                table: "Usuarios");

            migrationBuilder.AddColumn<string>(
                name: "BloquesMantenimiento",
                table: "Canchas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiasNoDisponibles",
                table: "Canchas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Canchas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EstadoEquipamiento",
                table: "Canchas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HorarioApertura",
                table: "Canchas",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HorarioCierre",
                table: "Canchas",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "LogReparaciones",
                table: "Canchas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotasEspeciales",
                table: "Canchas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioNocturno",
                table: "Canchas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioPremium",
                table: "Canchas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProximoMantenimiento",
                table: "Canchas",
                type: "datetime2",
                nullable: true);
        }
    }
}
