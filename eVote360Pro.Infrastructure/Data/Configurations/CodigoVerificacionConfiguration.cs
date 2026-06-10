using eVote360Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Data.Configurations;

public class CodigoVerificacionConfiguration : IEntityTypeConfiguration<CodigoVerificacion>
{
    public void Configure(EntityTypeBuilder<CodigoVerificacion> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Codigo)
               .IsRequired()
               .HasMaxLength(6);

        builder.Property(c => c.FechaGeneracion).IsRequired();
        builder.Property(c => c.FechaExpiracion).IsRequired();
        builder.Property(c => c.Utilizado).IsRequired();

        builder.HasOne(c => c.Ciudadano)
               .WithMany(ci => ci.CodigosVerificacion)
               .HasForeignKey(c => c.CiudadanoId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Eleccion)
               .WithMany(e => e.CodigosVerificacion)
               .HasForeignKey(c => c.EleccionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}