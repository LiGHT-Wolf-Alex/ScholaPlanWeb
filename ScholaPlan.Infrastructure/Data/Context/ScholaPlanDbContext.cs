using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.Infrastructure.Data.Context;

/// <summary>
/// Основной DbContext для приложения ScholaPlan.
/// </summary>
public class ScholaPlanDbContext : DbContext
{
    public ScholaPlanDbContext(DbContextOptions<ScholaPlanDbContext> options)
        : base(options)
    {
    }

    public DbSet<School> Schools { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<LessonSchedule> LessonSchedules { get; set; }
    public DbSet<MaxLessonsPerDayConfig> MaxLessonsPerDayConfigs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Отключаем каскадное удаление для связи LessonSchedule -> School,
        // чтобы избежать multiple cascade paths.
        modelBuilder.Entity<LessonSchedule>()
            .HasOne(ls => ls.School)
            .WithMany(s => s.LessonSchedules)
            .HasForeignKey(ls => ls.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);

        // По желанию можно отключить каскад и для остальных внешних ключей,
        // если хотите управлять удалениями вручную:
        //
        modelBuilder.Entity<LessonSchedule>()
            .HasOne(ls => ls.Room)
            .WithMany(r => r.Lessons)
            .HasForeignKey(ls => ls.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LessonSchedule>()
            .HasOne(ls => ls.Teacher)
            .WithMany(t => t.Lessons)
            .HasForeignKey(ls => ls.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LessonSchedule>()
            .HasOne(ls => ls.Subject)
            .WithMany()
            .HasForeignKey(ls => ls.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}