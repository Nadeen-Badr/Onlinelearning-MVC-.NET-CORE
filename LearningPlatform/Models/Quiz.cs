public class Quiz
{
    public int QuizId { get; set; }
    public string? Question { get; set; }
    public string? Answer { get; set; } // Correct answer
    public ICollection<QuizOption>? Options { get; set; } // Multiple options

    public ICollection<QuizAttempt>? QuizAttempts { get; set; }

    public int? LessonId { get; set; } // FK to Lesson
    public Lesson? Lesson { get; set; } // Navigation property to Lesson
}

