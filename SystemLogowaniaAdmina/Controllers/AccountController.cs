using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SystemLogowaniaAdmina.Models;


public class AccountController : Controller
{
    private readonly List<UserModel> _users;

    public AccountController(IConfiguration configuration)
    {
        // Odczyt użytkowników z appsettings.json
        _users = configuration.GetSection("Users").Get<List<UserModel>>();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        // Sprawdź, czy podane dane zgadzają się z którymś użytkownikiem
        var user = _users.Find(u => u.UserName == username && u.Password == password);

        if (user == null)
        {
            ViewBag.Error = "Niepoprawna nazwa użytkownika lub hasło";
            return View();
        }

        // Tworzymy claims (uprawnienia)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var identity = new ClaimsIdentity(claims, "CookieAuth");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("CookieAuth", principal);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
