using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.Infrastructure.Data.Context;

/// <summary>
/// Контекст базы данных для проекта ScholaPlan.
/// </summary>
public class ScholaPlanDbContext : DbContext
{
    public ScholaPlanDbContext(DbContextOptions<ScholaPlanDbContext> options)
        : base(options)
    {
    }

    public DbSet<MaxLessonsPerDayConfig> MaxLessonsPerDayConfigs { get; set; }
    public DbSet<School> Schools { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<LessonSchedule> LessonSchedules { get; set; }
    public DbSet<Subject> Subjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<School>().HasKey(s => s.Id);
        modelBuilder.Entity<Teacher>().HasKey(t => t.Id);
        modelBuilder.Entity<Room>().HasKey(r => r.Id);
        modelBuilder.Entity<LessonSchedule>().HasKey(ls => ls.Id);
        modelBuilder.Entity<Subject>().HasKey(sub => sub.Id);
        modelBuilder.Entity<Teacher>().OwnsOne(t => t.Name);
    }
}