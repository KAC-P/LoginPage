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
            // Próba logowania jako admin
            var admin = _admins.FirstOrDefault(a => a.UserName == username && a.Password == password);

            if (admin != null)
            {
                var adminClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, admin.UserName),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim(ClaimTypes.Role, admin.Role)
        };

                var adminIdentity = new ClaimsIdentity(adminClaims, "CookieAuth");
                var adminPrincipal = new ClaimsPrincipal(adminIdentity);

                await HttpContext.SignInAsync("CookieAuth", adminPrincipal);
                return RedirectToAction("Index", "Home");
            }

            // Próba logowania jako użytkownik
            var user = _users.FirstOrDefault(u => u.UserName == username && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Niepoprawna nazwa użytkownika lub hasło";
                return View();
            }

            var userClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim("AccessTo", string.Join(",", user.AccessTo))
    };

            var userIdentity = new ClaimsIdentity(userClaims, "CookieAuth");
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            await HttpContext.SignInAsync("CookieAuth", userPrincipal);
            return RedirectToAction("Index", "Home");
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
