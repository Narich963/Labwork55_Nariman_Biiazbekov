using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using To_Do_List.Models;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using To_Do_List.ViewModels;

namespace To_Do_List.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        return View(new LoginViewModel() { ReturnUrl = returnUrl });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            User user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                SignInResult result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return RedirectToAction("Index", "MyTask");
                }
            }
            ModelState.AddModelError("", "Invalid Email or password");
        }
        return View(model);
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            User? user = new User()
            {
                Email = model.Email,
                UserName = model.Name
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "MyTask");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }
    private async Task Authenticate(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email)
        };
        ClaimsIdentity id = new ClaimsIdentity(
                claims,
                "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType
            );
        await HttpContext.SignInAsync
            (
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(id),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                }
            );
    }
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}
