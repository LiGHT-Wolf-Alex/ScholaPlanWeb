using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ScholaPlan.Domain.Entities;

namespace ScholaPlan.Infrastructure.Data.Context;

/// <summary>
/// Основной DbContext для приложения ScholaPlan.
/// Включает поддержку ASP.NET Core Identity.
/// </summary>
public class ScholaPlanDbContext(DbContextOptions<ScholaPlanDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    public DbSet<School> Schools { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<LessonSchedule> LessonSchedules { get; set; }
    public DbSet<SchoolGradeConfig> MaxLessonsPerDayConfigs { get; set; }
    public DbSet<TeacherPreferences> TeacherPreferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Конфигурация TeacherPreferences
        modelBuilder.Entity<TeacherPreferences>()
            .HasKey(tp => tp.TeacherId);

        modelBuilder.Entity<TeacherPreferences>()
            .HasOne(tp => tp.Teacher)
            .WithOne(t => t.Preferences)
            .HasForeignKey<TeacherPreferences>(tp => tp.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        // Отключаем каскадное удаление для связи LessonSchedule -> School,
        // чтобы избежать multiple cascade paths.
        modelBuilder.Entity<LessonSchedule>()
            .HasOne(ls => ls.School)
            .WithMany(s => s.LessonSchedules)
            .HasForeignKey(ls => ls.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);

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
    }
}