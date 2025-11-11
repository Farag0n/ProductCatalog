using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Enums;

namespace ProductCatalog.Infraestructure.Data.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        //Se le especifica a la db que la tabla tiene una llave y es u. Id
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(25);
        
        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(15);
        
        //Crear un admin de prueba:
        builder.HasData(new User
        {
            Id = 1,
            Name = "Zoro",
            LastName = "Rorronoa",
            Email = "rorronoa.zoro@muguiwara.com",
            UserName = "Espadachin",
            PasswordHash = "3-Espadas",
            Role = UserRole.Admin
        });
    }
}