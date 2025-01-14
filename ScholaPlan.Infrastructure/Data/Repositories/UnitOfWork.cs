using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Infrastructure.Data.Context;

namespace ScholaPlan.Infrastructure.Data.Repositories;

/// <summary>
/// Реализация паттерна Unit of Work для управления транзакциями.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ScholaPlanDbContext _context;

    public ISchoolRepository Schools { get; }
    public ILessonScheduleRepository LessonSchedules { get; }

    public UnitOfWork(ScholaPlanDbContext context, ISchoolRepository schoolRepository,
        ILessonScheduleRepository lessonScheduleRepository)
    {
        _context = context;
        Schools = schoolRepository;
        LessonSchedules = lessonScheduleRepository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}