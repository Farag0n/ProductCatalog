using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infraestructure.Data.Configurations;

namespace ProductCatalog.Infraestructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
    
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    
    //Usar las configuraciones de la api para la conexion
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Aplica las configuraciones Fluent API desde la carpeta Configurations
        modelBuilder.ApplyConfiguration(new ProductConfigurations());
        modelBuilder.ApplyConfiguration(new UserConfigurations());
    }
}