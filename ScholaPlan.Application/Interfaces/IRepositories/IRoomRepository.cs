using ScholaPlan.Domain.Entities;
using System.Threading.Tasks;

namespace ScholaPlan.Application.Interfaces.IRepositories
{
    /// <summary>
    /// Репозиторий для работы с кабинетами.
    /// </summary>
    public interface IRoomRepository
    {
        Task<Room?> GetByIdAsync(int roomId);
        Task AddAsync(Room room);

        void Remove(Room room); // Добавлено
        // Дополнительные методы при необходимости
    }
}