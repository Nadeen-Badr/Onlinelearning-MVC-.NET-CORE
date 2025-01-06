// Location: /ViewModels/CourseDetailViewModel.cs
public class CourseDetailViewModel
{
    public Course Course { get; set; }
    public IEnumerable<Lesson> Lessons { get; set; }
    public IEnumerable<LessonProgress> LessonProgress { get; set; }
}
