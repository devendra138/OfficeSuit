using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeSuit.Models;

namespace OfficeSuit.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public EmployeeController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index(int id)
        {
            var task = _context.Tasks
                               .FirstOrDefault(t => t.UserId == id);

            if (task == null)
            {
                ViewBag.Message = "No task found for this employee.";
                return View();   // ✅ load Index.cshtml without model
            }

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditStatus(int id, string status)
        {
            var task = _context.Tasks
                               .FirstOrDefault(t => t.TaskId == id);

            if (task == null)
            {
                return NotFound();
            }

            // update only status
            task.Status = status;
            _context.SaveChanges();

            return RedirectToAction("Index", new { id = task.UserId });
        }
    }
}
