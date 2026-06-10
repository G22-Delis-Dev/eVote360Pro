using eVote360Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Data.Configurations;

public class AlianzaPoliticaConfiguration : IEntityTypeConfiguration<AlianzaPolitica>
{
    public void Configure(EntityTypeBuilder<AlianzaPolitica> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Estado)
               .IsRequired()
               .HasConversion<int>();

        builder.Property(a => a.FechaRespuesta);

        builder.HasOne(a => a.PartidoSolicitante)
               .WithMany(p => p.AlianzasComoSolicitante)
               .HasForeignKey(a => a.PartidoSolicitanteId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.PartidoReceptor)
               .WithMany(p => p.AlianzasComoReceptor)
               .HasForeignKey(a => a.PartidoReceptorId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}