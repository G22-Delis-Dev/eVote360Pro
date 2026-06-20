using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Infrastructure.Data.Configurations;

public class AsignacionDirigenteConfiguration : IEntityTypeConfiguration<AsignacionDirigente>
{
    public void Configure(EntityTypeBuilder<AsignacionDirigente> builder)
    {
        builder.ToTable("AsignacionesDirigentes");
        builder.HasKey(a => a.Id);

        //  Un usuario solo puede estar asignado a un partido como dirigente a la vez
        builder.HasIndex(a => a.UsuarioId).IsUnique();
        builder.HasIndex(a => a.PartidoPoliticoId).IsUnique();

        builder.HasOne(a => a.Usuario)
               .WithOne(u => u.AsignacionDirigente)
               .HasForeignKey<AsignacionDirigente>(a => a.UsuarioId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.PartidoPolitico)
               .WithOne(p => p.AsignacionDirigente)
               .HasForeignKey<AsignacionDirigente>(a => a.PartidoPoliticoId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}