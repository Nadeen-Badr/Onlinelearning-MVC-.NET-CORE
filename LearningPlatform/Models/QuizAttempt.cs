public class QuizAttempt
{
    public int QuizAttemptId { get; set; }
    public string? UserId { get; set; } // FK to ApplicationUser
    public ApplicationUser? User { get; set; }
    public int QuizId { get; set; } // FK to Quiz
    public Quiz? Quiz { get; set; }
    public int SelectedOptionId { get; set; } // FK to QuizOption
    public QuizOption? SelectedOption { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime AttemptDate { get; set; }
}
