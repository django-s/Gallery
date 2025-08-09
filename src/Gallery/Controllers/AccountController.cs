using Gallery.Database;
using Gallery.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Gallery.Controllers;

public class AccountController : Controller
{
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, [FromServices] SignInManager<User> signInManager)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        SignInResult result =
            await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Username or password is incorrect.");
        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> Logout([FromServices] SignInManager<User> signInManager)
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public IActionResult ChangePassword() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model,
        [FromServices] UserManager<User> userManager)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string? userName = User.Identity?.Name;
        if (userName == null)
        {
            return Error();
        }

        User? user = await userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return Error();
        }

        IdentityResult result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            return Error();
        }

        return RedirectToAction("Index", "Home");

        IActionResult Error()
        {
            ModelState.AddModelError(string.Empty, "Failed to change password. Please try again.");
            return View(model);
        }
    }
}
