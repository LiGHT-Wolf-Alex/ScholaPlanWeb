namespace ScholaPlan.Application.Interfaces.IRepositories;

/// <summary>
/// Интерфейс Unit of Work для управления транзакциями.
/// </summary>
public interface IUnitOfWork
{
    ISchoolRepository Schools { get; }
    ILessonScheduleRepository LessonSchedules { get; }
    Task<int> SaveChangesAsync();
}