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
        builder.HasData(
            new User
            {
                Id = 1,
                Name = "Zoro",
                LastName = "Rorronoa",
                Email = "rorronoa.zoro@muguiwara.com",
                UserName = "Espadachin",
                PasswordHash = "3-Espadas", // Usuario de prueba sin hash
                Role = UserRole.Admin
            },
            new User
            {
                Id = 2,
                Name = "string",
                LastName = "string",
                Email = "string",
                UserName = "string",
                PasswordHash = "$2a$11$7N7p3ZL6osWBVL2Jw7q0wO/ruS3g8Fx3rbNAw4yuhJoe/TpCOgez2",
                Role = UserRole.User
            },
            new User
            {
                Id = 3,
                Name = "Anthony",
                LastName = "Stark",
                Email = "tonystark@gmail.com",
                UserName = "Tony",
                PasswordHash = "$2a$11$YOQBT3ykxGQyVkP6ZMy.G.txjSk1cQ4H90T2Nd4xfeiOGOWcrQYC2",
                Role = UserRole.Admin
            },
            new User
            {
                Id = 4,
                Name = "Miguel",
                LastName = "Angarita",
                Email = "miguelanga1604@gmail.com",
                UserName = "faragon",
                PasswordHash = "$2a$11$p8I4wW75WLVUJWVUG4/cGOumqvhyff2.KN2qVE3/yQSytZlf9uxcm",
                Role = UserRole.Admin
            },
            new User
            {
                Id = 5,
                Name = "sergio",
                LastName = "cortes",
                Email = "sercor@gmail.com",
                UserName = "Chery",
                PasswordHash = "$2a$11$/Qd2pSc0XiHMSbE/m6jA4.RB.7vrB9wL6ZJ5QsWm1ldcA84R15nLW",
                Role = UserRole.User
            },
            new User
            {
                Id = 6,
                Name = "pepe",
                LastName = "pepon",
                Email = "pepito@gmail.com",
                UserName = "pepito",
                PasswordHash = "$2a$11$lv8iWXIAH8hSMUCGLHxM3.oH0pE/pSqkv36yf1dClh/d8fvNbyIgu",
                Role = UserRole.User
            },
            new User
            {
                Id = 7,
                Name = "Andres",
                LastName = "David",
                Email = "andres.david@riwi.io",
                UserName = "Adres-C#",
                PasswordHash = "$2a$11$GAHaCr7hyBp1hiRrnNuXkefvfXS8rCtxjXYUQy3aefol7aFseUde2",
                Role = UserRole.User
            },
            new User
            {
                Id = 8,
                Name = "string3",
                LastName = "string3",
                Email = "string3",
                UserName = "string3",
                PasswordHash = "$2a$11$dsedpWv5bZYXu9XlvbZILOw.2S1bLADQw1HSVtqUILKN9r4pokZIe",
                Role = UserRole.User
            }
        );

    }
}