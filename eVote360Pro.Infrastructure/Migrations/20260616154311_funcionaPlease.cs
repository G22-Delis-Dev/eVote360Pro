using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVote360Pro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class funcionaPlease : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Votos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "ParticipacionesElectorales",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "EleccionPuestos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Elecciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "CodigosVerificacion",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "AsignacionesDirigentes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "AsignacionesCandidatoPuesto",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "AlianzasPoliticas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Votos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "ParticipacionesElectorales");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "EleccionPuestos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Elecciones");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "CodigosVerificacion");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "AsignacionesDirigentes");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "AsignacionesCandidatoPuesto");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "AlianzasPoliticas");
        }
    }
}
