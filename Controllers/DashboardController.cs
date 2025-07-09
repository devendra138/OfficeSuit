using Microsoft.AspNetCore.Mvc;

namespace OfficeSuit.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
