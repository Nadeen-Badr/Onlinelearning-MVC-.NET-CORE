using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Student")]
public class EnrollmentController : Controller
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public EnrollmentController(IEnrollmentRepository enrollmentRepository, ICourseRepository courseRepository, UserManager<ApplicationUser> userManager)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _userManager = userManager;
    }

    // View all courses the student is enrolled in
    public async Task<IActionResult> MyCourses()
    {
        var userId = _userManager.GetUserId(User);
        var enrollments = await _enrollmentRepository.GetEnrollmentsByUserIdAsync(userId);
        return View(enrollments);
    }

    // Enroll in a course
    [HttpPost]
    public async Task<IActionResult> Enroll(int courseId)
    {
        var userId = _userManager.GetUserId(User);
        try
        {
            await _enrollmentRepository.EnrollInCourseAsync(userId, courseId);
            return RedirectToAction(nameof(MyCourses));
        }
        catch (Exception ex)
        {
            // Handle the case when the user is already enrolled
            ModelState.AddModelError(string.Empty, ex.Message);
            return RedirectToAction("Index", "Course"); // Or where you display all courses
        }
    }
    // Unenroll from a course
[HttpPost]
public async Task<IActionResult> Unenroll(int courseId)
{
    var userId = _userManager.GetUserId(User);
    try
    {
        // Call the repository to unenroll the user
        await _enrollmentRepository.UnenrollFromCourseAsync(userId, courseId);
        return RedirectToAction(nameof(MyCourses)); // Redirect to the list of courses the user is enrolled in
    }
    catch (Exception ex)
    {
        // Handle any errors (e.g., course not found or other issues)
        ModelState.AddModelError(string.Empty, ex.Message);
        return RedirectToAction("Index", "Course"); // Or wherever you want to show all courses
    }
}

}
