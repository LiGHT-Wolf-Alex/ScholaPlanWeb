using Microsoft.EntityFrameworkCore;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.Infrastructure.Data.Context;

/// <summary>
/// Контекст базы данных для проекта ScholaPlan.
/// </summary>
public class ScholaPlanDbContext(DbContextOptions<ScholaPlanDbContext> options) : DbContext(options)
{
    /// <summary>
    /// DbSet для таблицы школ.
    /// </summary>
    public DbSet<School> Schools { get; set; }

    /// <summary>
    /// DbSet для таблицы кабинетов.
    /// </summary>
    public DbSet<Room> Rooms { get; set; }

    /// <summary>
    /// DbSet для таблицы занятий.
    /// </summary>
    public DbSet<LessonSchedule> LessonSchedules { get; set; }

    /// <summary>
    /// DbSet для таблицы предметов.
    /// </summary>
    public DbSet<Subject> Subjects { get; set; }

    /// <summary>
    /// DbSet для таблицы учителей.
    /// </summary>
    public DbSet<Teacher> Teachers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<School>()
            .HasMany(s => s.Teachers)
            .WithOne(t => t.School)
            .HasForeignKey(t => t.SchoolId);

        modelBuilder.Entity<School>()
            .HasMany(s => s.Rooms)
            .WithOne(r => r.School)
            .HasForeignKey(r => r.SchoolId);

        modelBuilder.Entity<School>()
            .HasMany(s => s.LessonSchedules)
            .WithOne(l => l.School)
            .HasForeignKey(l => l.SchoolId);
    }
}