using eVote360Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Data.Configurations;

public class AsignacionCandidatoPuestoConfiguration : IEntityTypeConfiguration<AsignacionCandidatoPuesto>
{
    public void Configure(EntityTypeBuilder<AsignacionCandidatoPuesto> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => new { a.CandidatoId, a.PartidoPoliticoId }).IsUnique();
        builder.HasIndex(a => new { a.PuestoElectivoId, a.PartidoPoliticoId }).IsUnique();

        builder.Property(a => a.EsAliado).IsRequired();

        builder.HasOne(a => a.Candidato)
               .WithMany(c => c.AsignacionesPuestos)
               .HasForeignKey(a => a.CandidatoId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.PuestoElectivo)
               .WithMany(p => p.AsignacionesCandidatos)
               .HasForeignKey(a => a.PuestoElectivoId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.PartidoPolitico)
               .WithMany(p => p.AsignacionesCandidatos)
               .HasForeignKey(a => a.PartidoPoliticoId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}