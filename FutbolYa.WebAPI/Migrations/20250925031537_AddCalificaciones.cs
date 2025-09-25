using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolYa.WebAPI.Migrations
{
    public partial class AddCalificaciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Usuarios_UsuarioId",
                table: "Calificaciones");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Calificaciones",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "EvaluadoId",
                table: "Calificaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EvaluadorId",
                table: "Calificaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "Calificaciones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PartidoId",
                table: "Calificaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_EvaluadoId",
                table: "Calificaciones",
                column: "EvaluadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_EvaluadorId",
                table: "Calificaciones",
                column: "EvaluadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_PartidoId",
                table: "Calificaciones",
                column: "PartidoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Partidos_PartidoId",
                table: "Calificaciones",
                column: "PartidoId",
                principalTable: "Partidos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Usuarios_EvaluadoId",
                table: "Calificaciones",
                column: "EvaluadoId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Usuarios_EvaluadorId",
                table: "Calificaciones",
                column: "EvaluadorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Usuarios_UsuarioId",
                table: "Calificaciones",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Partidos_PartidoId",
                table: "Calificaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Usuarios_EvaluadoId",
                table: "Calificaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Usuarios_EvaluadorId",
                table: "Calificaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Calificaciones_Usuarios_UsuarioId",
                table: "Calificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Calificaciones_EvaluadoId",
                table: "Calificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Calificaciones_EvaluadorId",
                table: "Calificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Calificaciones_PartidoId",
                table: "Calificaciones");

            migrationBuilder.DropColumn(
                name: "EvaluadoId",
                table: "Calificaciones");

            migrationBuilder.DropColumn(
                name: "EvaluadorId",
                table: "Calificaciones");

            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "Calificaciones");

            migrationBuilder.DropColumn(
                name: "PartidoId",
                table: "Calificaciones");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Calificaciones",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Calificaciones_Usuarios_UsuarioId",
                table: "Calificaciones",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
