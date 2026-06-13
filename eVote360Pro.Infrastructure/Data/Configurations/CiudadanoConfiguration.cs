using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Infrastructure.Data.Configurations;

public class CiudadanoConfiguration : IEntityTypeConfiguration<Ciudadano>
{
    public void Configure(EntityTypeBuilder<Ciudadano> builder)
    {
        builder.ToTable("Ciudadanos");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre).IsRequired().HasMaxLength(100);

        builder.Property(c => c.Apellido).IsRequired().HasMaxLength(100);

        // Regla: El documento de identidad debe ser String y Único
        builder.Property(c => c.NumeroDocumento)
               .IsRequired()
               .HasMaxLength(20);

        builder.HasIndex(c => c.NumeroDocumento).IsUnique();

        builder.Property(c => c.CorreoElectronico)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(c => c.Activo).HasDefaultValue(true);
    }
}