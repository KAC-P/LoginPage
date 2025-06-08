using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SystemLogowaniaAdmina.Models;

namespace SystemLogowaniaAdmina.Controllers
{
    public class HomeController : Controller
    {
        private readonly List<UserModel> _users;

        public HomeController(IConfiguration configuration)
        {
            _users = configuration.GetSection("Users").Get<List<UserModel>>();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Panel()
        {
            // Przekazujemy listę użytkowników do widoku panelu admina
            return View(_users);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
