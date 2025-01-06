using Microsoft.EntityFrameworkCore;

public class LessonProgressRepository : ILessonProgressRepository
{
    private readonly ApplicationDbContext _context;

    public LessonProgressRepository(ApplicationDbContext context)
    {
        _context = context;
    }
public async Task MarkLessonAsReadAsync(string userId, int lessonId)
{
    var lessonProgress = new LessonProgress
    {
        UserId = userId,
        LessonId = lessonId,
        IsRead = true,
        DateRead = DateTime.Now
    };

    _context.LessonProgresses.Add(lessonProgress);
    await _context.SaveChangesAsync();
}

    public async Task<bool> IsLessonMarkedAsReadAsync(string userId, int lessonId)
    {
        return await _context.LessonProgresses
            .AnyAsync(lp => lp.UserId == userId && lp.LessonId == lessonId && lp.IsRead);
    }

    public async Task<IEnumerable<LessonProgress>> GetLessonProgressByUserIdAsync(string userId)
    {
        return await _context.LessonProgresses
            .Where(lp => lp.UserId == userId)
            .ToListAsync();
    }
    public async Task<Enrollment> GetEnrollmentByUserIdAndCourseIdAsync(string userId, int courseId)
{
    return await _context.Enrollments
        .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
}
public async Task<int> GetReadLessonsCountAsync(string userId, int courseId)
{
    return await _context.LessonProgresses
        .Where(lp => lp.UserId == userId && lp.Lesson.CourseId == courseId && lp.IsRead)
        .CountAsync();
}


}
