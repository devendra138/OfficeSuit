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
            // get ProjectId from session
            //int? projectId = HttpContext.Session.GetInt32("ProjectId");

            //if (projectId == null)
            //{
            //    return RedirectToAction("Index", "Projects"); // or wherever you want
            //}

            // fetch task only if it belongs to that project
            var task = _context.Tasks
                               .FirstOrDefault(t => t.UserId == id );

            if (task == null)
            {
                return NotFound(); // Task not found or not in this project
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
