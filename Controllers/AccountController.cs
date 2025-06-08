using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SystemLogowaniaAdmina.Models;

namespace SystemLogowaniaAdmina.Controllers
{
    public class AccountController : Controller
    {
        private readonly List<AdminModel> _admins;
        private readonly List<UserModel> _users;

        public AccountController(IConfiguration configuration)
        {
            _admins = configuration.GetSection("Admins").Get<List<AdminModel>>();
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
            var admin = _admins.Find(a => a.UserName == username && a.Password == password);

            if (admin == null)
            {
                ViewBag.Error = "Niepoprawna nazwa użytkownika lub hasło";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.UserName),
                new Claim(ClaimTypes.Role, admin.Role),
                new Claim(ClaimTypes.Email, admin.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);


            // Przekierowanie do panelu admina
            return RedirectToAction("Index","Home");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Panel()
        {
            // Przekazujemy listę użytkowników do widoku panelu admina
            return View("~/Views/Admin/Panel.cshtml", _users);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
