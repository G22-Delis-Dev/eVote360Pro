using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Infrastructure.Data.Configurations;

public class EleccionPuestoConfiguration : IEntityTypeConfiguration<EleccionPuesto>
{
    public void Configure(EntityTypeBuilder<EleccionPuesto> builder)
    {
        builder.ToTable("EleccionPuestos");
        builder.HasKey(e => e.Id);

        // No se puede agregar el mismo puesto dos veces a la misma elección
        builder.HasIndex(ep => new { ep.EleccionId, ep.PuestoElectivoId }).IsUnique();

        builder.HasOne(ep => ep.Eleccion)
              .WithMany(e => e.EleccionPuestos)
              .HasForeignKey(ep => ep.EleccionId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ep => ep.PuestoElectivo)
               .WithMany(p => p.EleccionPuestos)
               .HasForeignKey(ep => ep.PuestoElectivoId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}