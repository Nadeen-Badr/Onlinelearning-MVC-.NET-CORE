using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class LessonController : Controller
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonProgressRepository _lessonProgressRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<LessonController> _logger;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public LessonController(ILessonRepository lessonRepository, ICourseRepository courseRepository, ILessonProgressRepository lessonProgressRepository, 
                            UserManager<ApplicationUser> userManager, ILogger<LessonController> logger, IEnrollmentRepository enrollmentRepository)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
        _lessonProgressRepository = lessonProgressRepository;
        _userManager = userManager;
        _logger = logger;
        _enrollmentRepository = enrollmentRepository;
    }

    // Create Lesson for Instructor
    [Authorize(Roles = "Instructor")]
    [HttpGet]
    public IActionResult Create(int courseId)
    {
        var lesson = new Lesson
        {
            CourseId = courseId
        };
        return View(lesson);
    }

    [Authorize(Roles = "Instructor")]
    [HttpPost]
    public async Task<IActionResult> Create(Lesson lesson)
    {
        if (lesson.CourseId == 0)
        {
            ModelState.AddModelError("CourseId", "CourseId is required.");
            return View(lesson);
        }

        try
        {
            await _lessonRepository.AddLessonAsync(lesson);
            return RedirectToAction("Detail", "Course", new { id = lesson.CourseId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while creating the lesson.");
            return View(lesson);
        }
    }

    // View all lessons for a specific course
    public async Task<IActionResult> Index(int courseId)
    {
        var lessons = await _lessonRepository.GetLessonsByCourseIdAsync(courseId);
        return View(lessons);
    }

    // Lesson Details for Student
    [HttpGet]
    public async Task<IActionResult> LessonDetails(int id)
    {
        var userId = _userManager.GetUserId(User);
        var isRead = await _lessonProgressRepository.IsLessonMarkedAsReadAsync(userId, id);
        ViewBag.IsRead = isRead;

        // Fetch and return lesson details
        return View();
    }

    // Mark Lesson as Read for Student
    [Authorize(Roles = "Student")]
    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int lessonId)
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        bool alreadyRead = await _lessonProgressRepository.IsLessonMarkedAsReadAsync(userId, lessonId);

        if (!alreadyRead)
        {
            await _lessonProgressRepository.MarkLessonAsReadAsync(userId, lessonId);
        }

        // Retrieve the course associated with the lesson
        var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);
        var courseId = lesson.CourseId;

        // Retrieve all lessons for the course
        var totalLessons = await _lessonRepository.GetLessonsByCourseIdAsync(courseId);
        var readLessons = await _lessonProgressRepository.GetReadLessonsCountAsync(userId, courseId);

        // Calculate the progress
        decimal readLessonsDecimal = (decimal)readLessons;
        decimal totalLessonsDecimal = (decimal)totalLessons.Count();
        decimal progress = (readLessonsDecimal / totalLessonsDecimal) * 100;

        // Update the student's enrollment progress
        var enrollment = await _enrollmentRepository.GetEnrollmentByUserIdAndCourseIdAsync(userId, courseId);
        if (enrollment != null)
        {
            enrollment.Progress = progress;
            await _enrollmentRepository.UpdateEnrollmentAsync(enrollment);
        }

        return RedirectToAction("MyCourses", "Enrollment");
    }
public async Task<IActionResult> Detail(int lessonId)
{
    var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);

    if (lesson == null)
    {
        return NotFound();
    }

    return View(lesson); // Ensure this matches the view's name and location
}

}
