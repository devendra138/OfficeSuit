using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeSuit.Models;

namespace OfficeSuit.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var projectList = _context.Projects.ToList(); // fetch from DB
            return View(projectList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {
                //_context.Projects.Add(project);
                //_context.SaveChanges();
                //return RedirectToAction(nameof(Index));
                var projects = _context.Projects
                           .Include(p => p.Manager) // join with UserInfo
                           .ToList();
                return View(projects);
            }

            // If validation fails, reload Index with list
            return View("Index", _context.Projects.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var project = _context.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
