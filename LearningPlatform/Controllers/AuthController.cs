using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    private readonly IAuthRepository _authRepository;

    public AuthController(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    // Display the registration page (GET request)
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // Sign up for student or instructor (POST request)
[HttpPost]
public async Task<IActionResult> Register(string username, string password, string fullName, string role)
{
    var user = new ApplicationUser
    {
        UserName = username,
        FullName = fullName,
    };

    var result = await _authRepository.RegisterUserAsync(user, password, role);

    if (result.Succeeded)
    {
        // User successfully registered
        return RedirectToAction("Login");
    }
    else
    {
        // Display the error message for the user
        foreach (var error in result.Errors)
        {
            // Check for password-related errors
            if (error.Description.Contains("password"))
            {
                ModelState.AddModelError(string.Empty, $"Password error: {error.Description}");
            }
            else
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(); // Re-render the registration form with error messages
    }
}


    // Display the login page (GET request)
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // Sign in (POST request)
    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var result = await _authRepository.SignInUserAsync(username, password);
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        return View("Login", "Invalid login attempt.");
    }

    // Logout
    public async Task<IActionResult> Logout()
    {
        await _authRepository.SignOutAsync();
        return RedirectToAction("Login");
    }
}
