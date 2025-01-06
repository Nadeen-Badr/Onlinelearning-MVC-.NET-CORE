public class Course
{
    public int CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Duration { get; set; } // Duration in hours
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
      public string? ImageUrl { get; set; }
    // Instructor relationship
    public string? InstructorId { get; set; } // FK to ApplicationUser
    // Navigational property to Enrollments
    public ICollection<Enrollment>? Enrollments { get; set; }
       public ICollection<Lesson>? Lessons { get; set; }
}
