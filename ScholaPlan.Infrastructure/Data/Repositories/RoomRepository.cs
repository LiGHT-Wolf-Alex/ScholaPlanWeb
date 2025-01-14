using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ScholaPlan.Infrastructure.Repositories
{
    /// <summary>
    /// Реализация репозитория для работы с кабинетами.
    /// </summary>
    public class RoomRepository : IRoomRepository
    {
        private readonly ScholaPlanDbContext _context;
        private readonly ILogger<RoomRepository> _logger;

        public RoomRepository(ScholaPlanDbContext context, ILogger<RoomRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Room?> GetByIdAsync(int roomId)
        {
            try
            {
                _logger.LogInformation($"Получение кабинета с ID {roomId}.");
                return await _context.Rooms
                    .Include(r => r.School)
                    .FirstOrDefaultAsync(r => r.Id == roomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении кабинета с ID {roomId}.");
                throw;
            }
        }

        public async Task AddAsync(Room room)
        {
            try
            {
                _logger.LogInformation($"Добавление нового кабинета: {room.Number}.");
                await _context.Rooms.AddAsync(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении кабинета.");
                throw;
            }
        }

        // Дополнительные методы при необходимости
    }
}