using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolYa.WebAPI.Migrations
{
    public partial class AddCanchas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Canchas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Superficie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrecioBaseHora = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioNocturno = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioFinDeSemana = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioPremium = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HorarioApertura = table.Column<TimeSpan>(type: "time", nullable: false),
                    HorarioCierre = table.Column<TimeSpan>(type: "time", nullable: false),
                    BloquesMantenimiento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiasNoDisponibles = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogReparaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoEquipamiento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotasEspeciales = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProximoMantenimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioEstablecimientoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canchas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Canchas_Usuarios_UsuarioEstablecimientoId",
                        column: x => x.UsuarioEstablecimientoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Canchas_UsuarioEstablecimientoId",
                table: "Canchas",
                column: "UsuarioEstablecimientoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Canchas");
        }
    }
}
