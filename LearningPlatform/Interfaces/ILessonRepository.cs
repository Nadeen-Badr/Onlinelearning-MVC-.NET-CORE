public interface ILessonRepository
{
    Task<Lesson> GetLessonByIdAsync(int lessonId);
    Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(int courseId);
    Task AddLessonAsync(Lesson lesson);
    Task UpdateLessonAsync(Lesson lesson);
    Task DeleteLessonAsync(int lessonId);
}
