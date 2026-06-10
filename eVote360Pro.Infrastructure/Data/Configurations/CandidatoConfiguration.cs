using eVote360Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Data.Configurations;

public class CandidatoConfiguration : IEntityTypeConfiguration<Candidato>
{
    public void Configure(EntityTypeBuilder<Candidato> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.Apellido)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.FotoRuta)
               .HasMaxLength(500);

        builder.Property(c => c.Activo)
               .IsRequired();

        builder.HasOne(c => c.PartidoPolitico)
               .WithMany(p => p.Candidatos)
               .HasForeignKey(c => c.PartidoPoliticoId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}