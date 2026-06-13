using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360Pro.Domain.Entities;

namespace eVote360Pro.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nombre)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.Apellido)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.CorreoElectronico)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(u => u.NombreUsuario)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(u => u.PasswordHash)
               .IsRequired();

        builder.Property(u => u.Rol)
               .IsRequired()
               .HasConversion<int>();

        builder.Property(u => u.Activo)
               .IsRequired();

        builder.HasIndex(u => u.NombreUsuario).IsUnique();
        builder.HasIndex(u => u.CorreoElectronico).IsUnique();
    }
}
