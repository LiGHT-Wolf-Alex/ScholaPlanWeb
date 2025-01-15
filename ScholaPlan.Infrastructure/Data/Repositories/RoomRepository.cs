using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Infrastructure.Data.Context;

namespace ScholaPlan.Infrastructure.Data.Repositories;

/// <summary>
/// Реализация репозитория для работы с кабинетами.
/// </summary>
public class RoomRepository(ScholaPlanDbContext context, ILogger<RoomRepository> logger)
    : IRoomRepository
{
    public async Task<Room?> GetByIdAsync(int roomId)
    {
        try
        {
            logger.LogInformation($"Получение кабинета с ID {roomId}.");
            return await context.Rooms
                .Include(r => r.School)
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Ошибка при получении кабинета с ID {roomId}.");
            throw;
        }
    }

    public async Task AddAsync(Room room)
    {
        try
        {
            logger.LogInformation($"Добавление нового кабинета: {room.Number}.");
            await context.Rooms.AddAsync(room);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при добавлении кабинета.");
            throw;
        }
    }

    public void Remove(Room room)
    {
        try
        {
            logger.LogInformation($"Удаление кабинета с ID {room.Id}.");
            context.Rooms.Remove(room);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Ошибка при удалении кабинета с ID {room.Id}.");
            throw;
        }
    }
}