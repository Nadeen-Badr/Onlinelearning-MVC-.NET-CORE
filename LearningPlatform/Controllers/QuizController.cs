using Microsoft.AspNetCore.Mvc;

public class QuizController : Controller
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IQuizRepository _quizRepository; // Your quiz repository
    private readonly IQuizOptionRepository _quizOptionRepository; // Your quiz option repository

    public QuizController(ILessonRepository lessonRepository, IQuizRepository quizRepository, IQuizOptionRepository quizOptionRepository)
    {
        _lessonRepository = lessonRepository;
        _quizRepository = quizRepository;
        _quizOptionRepository = quizOptionRepository;
    }

public async Task<IActionResult> Create(int courseId)
{
    // Get lessons related to the specific course
    var lessons = await _lessonRepository.GetLessonsByCourseIdAsync(courseId);
    ViewBag.Lessons = lessons;
    ViewBag.CourseId = courseId; // Pass CourseId to the view

    return View();
}


    // POST: Create Quiz
    [HttpPost]
    public async Task<IActionResult> Create(Quiz quiz, List<QuizOption> options)
    {
        if (ModelState.IsValid)
        {
            // Create the quiz
            quiz.Options = options;
            await _quizRepository.AddQuizAsync(quiz);

            // Save the options
            foreach (var option in options)
            {
                option.QuizId = quiz.QuizId;
                await _quizOptionRepository.AddOptionAsync(option);
            }

            return RedirectToAction("Index"); // Redirect to a list of quizzes or quiz details page
        }

        // If there is an error, return to the form with validation errors
        return View(quiz);
    }
}
