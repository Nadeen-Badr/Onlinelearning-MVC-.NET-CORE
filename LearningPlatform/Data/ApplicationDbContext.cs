using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<QuizAttempt> QuizAttempts { get; set; }
    public DbSet<LessonProgress> LessonProgresses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<QuizOption> QuizOption { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Configuring the foreign key relationship between Enrollment and Course with No Action on Delete
    modelBuilder.Entity<Enrollment>()
        .HasOne(e => e.Course)
        .WithMany(c => c.Enrollments)
        .HasForeignKey(e => e.CourseId)
        .OnDelete(DeleteBehavior.NoAction);  // No cascading delete for Course -> Enrollment

    // Configuring the foreign key relationship between Enrollment and User (ApplicationUser) with No Action on Delete
    modelBuilder.Entity<Enrollment>()
        .HasOne(e => e.User)
        .WithMany()
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.NoAction);  // No cascading delete for User -> Enrollment

    // Configure QuizAttempt and Quiz relationship with No Action on Delete
    modelBuilder.Entity<QuizAttempt>()
        .HasOne(qa => qa.Quiz)
        .WithMany(q => q.QuizAttempts)
        .HasForeignKey(qa => qa.QuizId)
        .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<QuizAttempt>()
        .HasOne(qa => qa.User)
        .WithMany()
        .HasForeignKey(qa => qa.UserId)
        .OnDelete(DeleteBehavior.NoAction);
}

}
