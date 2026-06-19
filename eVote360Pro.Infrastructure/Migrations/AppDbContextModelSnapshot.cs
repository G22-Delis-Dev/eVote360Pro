using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using eVote360Pro.Infrastructure.Data;

#nullable disable

namespace eVote360Pro.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Candidato", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaModificacion")
                        .HasColumnType("datetime2");

                    b.Property<string>("FotoRuta")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("PartidoPoliticoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PartidoPoliticoId");

                    b.ToTable("Candidatos", (string)null);
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.AlianzaPolitica", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<int>("Estado")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaModificacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaRespuesta")
                        .HasColumnType("datetime2");

                    b.Property<int>("PartidoReceptorId")
                        .HasColumnType("int");

                    b.Property<int>("PartidoSolicitanteId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PartidoReceptorId");

                    b.HasIndex("PartidoSolicitanteId");

                    b.ToTable("AlianzasPoliticas", (string)null);
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.AsignacionCandidatoPuesto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<int>("CandidatoId")
                        .HasColumnType("int");

                    b.Property<bool>("EsAliado")
                        .HasColumnType("bit");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaModificacion")
                        .HasColumnType("datetime2");

                    b.Property<int>("PartidoPoliticoId")
                        .HasColumnType("int");

                    b.Property<int>("PuestoElectivoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PartidoPoliticoId");

                    b.HasIndex("CandidatoId", "PartidoPoliticoId")
                        .IsUnique();

                    b.HasIndex("PuestoElectivoId", "PartidoPoliticoId")
                        .IsUnique();

                    b.ToTable("AsignacionesCandidatoPuesto", (string)null);
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.AsignacionDirigente", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaModificacion")
                        .HasColumnType("datetime2");

                    b.Property<int>("PartidoPoliticoId")
                        .HasColumnType("int");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PartidoPoliticoId")
                        .IsUnique();

                    b.HasIndex("UsuarioId")
                        .IsUnique();

                    b.ToTable("AsignacionesDirigentes", (string)null);
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.Ciudadano", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Activo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("CorreoElectronico")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaModificacion")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NumeroDocumento")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("NumeroDocumento")
                        .IsUnique();

                    b.ToTable("Ciudadanos", (string)null);
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.CodigoVerificacion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<int>("CiudadanoId")
                        .HasColumnType("int");

                    b.Property<string>("Codigo")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("nvarchar(6)");

                    b.Property<int>("EleccionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaExpiracion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaGeneracion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaModificacion")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Utilizado")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("CiudadanoId");

                    b.HasIndex("EleccionId");

                    b.ToTable("CodigosVerificacion", (string)null);
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.Eleccion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<int>("Estado")
                        .HasColumnType("int");

                    b.Property<DateTime?>("FechaActivacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaFinalizacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaModificacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaRealizacion")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("Elecciones", (string)null);
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.EleccionPuesto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<int>("EleccionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaModificacion")
                        .HasColumnType("datetime2");

                    b.Property<int>("PuestoElectivoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PuestoElectivoId");

                    b.HasIndex("EleccionId", "PuestoElectivoId")
                        .IsUnique();

                    b.ToTable("EleccionPuestos", (string)null);
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.ParticipacionElectoral", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<int>("CiudadanoId")
                        .HasColumnType("int");

                    b.Property<int>("EleccionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaModificacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaVoto")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("EleccionId");

                    b.HasIndex("CiudadanoId", "EleccionId")
                        .IsUnique();

                    b.ToTable("ParticipacionesElectorales", (string)null);
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.PartidoPolitico", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<string>("Descripcion")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaModificacion")
                        .HasColumnType("datetime2");

                    b.Property<string>("LogoRuta")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Siglas")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Id");

                    b.HasIndex("Siglas")
                        .IsUnique();

                    b.ToTable("PartidosPoliticos", (string)null);
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.PuestoElectivo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaModificacion")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id");

                    b.HasIndex("Nombre")
                        .IsUnique();

                    b.ToTable("PuestosElectivos", (string)null);
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("CorreoElectronico")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaModificacion")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NombreUsuario")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Rol")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CorreoElectronico")
                        .IsUnique();

                    b.HasIndex("NombreUsuario")
                        .IsUnique();

                    b.ToTable("Usuarios", (string)null);
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.Voto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<int?>("CandidatoId")
                        .HasColumnType("int");

                    b.Property<int>("EleccionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaModificacion")
                        .HasColumnType("datetime2");

                    b.Property<int?>("PartidoPoliticoId")
                        .HasColumnType("int");

                    b.Property<int>("PuestoElectivoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CandidatoId");

                    b.HasIndex("EleccionId");

                    b.HasIndex("PartidoPoliticoId");

                    b.HasIndex("PuestoElectivoId");

                    b.ToTable("Votos", (string)null);
                });

            modelBuilder.Entity("Candidato", b =>
                {
                    b.HasOne("eVote360Pro.Domain.Entities.PartidoPolitico", "PartidoPolitico")
                        .WithMany("Candidatos")
                        .HasForeignKey("PartidoPoliticoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("PartidoPolitico");
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.AlianzaPolitica", b =>
                {
                    b.HasOne("eVote360Pro.Domain.Entities.PartidoPolitico", "PartidoReceptor")
                        .WithMany("AlianzasComoReceptor")
                        .HasForeignKey("PartidoReceptorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("eVote360Pro.Domain.Entities.PartidoPolitico", "PartidoSolicitante")
                        .WithMany("AlianzasComoSolicitante")
                        .HasForeignKey("PartidoSolicitanteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("PartidoReceptor");

                    b.Navigation("PartidoSolicitante");
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.AsignacionCandidatoPuesto", b =>
                {
                    b.HasOne("Candidato", "Candidato")
                        .WithMany("AsignacionesPuestos")
                        .HasForeignKey("CandidatoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("eVote360Pro.Domain.Entities.PartidoPolitico", "PartidoPolitico")
                        .WithMany("AsignacionesCandidatos")
                        .HasForeignKey("PartidoPoliticoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("eVote360Pro.Domain.Entities.PuestoElectivo", "PuestoElectivo")
                        .WithMany("AsignacionesCandidatos")
                        .HasForeignKey("PuestoElectivoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Candidato");

                    b.Navigation("PartidoPolitico");

                    b.Navigation("PuestoElectivo");
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.AsignacionDirigente", b =>
                {
                    b.HasOne("eVote360Pro.Domain.Entities.PartidoPolitico", "PartidoPolitico")
                        .WithOne("AsignacionDirigente")
                        .HasForeignKey("eVote360Pro.Domain.Entities.AsignacionDirigente", "PartidoPoliticoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("eVote360Pro.Domain.Entities.Usuario", "Usuario")
                        .WithOne("AsignacionDirigente")
                        .HasForeignKey("eVote360Pro.Domain.Entities.AsignacionDirigente", "UsuarioId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("PartidoPolitico");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.CodigoVerificacion", b =>
                {
                    b.HasOne("eVote360Pro.Domain.Entities.Ciudadano", "Ciudadano")
                        .WithMany("CodigosVerificacion")
                        .HasForeignKey("CiudadanoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("eVote360Pro.Domain.Entities.Eleccion", "Eleccion")
                        .WithMany("CodigosVerificacion")
                        .HasForeignKey("EleccionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Ciudadano");

                    b.Navigation("Eleccion");
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.EleccionPuesto", b =>
                {
                    b.HasOne("eVote360Pro.Domain.Entities.Eleccion", "Eleccion")
                        .WithMany("EleccionPuestos")
                        .HasForeignKey("EleccionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("eVote360Pro.Domain.Entities.PuestoElectivo", "PuestoElectivo")
                        .WithMany("EleccionPuestos")
                        .HasForeignKey("PuestoElectivoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Eleccion");

                    b.Navigation("PuestoElectivo");
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.ParticipacionElectoral", b =>
                {
                    b.HasOne("eVote360Pro.Domain.Entities.Ciudadano", "Ciudadano")
                        .WithMany("ParticipacionesElectorales")
                        .HasForeignKey("CiudadanoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("eVote360Pro.Domain.Entities.Eleccion", "Eleccion")
                        .WithMany("ParticipacionesElectorales")
                        .HasForeignKey("EleccionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Ciudadano");

                    b.Navigation("Eleccion");
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.Voto", b =>
                {
                    b.HasOne("Candidato", "Candidato")
                        .WithMany()
                        .HasForeignKey("CandidatoId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("eVote360Pro.Domain.Entities.Eleccion", "Eleccion")
                        .WithMany("Votos")
                        .HasForeignKey("EleccionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("eVote360Pro.Domain.Entities.PartidoPolitico", "PartidoPolitico")
                        .WithMany()
                        .HasForeignKey("PartidoPoliticoId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("eVote360Pro.Domain.Entities.PuestoElectivo", "PuestoElectivo")
                        .WithMany()
                        .HasForeignKey("PuestoElectivoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Candidato");

                    b.Navigation("Eleccion");

                    b.Navigation("PartidoPolitico");

                    b.Navigation("PuestoElectivo");
                });

            modelBuilder.Entity("Candidato", b =>
                {
                    b.Navigation("AsignacionesPuestos");
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.Ciudadano", b =>
                {
                    b.Navigation("CodigosVerificacion");

                    b.Navigation("ParticipacionesElectorales");
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.Eleccion", b =>
                {
                    b.Navigation("CodigosVerificacion");

                    b.Navigation("EleccionPuestos");

                    b.Navigation("ParticipacionesElectorales");

                    b.Navigation("Votos");
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.PartidoPolitico", b =>
                {
                    b.Navigation("AlianzasComoReceptor");

                    b.Navigation("AlianzasComoSolicitante");

                    b.Navigation("AsignacionDirigente");

                    b.Navigation("AsignacionesCandidatos");

                    b.Navigation("Candidatos");
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.PuestoElectivo", b =>
                {
                    b.Navigation("AsignacionesCandidatos");

                    b.Navigation("EleccionPuestos");
                });

            modelBuilder.Entity("eVote360Pro.Domain.Entities.Usuario", b =>
                {
                    b.Navigation("AsignacionDirigente");
                });
#pragma warning restore 612, 618
        }
    }
}
