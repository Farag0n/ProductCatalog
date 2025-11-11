using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Services;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Infraestructure.Data;
using ProductCatalog.Infraestructure.Extensions;
using ProductCatalog.Infraestructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// =======================================================
// 1. Configuración de la cadena de conexión a MySQL
// =======================================================

// Obtiene la cadena desde appsettings.json

// Configura EF Core con autodetección de versión MySQL

builder.Services.AddInfrastructure(builder.Configuration);
// =======================================================
// 2. Inyección de dependencias
// =======================================================

// Repositorios
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


//Servicios de application
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();

//Configura el sistema de autenticación para validar tokens JWT en las solicitudes HTTP.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();


// =======================================================
// 3. Controladores y Swagger
// =======================================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "User,Product Management API",
        Version = "v1",
        Description = "API para la gestión de usuarios y productos"
    });
});

// ======================
//  CONFIGURACIÓN DE CORS
// ======================
var corsPolicyName = "AllowSpecificOrigins";

builder.Services.AddCors( options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod(); // si el front envía cookies o auth headers
        });
});

// =======================================================
// 4. Construcción y pipeline
// =======================================================

var app = builder.Build();

// Test the connection to the database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.OpenConnection();
        Console.WriteLine("Connection to database successful.");
        db.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error of connection: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "School Management API v1");
        c.RoutePrefix = string.Empty;
    });
}

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Catalog API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();