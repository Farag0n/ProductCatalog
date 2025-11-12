using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infraestructure.Data.Configurations;

public class ProductConfigurations : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        //Se le especifica a la db que la tabla tiene una llave y es p. Id
        builder.HasKey(p => p.Id);
        
        //Se especifica si una propiedad es requerida y el tamaño maximo de la cadena
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Price)
            .IsRequired();

        builder.Property(p => p.Quantity)
            .IsRequired();
        
        //Crear producto de prueba basico:

    }
}