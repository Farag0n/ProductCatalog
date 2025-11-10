using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Infraestructure.Data;

namespace ProductCatalog.Infraestructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string conn = configuration.GetConnectionString("DefaultConnection");
    
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                conn,
                ServerVersion.AutoDetect(conn)
            )
        );
        return services;
    }
}