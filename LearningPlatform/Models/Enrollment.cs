public class Enrollment
{
    public int EnrollmentId { get; set; }
    public string UserId { get; set; } // FK to ApplicationUser
    public ApplicationUser? User { get; set; }
    public int CourseId { get; set; } // FK to Course
    public Course? Course { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public decimal Progress { get; set; } // Percentage progress (use decimal for precision)
}
