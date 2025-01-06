using Microsoft.EntityFrameworkCore;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _context;

    public EnrollmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Enroll a student in a course
    public async Task EnrollInCourseAsync(string userId, int courseId)
    {
        // Check if the student is already enrolled in the course
        var existingEnrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (existingEnrollment != null)
        {
            throw new Exception("User is already enrolled in this course.");
        }

        var enrollment = new Enrollment
        {
            UserId = userId,
            CourseId = courseId,
            EnrollmentDate = DateTime.Now,
            Progress = 0 // Initial progress is 0%
        };

        await _context.Enrollments.AddAsync(enrollment);
        await _context.SaveChangesAsync();
    }

    public async Task<Enrollment> GetEnrollmentByUserIdAndCourseIdAsync(string userId, int courseId)
{
    return await _context.Enrollments
        .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
}


    // Get all enrollments for a student
    public async Task<IEnumerable<Enrollment>> GetEnrollmentsByUserIdAsync(string userId)
    {
        return await _context.Enrollments
            .Where(e => e.UserId == userId)
            .Include(e => e.Course) // Include related course information
            .ToListAsync();
    }
    public async Task<bool> IsUserEnrolledInCourseAsync(string userId, int courseId)
{
    var enrollment = await _context.Enrollments
        .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
    return enrollment != null;
}
public async Task UnenrollFromCourseAsync(string userId, int courseId)
{
    var enrollment = await _context.Enrollments
        .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

    if (enrollment == null)
    {
        throw new Exception("Enrollment not found.");
    }

    _context.Enrollments.Remove(enrollment);
    await _context.SaveChangesAsync();
}
public async Task UpdateEnrollmentAsync(Enrollment enrollment)
{
    // Find the enrollment record
    var existingEnrollment = await _context.Enrollments
        .FirstOrDefaultAsync(e => e.EnrollmentId == enrollment.EnrollmentId);

    if (existingEnrollment != null)
    {
        // Update the progress
        existingEnrollment.Progress = enrollment.Progress;

        // Save the changes to the database
        await _context.SaveChangesAsync();
    }
}


}
