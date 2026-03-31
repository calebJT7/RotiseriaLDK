using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text; // Para CodePagesEncodingProvider
using Microsoft.EntityFrameworkCore;
using RotiseriaAPI.Data; // Asegurate de que este sea el namespace donde está tu AppDbContext

var builder = WebApplication.CreateBuilder(args);

// Soporte para páginas de código de impresora (IBM850 y similares)
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// ==========================================
// 1. CONFIGURACIÓN DE SERVICIOS
// ==========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS: Permite que el Front-End (Blazor) se conecte a la API sin ser bloqueado
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Base de Datos: Conexión a SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT (Día 12): Configuración de los tokens de seguridad
var jwtKey = builder.Configuration["Jwt:Key"] ?? "EstaEsUnaClaveSecretaDeRespaldoMuyLarga12345!"; // Usa la de appsettings.json o esta de respaldo
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<RotiseriaAPI.Services.PrintService>();
// ==========================================
// --- CONSTRUCCIÓN DE LA APP ---
// ==========================================
var app = builder.Build();

// ==========================================
// 2. MIDDLEWARE (El orden acá es vital)
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// IMPORTANTE: Cors SIEMPRE va antes de Auth
app.UseCors("AllowAll");

app.UseAuthentication(); // Primero identifica quién es el usuario
app.UseAuthorization();  // Después verifica si tiene permiso

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    if (!context.Customers.Any())
    {
        context.Customers.AddRange(
            new RotiseriaAPI.Models.Customer { Name = "Juan Perez", Phone = "351123456", Balance = -5000, Notes = "Debe desde enero" },
            new RotiseriaAPI.Models.Customer { Name = "Ana Garcia", Phone = "351987654", Balance = 0 },
            new RotiseriaAPI.Models.Customer { Name = "Carlos Toledo", Phone = "351000111", Balance = -1200, Notes = "Amigo de la casa" }
        );
        context.SaveChanges();
    }

    if (!context.Products.Any())
    {
        context.Products.AddRange(
            new RotiseriaAPI.Models.Product { Name = "MILANESA CON FRITAS", Price = 4500, Stock = 10, IsActive = true },
            new RotiseriaAPI.Models.Product { Name = "EMPANADA CARNE", Price = 800, Stock = 50, IsActive = true },
            new RotiseriaAPI.Models.Product { Name = "COCA COLA 1.5L", Price = 2500, Stock = 20, IsActive = true }
        );
        context.SaveChanges();
    }
}

app.Run();