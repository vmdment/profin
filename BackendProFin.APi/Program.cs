using Microsoft.EntityFrameworkCore; // Necesario para .UseSqlServer()
using BackendProFinAPi.Config;       // Necesario para ApplicationDbContext
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// 1. CONFIGURACIÓN DE SERVICIOS (Dependency Injection)
// -----------------------------------------------------------------------------

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


// -----------------------------------------------------------------------------
// 2. CONFIGURACIÓN DEL PIPELINE HTTP (Middleware)
// -----------------------------------------------------------------------------

var app = builder.Build();

// Configuración para entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirección HTTPS automática
app.UseHttpsRedirection();

// Middleware de Autorización
app.UseAuthorization();

// Mapeo de los controladores (rutas de la API)
app.MapControllers();

// Iniciar la aplicación
app.Run();