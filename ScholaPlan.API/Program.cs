using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Infrastructure.Data;
using ScholaPlan.Infrastructure.Data.Context;
using ScholaPlan.Infrastructure.Data.Repositories;
using ScholaPlan.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Подключаем DbContext 
builder.Services.AddDbContext<ScholaPlanDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Автоматический скан репозиториев (SchoolRepository, LessonScheduleRepository и т.д.)
builder.Services.Scan(scan => scan
    .FromAssemblies(typeof(SchoolRepository).Assembly) // или укажите конкретную сборку
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

// Пример ручной регистрации (на случай, если нужен мануальный вариант, можно раскомментировать)
// builder.Services.AddScoped<ISchoolRepository, SchoolRepository>();
// builder.Services.AddScoped<ILessonScheduleRepository, LessonScheduleRepository>();

// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Подключаем контроллеры
builder.Services.AddControllers();

// Swagger
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

// Инициализируем БД тестовыми данными (DbInitializer)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ScholaPlanDbContext>();
    DbInitializer.Initialize(context);
}

// Запускаем Swagger (для dev/prod сред)
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

app.MapControllers();

app.Run();