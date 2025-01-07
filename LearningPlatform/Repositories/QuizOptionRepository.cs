public class QuizOptionRepository : IQuizOptionRepository
{
    private readonly ApplicationDbContext _context;

    public QuizOptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddOptionAsync(QuizOption option)
    {
        await _context.QuizOption.AddAsync(option);
        await _context.SaveChangesAsync();
    }
}