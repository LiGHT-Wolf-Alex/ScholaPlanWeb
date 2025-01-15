using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Infrastructure.Data.Context;

namespace ScholaPlan.Infrastructure.Data.Repositories;

/// <summary>
/// Реализация паттерна Unit of Work для управления транзакциями.
/// </summary>
public class UnitOfWork(
    ScholaPlanDbContext context,
    ISchoolRepository schoolRepository,
    IRoomRepository roomRepository,
    ISubjectRepository subjectRepository,
    ITeacherRepository teacherRepository,
    ILessonScheduleRepository lessonScheduleRepository)
    : IUnitOfWork
{
    public ISchoolRepository Schools { get; } = schoolRepository;
    public IRoomRepository Rooms { get; } = roomRepository;
    public ISubjectRepository Subjects { get; } = subjectRepository;
    public ITeacherRepository Teachers { get; } = teacherRepository;
    public ILessonScheduleRepository LessonSchedules { get; } = lessonScheduleRepository;

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}