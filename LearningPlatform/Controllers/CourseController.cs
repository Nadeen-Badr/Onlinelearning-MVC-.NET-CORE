using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Identity;

[Authorize]
public class CourseController : Controller
{
    private readonly ICourseRepository _courseRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CourseController> _logger;
    private readonly IEnrollmentRepository _EnrollmentRepository;
      private readonly ILessonRepository _lessonRepository;
      private readonly ILessonProgressRepository _lessonProgressRepository;




   public CourseController(ICourseRepository courseRepository, UserManager<ApplicationUser> userManager, ILogger<CourseController> logger, IEnrollmentRepository EnrollmentRepository,ILessonRepository lessonRepository,ILessonProgressRepository lessonProgressRepository)
{
    _courseRepository = courseRepository;
    _userManager = userManager;
    _logger = logger;
    _EnrollmentRepository=EnrollmentRepository;
    _lessonRepository=lessonRepository;
     _lessonProgressRepository=lessonProgressRepository;
}

    // 1. View all courses for both students and instructors
    [AllowAnonymous]
   public async Task<IActionResult> Index()
{
    var userId = _userManager.GetUserId(User);
    var courses = await _courseRepository.GetAllCoursesAsync();
    
    // Dictionary to store enrollment status for each course
    var courseEnrollments = new Dictionary<int, bool>();
    foreach (var course in courses)
    {
        var isEnrolled = await _EnrollmentRepository.IsUserEnrolledInCourseAsync(userId, course.CourseId);
        courseEnrollments.Add(course.CourseId, isEnrolled);
    }

    // Pass the enrollment status dictionary to the view
    ViewBag.CourseEnrollments = courseEnrollments;

    return View(courses);
}

    // 2. Create course for instructors
    [Authorize(Roles = "Instructor")]
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Instructor")]
[HttpPost]
public async Task<IActionResult> Create(Course course, IFormFile Image)
{
    var currentUser = await _userManager.GetUserAsync(User);
    if (currentUser == null)
    {
        ModelState.AddModelError(string.Empty, "User not found.");
        return View(course);
    }

    // Ensure the InstructorId is set correctly and not being overridden
    course.InstructorId = currentUser.Id;
    _logger.LogInformation($"Instructor ID: {course.InstructorId}");  // Check the log to see if InstructorId is correct.

    if (ModelState.IsValid)
    {
        // Handle image upload
        if (Image != null && Image.Length > 0)
        {
            var fileName = Path.GetFileName(Image.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Image.CopyToAsync(stream);
            }

            course.ImageUrl = "/images/" + fileName;
        }

        await _courseRepository.AddCourseAsync(course);
        return RedirectToAction("MyCourses", "Course");
    }

    // Log any model errors
    foreach (var modelStateKey in ModelState.Keys)
    {
        var value = ModelState[modelStateKey];
        foreach (var error in value.Errors)
        {
            _logger.LogError($"Error in {modelStateKey}: {error.ErrorMessage}");
        }
    }

    return View(course);
}


    // 3. View instructor's courses (only the courses they created)
    [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> MyCourses()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var courses =await  _courseRepository.GetCoursesByInstructorAsync(userId);
        return View(courses);
    }

    // 4. Edit course
    [Authorize(Roles = "Instructor")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _courseRepository.GetCourseByIdAsync(id);
        if (course == null || course.InstructorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            return NotFound();
        }
        return View(course);
    }

 [Authorize(Roles = "Instructor")]
[HttpPost]
public async Task<IActionResult> Edit(int id, Course course, IFormFile? Image)
{
    _logger.LogInformation("Editing course with ID: {CourseId} by Instructor: {InstructorId}", id, User.FindFirstValue(ClaimTypes.NameIdentifier));

    if (ModelState.IsValid)
    {
        // Fetch the course to be edited
        var existingCourse = await _courseRepository.GetCourseByIdAsync(id);
        if (existingCourse == null)
        {
            _logger.LogWarning("Course with ID: {CourseId} not found.", id);
            return NotFound(); // Course doesn't exist
        }

        if (existingCourse.InstructorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            _logger.LogWarning("Instructor {InstructorId} is trying to edit a course they do not own. Course ID: {CourseId}", User.FindFirstValue(ClaimTypes.NameIdentifier), id);
            return Forbid(); // Instructor doesn't own the course
        }

        // Log the current course data before update
        _logger.LogInformation("Current Course Title: {CourseTitle}, Description: {CourseDescription}, Duration: {CourseDuration}, Price: {CoursePrice}", existingCourse.Title, existingCourse.Description, existingCourse.Duration, existingCourse.Price);

        // Update the course properties
        existingCourse.Title = course.Title;
        existingCourse.Description = course.Description;
        existingCourse.Duration = course.Duration;
        existingCourse.Price = course.Price;

        // Handle image upload
        if (Image != null && Image.Length > 0)
        {
            var fileName = Path.GetFileName(Image.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }
                existingCourse.ImageUrl = "/images/" + fileName;
                _logger.LogInformation("Image uploaded successfully: {ImageUrl}", existingCourse.ImageUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading image for Course ID: {CourseId}", id);
                ModelState.AddModelError(string.Empty, "There was an error uploading the image.");
                return View(course);
            }
        }

        // Update the course in the repository
        await _courseRepository.UpdateCourseAsync(existingCourse);
        _logger.LogInformation("Course with ID: {CourseId} updated successfully.", id);

        return RedirectToAction(nameof(MyCourses));
    }

    // If ModelState is not valid, log the errors
    _logger.LogWarning("Model state is invalid for course ID: {CourseId}.", id);
    foreach (var error in ModelState.Values)
    {
        foreach (var e in error.Errors)
        {
            _logger.LogError("Model Error: {Error}", e.ErrorMessage);
        }
    }

    return View(course);
}


    // 5. Delete course
    [Authorize(Roles = "Instructor")]
  public async Task<IActionResult> ConfirmDelete(int id)
    {
        var course = await _courseRepository.GetCourseByIdAsync(id);
        if (course == null)
        {
            return NotFound();
        }
        return View(course);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _courseRepository.GetCourseByIdAsync(id);
        if (course == null || course.InstructorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            return NotFound();
        }

        await _courseRepository.DeleteCourseAsync(course);
        return RedirectToAction(nameof(MyCourses));
    }// Location: /Controllers/CourseController.cs
public async Task<IActionResult> Detail(int id)
{
    var course = await _courseRepository.GetCourseByIdAsync(id);
    if (course == null)
    {
        return NotFound();
    }

    // Retrieve the lessons associated with this course
    var lessons = await _lessonRepository.GetLessonsByCourseIdAsync(id);

    // Get the user's lesson progress
    var userId = _userManager.GetUserId(User);
    var lessonProgress = await _lessonProgressRepository.GetLessonProgressByUserIdAsync(userId);

    // Create the ViewModel and populate it
    var viewModel = new CourseDetailViewModel
    {
        Course = course,
        Lessons = lessons,
        LessonProgress = lessonProgress
    };

    return View(viewModel);
}


 
}
