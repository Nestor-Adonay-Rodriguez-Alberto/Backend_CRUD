using Backend_CRUD.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend_CRUD.Infrastructure.Database.Configurations
{
    public class SerieConfiguration : IEntityTypeConfiguration<Serie>
    {
        public void Configure(EntityTypeBuilder<Serie> builder)
        {
            builder.ToTable("Series");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            builder.Property(e => e.Titulo)
                .HasColumnName("Titulo")
                .IsRequired();

            builder.Property(e => e.Temporadas)
                .HasColumnName("Temporadas")
                .IsRequired();
        }
    }
}