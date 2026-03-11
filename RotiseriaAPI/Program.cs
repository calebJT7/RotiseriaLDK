using Microsoft.EntityFrameworkCore;
using RotiseriaAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de la Base de Datos con puerto 5433
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5433;Database=rotiseria_pos;Username=admin;Password=password123"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. Configuración del pipeline (Swagger y Rutas)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();