using Microsoft.AspNetCore.Identity;

public class AuthRepository : IAuthRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

 public async Task<IdentityResult> RegisterUserAsync(ApplicationUser user, string password, string role)
{
    // Check if user already exists
    var userExists = await _userManager.FindByNameAsync(user.UserName);
    if (userExists != null)
    {
        return IdentityResult.Failed(new IdentityError { Description = "User already exists." });
    }

    // Create user
    var result = await _userManager.CreateAsync(user, password);

    if (result.Succeeded)
    {
        // Add user to role if a role is provided and exists
        if (!string.IsNullOrEmpty(role))
        {
            var roleExist = await _roleManager.RoleExistsAsync(role);
            if (roleExist)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                result.Errors.Append(new IdentityError { Description = "Role does not exist" });
            }
        }
    }

    return result;
}


    public async Task<SignInResult> SignInUserAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return SignInResult.Failed;

        return await _signInManager.PasswordSignInAsync(user, password, false, false);
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
