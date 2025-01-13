using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Infrastructure.Data.Context;
using ScholaPlan.Infrastructure.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Добавление контекста базы данных
builder.Services.AddDbContext<ScholaPlanDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Автоматическая регистрация репозиториев с явным указанием сборки
builder.Services.Scan(scan => scan
    .FromAssemblies(Assembly.Load("ScholaPlan.Infrastructure"))
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.Run();