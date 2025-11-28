using BackendProFinAPi.Config;
using BackendProFinAPi.Models; // NECESARIO para UserModel
using BackendProFinAPi.Repositories;
using BackendProFinAPi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

// PATRON CREACIONAL
var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// 1. CONFIGURACIÓN DE SERVICIOS (Dependency Injection)
// =========================================================================

// Agregar servicios para controladores (API Endpoints)
builder.Services.AddControllers();

// Agregar servicios de Swagger/OpenAPI (Documentación y pruebas)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CONFIGURACIÓN DE LA BASE DE DATOS (Entity Framework Core) ---

// 1. Configuración de Entity Framework Core con SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- CONFIGURACIÓN DE IDENTIDAD Y SEGURIDAD ---



// CRÍTICO: 3. Configuración de JWT Authentication (Bearer Token)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        // Obtiene valores de appsettings.json
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        // Obtiene la clave secreta de appsettings.json
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// --- REGISTRO DE CAPAS DE SEGURIDAD Y DATOS ---

// 4. Registro del Repositorio (Patrón Repositorio)
builder.Services.AddScoped<IUserRepository, UserRepository>();

// 5. Registro del Servicio de Autenticación 
builder.Services.AddScoped<IAuthService, AuthService>();


// =========================================================================
// 2. CONFIGURACIÓN DEL PIPELINE HTTP (Middleware)
// =========================================================================

var app = builder.Build();

// Configuración para entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirección HTTPS automática
app.UseHttpsRedirection();

// Middleware de Autenticación: Debe ir antes de UseAuthorization
app.UseAuthentication();

// Middleware de Autorización
app.UseAuthorization();

// Mapeo de los controladores (rutas de la API)
app.MapControllers();

// CRÍTICO: 6. INVOCACIÓN DEL SEEDER (Inicialización de DB con Roles y Usuarios)
// 
// 🚨 LA LLAMADA AL SEEDER HA SIDO ELIMINADA. 
// La base de datos y los datos iniciales deben ser manejados manualmente.
// 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Se asegura que la base de datos se migre a su estado más reciente
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Si hay un fallo en la inicialización, lo registramos.
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al intentar aplicar las migraciones.");
    }
}
// =========================================================================

// Iniciar la aplicación
app.Run();