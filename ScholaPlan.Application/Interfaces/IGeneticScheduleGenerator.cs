using ScholaPlan.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScholaPlan.Application.Interfaces
{
    /// <summary>
    /// Интерфейс генератора расписания на основе генетических алгоритмов.
    /// </summary>
    public interface IGeneticScheduleGenerator
    {
        /// <summary>
        /// Генерирует расписание для указанной школы с учетом предпочтений учителей.
        /// </summary>
        /// <param name="school">Школа для генерации расписания.</param>
        /// <param name="teacherPreferences">Предпочтения учителей.</param>
        /// <returns>Список расписаний занятий.</returns>
        Task<IEnumerable<LessonSchedule>> GenerateScheduleAsync(School school,
            Dictionary<int, TeacherPreferences> teacherPreferences);
    }
}