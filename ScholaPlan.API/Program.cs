using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ScholaPlan.Infrastructure.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// Добавление контекста базы данных
builder.Services.AddDbContext<ScholaPlanDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ScholaPlan API",
        Version = "v1",
        Description = "API для управления школьным расписанием в ScholaPlan"
    });
});

var app = builder.Build();
app.MapControllers();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ScholaPlan API v1");
        c.RoutePrefix = string.Empty; // Главная страница - Swagger
    });
}

app.UseHttpsRedirection();

app.Run();