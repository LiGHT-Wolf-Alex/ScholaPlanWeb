using ScholaPlan.Domain.Entities;

namespace ScholaPlan.Application.Interfaces.IRepositories;

/// <summary>
/// Репозиторий для работы с учителями.
/// </summary>
public interface ITeacherRepository
{
    Task<Teacher?> GetByIdAsync(int teacherId);
    Task AddAsync(Teacher teacher);
    void Remove(Teacher teacher); 
}