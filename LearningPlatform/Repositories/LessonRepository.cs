using Microsoft.EntityFrameworkCore;

public class LessonRepository : ILessonRepository
{
    private readonly ApplicationDbContext _context;

    public LessonRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Lesson> GetLessonByIdAsync(int lessonId)
    {
        return await _context.Lessons
            .Include(l => l.Course) // If you want to include related data
            .FirstOrDefaultAsync(l => l.LessonId == lessonId);
    }

    public async Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(int courseId)
    {
        return await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .ToListAsync();
    }

    public async Task AddLessonAsync(Lesson lesson)
    {
        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateLessonAsync(Lesson lesson)
    {
        _context.Lessons.Update(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteLessonAsync(int lessonId)
    {
        var lesson = await _context.Lessons.FindAsync(lessonId);
        if (lesson != null)
        {
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }
    }
}
