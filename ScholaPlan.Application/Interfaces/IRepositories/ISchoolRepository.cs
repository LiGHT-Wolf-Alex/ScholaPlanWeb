using ScholaPlan.Domain.Entities;

namespace ScholaPlan.Application.Interfaces.IRepositories;

/// <summary>
/// Репозиторий для работы со школами.
/// </summary>
public interface ISchoolRepository
{
    Task<School?> GetByIdAsync(int schoolId);
    Task<List<School>> GetAllAsync();
    Task AddAsync(School school);
    Task SaveChangesAsync();
}