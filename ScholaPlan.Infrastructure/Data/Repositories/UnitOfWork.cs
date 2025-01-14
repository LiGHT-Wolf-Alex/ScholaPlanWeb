using ScholaPlan.Application.Interfaces.IRepositories;
using ScholaPlan.Infrastructure.Data.Context;
using System.Threading.Tasks;

namespace ScholaPlan.Infrastructure.Repositories
{
    /// <summary>
    /// Реализация паттерна Unit of Work для управления транзакциями.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ScholaPlanDbContext _context;

        public ISchoolRepository Schools { get; }
        public IRoomRepository Rooms { get; }
        public ISubjectRepository Subjects { get; }
        public ITeacherRepository Teachers { get; }
        public ILessonScheduleRepository LessonSchedules { get; }

        public UnitOfWork(
            ScholaPlanDbContext context,
            ISchoolRepository schoolRepository,
            IRoomRepository roomRepository,
            ISubjectRepository subjectRepository,
            ITeacherRepository teacherRepository,
            ILessonScheduleRepository lessonScheduleRepository)
        {
            _context = context;
            Schools = schoolRepository;
            Rooms = roomRepository;
            Subjects = subjectRepository;
            Teachers = teacherRepository;
            LessonSchedules = lessonScheduleRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}