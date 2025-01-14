using ScholaPlan.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScholaPlan.Application.Interfaces
{
    /// <summary>
    /// Интерфейс генератора расписания.
    /// </summary>
    public interface IScheduleGenerator
    {
        Task<IEnumerable<LessonSchedule>> GenerateScheduleAsync(School school,
            Dictionary<int, TeacherPreferences> teacherPreferences);
    }
}