public class QuizOption
{
    public int QuizOptionId { get; set; }
    public string? OptionText { get; set; }
    public bool IsCorrect { get; set; } // Marks whether it's the correct answer

    public int? QuizId { get; set; } // FK to Quiz
    public Quiz? Quiz { get; set; }
}
