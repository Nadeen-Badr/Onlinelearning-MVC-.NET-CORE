public class QuizRepository : IQuizRepository
{
    private readonly ApplicationDbContext _context;

    public QuizRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Quiz> GetAllQuizzes()
    {
        return _context.Quizzes.ToList();
    }

    public Quiz GetQuizById(int id)
    {
        return _context.Quizzes.Find(id);
    }

    public void AddQuiz(Quiz quiz)
    {
        _context.Quizzes.Add(quiz);
        _context.SaveChanges();
    }

    public void UpdateQuiz(Quiz quiz)
    {
        _context.Quizzes.Update(quiz);
        _context.SaveChanges();
    }

    public void DeleteQuiz(int id)
    {
        var quiz = _context.Quizzes.Find(id);
        if (quiz != null)
        {
            _context.Quizzes.Remove(quiz);
            _context.SaveChanges();
        }
    }
}
