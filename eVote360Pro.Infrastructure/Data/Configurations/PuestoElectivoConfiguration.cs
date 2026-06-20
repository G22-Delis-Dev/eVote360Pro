using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Infrastructure.Data.Configurations;

public class PuestoElectivoConfiguration : IEntityTypeConfiguration<PuestoElectivo>
{
    public void Configure(EntityTypeBuilder<PuestoElectivo> builder)
    {
        builder.ToTable("PuestosElectivos");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(p => p.Activo)
               .IsRequired();

        builder.HasIndex(p => p.Nombre).IsUnique();
    }
}