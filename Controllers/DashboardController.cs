using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeSuit.Models;

namespace OfficeSuit.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        public IActionResult Profile()
        {
            var profile = new UserProfile
            {
                Name = "Darshan Rawool",
                Email = "darshan@example.com",
                Designation = "Software Developer",
                ProfileImage = "/images/profile.jpg"
            };
            return View(profile);
        }

        public IActionResult EditProfile(UserProfile userProfile)
        {            
            return View(userProfile);
        }
    }
}
