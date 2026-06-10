using eVote360Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Data.Configurations;

public class AsignacionDirigenteConfiguration : IEntityTypeConfiguration<AsignacionDirigente>
{
    public void Configure(EntityTypeBuilder<AsignacionDirigente> builder)
    {
        builder.HasKey(a => a.Id);

        // 1:1 — un dirigente un partido, un partido un dirigente
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