using System.Collections.Generic;
using System.Threading.Tasks;


public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAllCoursesAsync();
    Task<Course> GetCourseByIdAsync(int id);
    Task<IEnumerable<Course>> GetCoursesByInstructorAsync(string instructorId);
    Task AddCourseAsync(Course course);
    Task UpdateCourseAsync(Course course);
    Task DeleteCourseAsync(Course course);
}
