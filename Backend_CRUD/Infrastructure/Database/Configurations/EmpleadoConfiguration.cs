using Backend_CRUD.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_CRUD.Infrastructure.Database.Configurations
{
    public class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
    {
        public void Configure(EntityTypeBuilder<Empleado> builder)
        {
            builder.ToTable("Empleados");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            builder.Property(e => e.Nombre)
                .HasColumnName("Nombre")
                .IsRequired();

            builder.Property(e => e.Puesto)
                .HasColumnName("Puesto")
                .IsRequired();

            builder.Property(e => e.Contraseña)
                .HasColumnName("Contraseña")
                .IsRequired();
        }
    }
}