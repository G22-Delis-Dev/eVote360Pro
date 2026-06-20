using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Infrastructure.Data.Configurations;

public class EleccionConfiguration : IEntityTypeConfiguration<Eleccion>
{
    public void Configure(EntityTypeBuilder<Eleccion> builder)
    {
        builder.ToTable("Elecciones");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombre)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(e => e.FechaRealizacion)
               .IsRequired();

        builder.Property(e => e.Estado)
               .IsRequired()
               .HasConversion<int>();

        builder.Property(e => e.FechaActivacion);
        builder.Property(e => e.FechaFinalizacion);
    }
}