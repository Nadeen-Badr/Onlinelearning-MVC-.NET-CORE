using Microsoft.AspNetCore.Identity;

public interface IAuthRepository
{
    Task<IdentityResult> RegisterUserAsync(ApplicationUser user, string password, string role);
    Task<SignInResult> SignInUserAsync(string username, string password);
    Task SignOutAsync();
}
