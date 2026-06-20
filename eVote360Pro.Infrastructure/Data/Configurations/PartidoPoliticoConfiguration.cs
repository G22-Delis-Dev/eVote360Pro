using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Infrastructure.Data.Configurations;

public class PartidoPoliticoConfiguration : IEntityTypeConfiguration<PartidoPolitico>
{
    public void Configure(EntityTypeBuilder<PartidoPolitico> builder)
    {
        builder.ToTable("PartidosPoliticos");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(p => p.Descripcion)
               .HasMaxLength(500);

        builder.Property(p => p.Siglas)
               .IsRequired()
               .HasMaxLength(10);

        builder.Property(p => p.LogoRuta)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(p => p.Activo)
               .IsRequired();

        builder.HasIndex(p => p.Siglas).IsUnique();
    }
}