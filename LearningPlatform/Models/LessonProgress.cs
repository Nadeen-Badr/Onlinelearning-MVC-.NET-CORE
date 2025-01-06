public class LessonProgress
{
    public int LessonProgressId { get; set; }
    public string? UserId { get; set; } // FK to ApplicationUser
    public int? LessonId { get; set; } // FK to Lesson
    public bool IsRead { get; set; } = false;
    public DateTime DateRead { get; set; }

    public ApplicationUser? User { get; set; }
    public Lesson?Lesson { get; set; }
}
