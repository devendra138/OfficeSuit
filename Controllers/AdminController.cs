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
            var projects = _context.Projects
                                   .Include(p => p.Manager)
                                   .ToList();
            // pass managers for dropdown
            ViewBag.Managers = _context.UserInfo
                                       .Where(u => u.DesignationId == 2)
                                       .ToList();

            return View(projects);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProjectModel project)
        {
            if (ModelState.IsValid)
            {
                _context.Projects.Add(project);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Managers = _context.UserInfo.Where(u => u.DesignationId == 2).ToList();
            return RedirectToAction("Index");
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
