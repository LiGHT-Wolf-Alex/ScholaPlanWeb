using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ScholaPlan.Application.Interfaces;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Application.Services;
using ScholaPlan.Infrastructure.Data.Context;
using ScholaPlan.Infrastructure.Repositories;
using ScholaPlan.Domain.Entities;
using System.Text;
using AutoMapper;
using ScholaPlan.API.MappingProfiles;
using AspNetCoreRateLimit;
using Serilog;
using Prometheus;
using ScholaPlan.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Подключаем DbContext 
builder.Services.AddDbContext<ScholaPlanDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ScholaPlanDbContext>()
    .AddDefaultTokenProviders();

// Настройка JWT аутентификации
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// Регистрация репозиториев
builder.Services.AddScoped<ISchoolRepository, SchoolRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ILessonScheduleRepository, LessonScheduleRepository>();

// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Регистрация сервисов
builder.Services.AddScoped<IScheduleGenerator, GeneticScheduleGenerator>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();

// Регистрация AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

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

    // Добавляем поддержку JWT в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                      "Введите 'Bearer' [пробел] и ваш токен в поле ниже.\r\n\r\n" +
                      "Пример: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("https://your-frontend-domain.com") // Замените на URL вашего фронтенда
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Настройка ограничения запросов (Rate Limiting)
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Добавление Prometheus
builder.Services.AddSingleton<ILoggerFactory, LoggerFactory>();

var app = builder.Build();

// Применение миграций и инициализация базы данных
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ScholaPlanDbContext>();
        context.Database.Migrate(); // Применение миграций
        DbInitializer.Initialize(context, services); // Инициализация данных
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or initializing the database.");
        throw;
    }
}

// Настройка Serilog Request Logging
app.UseSerilogRequestLogging();

// Использование Prometheus
app.UseMetricServer(); // Метрики доступны по /metrics
app.UseHttpMetrics(); // Сбор метрик HTTP запросов

// Использование ограничения запросов
app.UseIpRateLimiting();

// Глобальная обработка исключений
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ScholaPlan API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

// Предупреждение о неустановленном HTTPS порте можно игнорировать в локальной разработке или установить явно
// Добавьте строку в appsettings.json или настройте явно
// Например, добавьте в launchSettings.json:
// "applicationUrl": "https://localhost:5001;http://localhost:5000"

app.UseCors("AllowSpecificOrigins");

// Включаем аутентификацию и авторизацию
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();