using Microsoft.AspNetCore.Mvc;
using SystemLogowaniaAdmina.Models;

namespace SystemLogowaniaAdmina.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Panel()
        {
            return View();
        }
    }
}