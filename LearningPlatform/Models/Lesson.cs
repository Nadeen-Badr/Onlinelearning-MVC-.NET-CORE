public class Lesson
{
    public int LessonId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; } // Content can be text, links to videos, etc.
    public int CourseId { get; set; } // FK to Course
    public Course? Course { get; set; }
}
