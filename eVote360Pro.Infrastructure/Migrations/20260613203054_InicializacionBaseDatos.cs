using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVote360Pro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InicializacionBaseDatos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ciudadanos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ciudadanos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Elecciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaRealizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaActivacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFinalizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elecciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartidosPoliticos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Siglas = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LogoRuta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartidosPoliticos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PuestosElectivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuestosElectivos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CodigosVerificacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CiudadanoId = table.Column<int>(type: "int", nullable: false),
                    EleccionId = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Utilizado = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodigosVerificacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodigosVerificacion_Ciudadanos_CiudadanoId",
                        column: x => x.CiudadanoId,
                        principalTable: "Ciudadanos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CodigosVerificacion_Elecciones_EleccionId",
                        column: x => x.EleccionId,
                        principalTable: "Elecciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParticipacionesElectorales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CiudadanoId = table.Column<int>(type: "int", nullable: false),
                    EleccionId = table.Column<int>(type: "int", nullable: false),
                    FechaVoto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipacionesElectorales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipacionesElectorales_Ciudadanos_CiudadanoId",
                        column: x => x.CiudadanoId,
                        principalTable: "Ciudadanos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParticipacionesElectorales_Elecciones_EleccionId",
                        column: x => x.EleccionId,
                        principalTable: "Elecciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlianzasPoliticas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartidoSolicitanteId = table.Column<int>(type: "int", nullable: false),
                    PartidoReceptorId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlianzasPoliticas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlianzasPoliticas_PartidosPoliticos_PartidoReceptorId",
                        column: x => x.PartidoReceptorId,
                        principalTable: "PartidosPoliticos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AlianzasPoliticas_PartidosPoliticos_PartidoSolicitanteId",
                        column: x => x.PartidoSolicitanteId,
                        principalTable: "PartidosPoliticos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Candidatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FotoRuta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PartidoPoliticoId = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidatos_PartidosPoliticos_PartidoPoliticoId",
                        column: x => x.PartidoPoliticoId,
                        principalTable: "PartidosPoliticos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EleccionPuestos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EleccionId = table.Column<int>(type: "int", nullable: false),
                    PuestoElectivoId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EleccionPuestos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EleccionPuestos_Elecciones_EleccionId",
                        column: x => x.EleccionId,
                        principalTable: "Elecciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EleccionPuestos_PuestosElectivos_PuestoElectivoId",
                        column: x => x.PuestoElectivoId,
                        principalTable: "PuestosElectivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AsignacionesDirigentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    PartidoPoliticoId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignacionesDirigentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsignacionesDirigentes_PartidosPoliticos_PartidoPoliticoId",
                        column: x => x.PartidoPoliticoId,
                        principalTable: "PartidosPoliticos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsignacionesDirigentes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AsignacionesCandidatoPuesto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidatoId = table.Column<int>(type: "int", nullable: false),
                    PuestoElectivoId = table.Column<int>(type: "int", nullable: false),
                    PartidoPoliticoId = table.Column<int>(type: "int", nullable: false),
                    EsAliado = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignacionesCandidatoPuesto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsignacionesCandidatoPuesto_Candidatos_CandidatoId",
                        column: x => x.CandidatoId,
                        principalTable: "Candidatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsignacionesCandidatoPuesto_PartidosPoliticos_PartidoPoliticoId",
                        column: x => x.PartidoPoliticoId,
                        principalTable: "PartidosPoliticos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsignacionesCandidatoPuesto_PuestosElectivos_PuestoElectivoId",
                        column: x => x.PuestoElectivoId,
                        principalTable: "PuestosElectivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Votos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EleccionId = table.Column<int>(type: "int", nullable: false),
                    PuestoElectivoId = table.Column<int>(type: "int", nullable: false),
                    CandidatoId = table.Column<int>(type: "int", nullable: true),
                    PartidoPoliticoId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votos_Candidatos_CandidatoId",
                        column: x => x.CandidatoId,
                        principalTable: "Candidatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Votos_Elecciones_EleccionId",
                        column: x => x.EleccionId,
                        principalTable: "Elecciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Votos_PartidosPoliticos_PartidoPoliticoId",
                        column: x => x.PartidoPoliticoId,
                        principalTable: "PartidosPoliticos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Votos_PuestosElectivos_PuestoElectivoId",
                        column: x => x.PuestoElectivoId,
                        principalTable: "PuestosElectivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlianzasPoliticas_PartidoReceptorId",
                table: "AlianzasPoliticas",
                column: "PartidoReceptorId");

            migrationBuilder.CreateIndex(
                name: "IX_AlianzasPoliticas_PartidoSolicitanteId",
                table: "AlianzasPoliticas",
                column: "PartidoSolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesCandidatoPuesto_CandidatoId_PartidoPoliticoId",
                table: "AsignacionesCandidatoPuesto",
                columns: new[] { "CandidatoId", "PartidoPoliticoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesCandidatoPuesto_PartidoPoliticoId",
                table: "AsignacionesCandidatoPuesto",
                column: "PartidoPoliticoId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesCandidatoPuesto_PuestoElectivoId_PartidoPoliticoId",
                table: "AsignacionesCandidatoPuesto",
                columns: new[] { "PuestoElectivoId", "PartidoPoliticoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesDirigentes_PartidoPoliticoId",
                table: "AsignacionesDirigentes",
                column: "PartidoPoliticoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesDirigentes_UsuarioId",
                table: "AsignacionesDirigentes",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_PartidoPoliticoId",
                table: "Candidatos",
                column: "PartidoPoliticoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ciudadanos_NumeroDocumento",
                table: "Ciudadanos",
                column: "NumeroDocumento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CodigosVerificacion_CiudadanoId",
                table: "CodigosVerificacion",
                column: "CiudadanoId");

            migrationBuilder.CreateIndex(
                name: "IX_CodigosVerificacion_EleccionId",
                table: "CodigosVerificacion",
                column: "EleccionId");

            migrationBuilder.CreateIndex(
                name: "IX_EleccionPuestos_EleccionId_PuestoElectivoId",
                table: "EleccionPuestos",
                columns: new[] { "EleccionId", "PuestoElectivoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EleccionPuestos_PuestoElectivoId",
                table: "EleccionPuestos",
                column: "PuestoElectivoId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipacionesElectorales_CiudadanoId_EleccionId",
                table: "ParticipacionesElectorales",
                columns: new[] { "CiudadanoId", "EleccionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipacionesElectorales_EleccionId",
                table: "ParticipacionesElectorales",
                column: "EleccionId");

            migrationBuilder.CreateIndex(
                name: "IX_PartidosPoliticos_Siglas",
                table: "PartidosPoliticos",
                column: "Siglas",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PuestosElectivos_Nombre",
                table: "PuestosElectivos",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_CorreoElectronico",
                table: "Usuarios",
                column: "CorreoElectronico",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NombreUsuario",
                table: "Usuarios",
                column: "NombreUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votos_CandidatoId",
                table: "Votos",
                column: "CandidatoId");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_EleccionId",
                table: "Votos",
                column: "EleccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_PartidoPoliticoId",
                table: "Votos",
                column: "PartidoPoliticoId");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_PuestoElectivoId",
                table: "Votos",
                column: "PuestoElectivoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlianzasPoliticas");

            migrationBuilder.DropTable(
                name: "AsignacionesCandidatoPuesto");

            migrationBuilder.DropTable(
                name: "AsignacionesDirigentes");

            migrationBuilder.DropTable(
                name: "CodigosVerificacion");

            migrationBuilder.DropTable(
                name: "EleccionPuestos");

            migrationBuilder.DropTable(
                name: "ParticipacionesElectorales");

            migrationBuilder.DropTable(
                name: "Votos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Ciudadanos");

            migrationBuilder.DropTable(
                name: "Candidatos");

            migrationBuilder.DropTable(
                name: "Elecciones");

            migrationBuilder.DropTable(
                name: "PuestosElectivos");

            migrationBuilder.DropTable(
                name: "PartidosPoliticos");
        }
    }
}
