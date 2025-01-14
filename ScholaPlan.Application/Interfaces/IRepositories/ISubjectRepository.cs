using ScholaPlan.Domain.Entities;

namespace ScholaPlan.Application.Interfaces.IRepositories
{
    /// <summary>
    /// Репозиторий для работы с предметами.
    /// </summary>
    public interface ISubjectRepository
    {
        Task<Subject?> GetByIdAsync(int subjectId);

        Task AddAsync(Subject subject);
    }
}