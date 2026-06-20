using eVote360Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Data.Configurations;

public class VotoConfiguration : IEntityTypeConfiguration<Voto>
{
    public void Configure(EntityTypeBuilder<Voto> builder)
    {
        builder.ToTable("Votos");
        builder.HasKey(v => v.Id);

        builder.HasOne(v => v.Eleccion)
               .WithMany(e => e.Votos)
               .HasForeignKey(v => v.EleccionId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.PuestoElectivo)
               .WithMany()
               .HasForeignKey(v => v.PuestoElectivoId)
               .OnDelete(DeleteBehavior.Restrict);

        // Nullable — opción "Ninguno"
        builder.HasOne(v => v.Candidato)
               .WithMany()
               .HasForeignKey(v => v.CandidatoId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);

        // Nullable — opción "Ninguno"
        builder.HasOne(v => v.PartidoPolitico)
               .WithMany()
               .HasForeignKey(v => v.PartidoPoliticoId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);
    }
}