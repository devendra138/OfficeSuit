using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OfficeSuit.Models;

namespace OfficeSuit.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginUser(string username, string password)
        {
            if (username == "Darshan" && password == "Darshan@123")
            {
                TempData["Info"] = "Login Successful.";
                return RedirectToAction("Login");
            }
            else
            {
                TempData["Info"] = "Wrong username or password. Please try again.";
                return RedirectToAction("Login");
            }
        }

        public IActionResult Registration()
        {
            return View();
        }

        public IActionResult UserRegistration(UserInfo userInfo)
        {

            return View();
        }
    }
}
