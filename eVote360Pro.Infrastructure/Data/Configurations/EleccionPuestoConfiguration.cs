using eVote360Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360Pro.Infrastructure.Data.Configurations;

public class EleccionPuestoConfiguration : IEntityTypeConfiguration<EleccionPuesto>
{
    public void Configure(EntityTypeBuilder<EleccionPuesto> builder)
    {
        builder.HasKey(ep => ep.Id);

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