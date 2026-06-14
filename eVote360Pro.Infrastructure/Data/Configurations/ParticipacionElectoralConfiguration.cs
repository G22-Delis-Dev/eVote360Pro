using eVote360Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Data.Configurations;

public class ParticipacionElectoralConfiguration : IEntityTypeConfiguration<ParticipacionElectoral>
{
    public void Configure(EntityTypeBuilder<ParticipacionElectoral> builder)
    {
        builder.ToTable("ParticipacionesElectorales");
        builder.HasKey(p => p.Id);

        // Un ciudadano solo puede tener una participación por elección
        builder.HasIndex(p => new { p.CiudadanoId, p.EleccionId }).IsUnique();

        builder.Property(p => p.FechaVoto).IsRequired();

        builder.HasOne(p => p.Ciudadano)
               .WithMany(c => c.ParticipacionesElectorales)
               .HasForeignKey(p => p.CiudadanoId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Eleccion)
               .WithMany(e => e.ParticipacionesElectorales)
               .HasForeignKey(p => p.EleccionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}