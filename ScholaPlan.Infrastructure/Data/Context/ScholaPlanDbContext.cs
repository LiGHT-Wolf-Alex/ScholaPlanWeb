using System.Text.Json;
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

        // TeacherName как Value Object
        modelBuilder.Entity<School>()
            .HasMany(s => s.MaxLessonsPerDayConfigs)
            .WithOne(c => c.School)
            .HasForeignKey(c => c.SchoolId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<School>()
            .OwnsOne(t => t.Name);

        modelBuilder.Entity<LessonSchedule>()
            .HasOne(ls => ls.School)
            .WithMany(s => s.LessonSchedules)
            .HasForeignKey(ls => ls.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LessonSchedule>()
            .HasOne(ls => ls.Teacher)
            .WithMany(t => t.Lessons)
            .HasForeignKey(ls => ls.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}