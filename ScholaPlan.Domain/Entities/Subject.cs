namespace ScholaPlan.Domain.Entities;

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int DifficultyLevel { get; set; }
    public int WeeklyHours { get; set; }
}