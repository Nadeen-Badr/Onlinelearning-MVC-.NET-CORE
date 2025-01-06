using LearningPlatform.Models;
using System.Collections.Generic;

public interface IEnrollmentRepository
{
    Task EnrollInCourseAsync(string userId, int courseId);
    Task<IEnumerable<Enrollment>> GetEnrollmentsByUserIdAsync(string userId);
    Task<bool> IsUserEnrolledInCourseAsync(string userId, int courseId);
    Task UnenrollFromCourseAsync(string userId, int courseId);
    Task<Enrollment> GetEnrollmentByUserIdAndCourseIdAsync(string userId, int courseId);
    Task UpdateEnrollmentAsync(Enrollment enrollment);
}
