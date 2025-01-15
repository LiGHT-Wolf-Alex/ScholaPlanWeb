using Microsoft.Extensions.Logging;
using ScholaPlan.Application.Interfaces;
using ScholaPlan.Domain.Entities;
using ScholaPlan.Domain.Enums;


namespace ScholaPlan.Application.Services
{
    /// <summary>
    /// Генератор расписания на основе генетических алгоритмов.
    /// </summary>
    public class GeneticScheduleGenerator : IScheduleGenerator
    {
        private readonly ILogger<GeneticScheduleGenerator> _logger;

        public GeneticScheduleGenerator(ILogger<GeneticScheduleGenerator> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<LessonSchedule>> GenerateScheduleAsync(School school,
            Dictionary<int, TeacherPreferences> teacherPreferences)
        {
            return await Task.Run(() => GenerateSchedule(school, teacherPreferences));
        }

        private IEnumerable<LessonSchedule> GenerateSchedule(School school,
            Dictionary<int, TeacherPreferences> teacherPreferences)
        {
            // Параметры генетического алгоритма
            int populationSize = 50;
            int generations = 1000;
            double mutationRate = 0.01;

            // Инициализация популяции
            var population = InitializePopulation(school, teacherPreferences, populationSize);

            for (int generation = 0; generation < generations; generation++)
            {
                // Оценка приспособленности
                var fitnessScores = population
                    .Select(individual => CalculateFitness(individual, school, teacherPreferences)).ToList();

                // Проверка условия остановки (например, достижение оптимальной приспособленности)
                if (fitnessScores.Max() >= 1.0)
                {
                    _logger.LogInformation($"Достигнута оптимальная приспособленность на поколении {generation}.");
                    break;
                }

                // Создание новой популяции
                var newPopulation = new List<List<LessonSchedule>>();

                while (newPopulation.Count < populationSize)
                {
                    // Отбор родителей (турнирный отбор)
                    var (parent1, parent2) = TournamentSelection(population, fitnessScores);

                    // Скрещивание (одноточечный кроссовер)
                    var offspring = Crossover(parent1, parent2, school, teacherPreferences);

                    // Мутация
                    Mutate(offspring, mutationRate, school, teacherPreferences);

                    // Добавление потомка в новую популяцию
                    newPopulation.Add(offspring);
                }

                population = newPopulation;
            }

            // Выбор лучшего решения
            var bestIndividual = population
                .Select(individual => new
                    { Individual = individual, Fitness = CalculateFitness(individual, school, teacherPreferences) })
                .OrderByDescending(x => x.Fitness)
                .First().Individual;

            _logger.LogInformation(
                $"Генерация расписания завершена. Лучшее расписание имеет приспособленность {CalculateFitness(bestIndividual, school, teacherPreferences)}.");
            return bestIndividual;
        }

        private List<List<LessonSchedule>> InitializePopulation(School school,
            Dictionary<int, TeacherPreferences> teacherPreferences, int populationSize)
        {
            var population = new List<List<LessonSchedule>>();
            var rnd = new Random();

            for (int i = 0; i < populationSize; i++)
            {
                var individual = new List<LessonSchedule>();

                foreach (var classConfig in school.MaxLessonsPerDayConfigs)
                {
                    int classGrade = classConfig.ClassGrade;
                    int maxLessonsPerDay = classConfig.MaxLessonsPerDay;

                    var subjects = school.Subjects.OrderByDescending(s => s.WeeklyHours).ToList();

                    foreach (var subject in subjects)
                    {
                        for (int j = 0; j < subject.WeeklyHours; j++)
                        {
                            // Случайный выбор учителя
                            var availableTeachers = school.Teachers
                                .Where(t => t.Specializations.Contains(subject.Specialization))
                                .ToList();

                            if (!availableTeachers.Any())
                                continue;

                            var teacher = availableTeachers[rnd.Next(availableTeachers.Count)];

                            // Случайный выбор кабинета
                            var availableRooms = school.Rooms
                                .Where(r => r.Type == RoomType.Standard)
                                .ToList();

                            if (!availableRooms.Any())
                                continue;

                            var room = availableRooms[rnd.Next(availableRooms.Count)];

                            // Случайный выбор дня и номера урока
                            var day = _daysOfWeek[rnd.Next(_daysOfWeek.Count)];
                            var lessonNumber = rnd.Next(1, 9); // 1-8

                            var schedule = new LessonSchedule
                            {
                                SchoolId = school.Id,
                                TeacherId = teacher.Id,
                                SubjectId = subject.Id,
                                RoomId = room.Id,
                                ClassGrade = classGrade,
                                DayOfWeek = day,
                                LessonNumber = lessonNumber
                            };

                            individual.Add(schedule);
                        }
                    }
                }

                population.Add(individual);
            }

            return population;
        }

        private double CalculateFitness(List<LessonSchedule> individual, School school,
            Dictionary<int, TeacherPreferences> teacherPreferences)
        {
            // Рассчитываем фитнес на основе:
            // - Отсутствие конфликтов (один учитель или кабинет в одно и то же время)
            // - Учет предпочтений учителей
            // - Минимизация перемещений

            double fitness = 0.0;

            var teacherSchedule = new Dictionary<int, HashSet<(DayOfWeek, int)>>();
            var roomSchedule = new Dictionary<int, HashSet<(DayOfWeek, int)>>();
            var teacherRooms = new Dictionary<int, HashSet<int>>(); // Для минимизации перемещений

            foreach (var lesson in individual)
            {
                // Проверка учителя
                if (!teacherSchedule.ContainsKey(lesson.TeacherId))
                    teacherSchedule[lesson.TeacherId] = new HashSet<(DayOfWeek, int)>();

                if (teacherSchedule[lesson.TeacherId].Contains((lesson.DayOfWeek, lesson.LessonNumber)))
                {
                    // Конфликт учителя
                    continue;
                }

                // Проверка кабинета
                if (!roomSchedule.ContainsKey(lesson.RoomId))
                    roomSchedule[lesson.RoomId] = new HashSet<(DayOfWeek, int)>();

                if (roomSchedule[lesson.RoomId].Contains((lesson.DayOfWeek, lesson.LessonNumber)))
                {
                    // Конфликт кабинета
                    continue;
                }

                // Учет предпочтений учителя
                if (teacherPreferences.ContainsKey(lesson.TeacherId))
                {
                    var prefs = teacherPreferences[lesson.TeacherId];
                    if (prefs.AvailableDays.Contains(lesson.DayOfWeek) &&
                        prefs.AvailableLessonNumbers.Contains(lesson.LessonNumber))
                        fitness += 1.0;
                    else
                        fitness += 0.5;
                }
                else
                {
                    fitness += 0.5;
                }

                // Минимизация перемещений между кабинетами
                if (!teacherRooms.ContainsKey(lesson.TeacherId))
                    teacherRooms[lesson.TeacherId] = new HashSet<int>();

                if (!teacherRooms[lesson.TeacherId].Contains(lesson.RoomId))
                {
                    fitness += 0.1; // Небольшое снижение фитнеса за перемещение
                }
                else
                {
                    fitness += 0.2; // Дополнительный фитнес за отсутствие перемещения
                }

                // Добавление в расписание
                teacherSchedule[lesson.TeacherId].Add((lesson.DayOfWeek, lesson.LessonNumber));
                roomSchedule[lesson.RoomId].Add((lesson.DayOfWeek, lesson.LessonNumber));
                teacherRooms[lesson.TeacherId].Add(lesson.RoomId);
            }

            return fitness;
        }

        private (List<LessonSchedule>, List<LessonSchedule>) TournamentSelection(List<List<LessonSchedule>> population,
            List<double> fitnessScores, int tournamentSize = 5)
        {
            var rnd = new Random();
            var selectedIndices = new List<int>();

            // Выбор индексов для турнира
            for (int i = 0; i < tournamentSize; i++)
            {
                int idx = rnd.Next(population.Count);
                selectedIndices.Add(idx);
            }

            // Сортировка выбранных индексов по фитнес-оценке
            selectedIndices = selectedIndices.OrderByDescending(idx => fitnessScores[idx]).ToList();

            // Выбор двух лучших как родителей
            var parent1 = population[selectedIndices[0]];
            var parent2 = population[selectedIndices[1]];

            return (parent1, parent2);
        }

        private List<LessonSchedule> Crossover(List<LessonSchedule> parent1, List<LessonSchedule> parent2,
            School school, Dictionary<int, TeacherPreferences> teacherPreferences)
        {
            var rnd = new Random();
            int crossoverPoint = rnd.Next(1, parent1.Count - 1);

            var offspring = new List<LessonSchedule>();

            offspring.AddRange(parent1.Take(crossoverPoint));
            offspring.AddRange(parent2.Skip(crossoverPoint));

            return offspring;
        }

        private void Mutate(List<LessonSchedule> individual, double mutationRate, School school,
            Dictionary<int, TeacherPreferences> teacherPreferences)
        {
            var rnd = new Random();

            foreach (var lesson in individual)
            {
                if (rnd.NextDouble() < mutationRate)
                {
                    // Случайный выбор нового учителя
                    var availableTeachers = school.Teachers
                        .Where(t => t.Specializations.Contains(lesson.Subject.Specialization))
                        .ToList();

                    if (availableTeachers.Any())
                    {
                        var newTeacher = availableTeachers[rnd.Next(availableTeachers.Count)];
                        lesson.TeacherId = newTeacher.Id;
                    }

                    // Случайный выбор нового кабинета
                    var availableRooms = school.Rooms
                        .Where(r => r.Type == RoomType.Standard)
                        .ToList();

                    if (availableRooms.Any())
                    {
                        var newRoom = availableRooms[rnd.Next(availableRooms.Count)];
                        lesson.RoomId = newRoom.Id;
                    }

                    // Случайный выбор дня и номера урока
                    var days = _daysOfWeek.ToList();
                    lesson.DayOfWeek = days[rnd.Next(days.Count)];
                    lesson.LessonNumber = rnd.Next(1, 9);
                }
            }
        }

        private readonly List<DayOfWeek> _daysOfWeek = new List<DayOfWeek>
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday
        };
    }
}