public interface ILessonProgressRepository
{    Task MarkLessonAsReadAsync(string userId, int lessonId);
    Task<bool> IsLessonMarkedAsReadAsync(string userId, int lessonId);
    Task<IEnumerable<LessonProgress>> GetLessonProgressByUserIdAsync(string userId);
    Task<int> GetReadLessonsCountAsync(string userId, int courseId);
}