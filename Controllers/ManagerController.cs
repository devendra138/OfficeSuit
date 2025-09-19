using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeSuit.Models;
using System.Data;
using System.Security.Cryptography.Pkcs;
using System.Threading.Tasks;

namespace OfficeSuit.Controllers
{
    public class ManagerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public ManagerController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public IActionResult Index(int id)
        {
            var project = _context.Projects
                              .FirstOrDefault(p => p.ProjectManagerID == id);
            if (project != null)
            {
                ViewBag.ProjectName = project.ProjectName;
                var resources = GetProjectResources(project.ProjectId);
                ViewBag.ProjectId = project.ProjectId;
                HttpContext.Session.SetInt32("ProjectId", project.ProjectId);
                ViewBag.Resources = resources;

                var users = _context.UserInfo
                        .Where(u => u.IsActive == 1 && u.DesignationId != 1 && u.DesignationId != 2)
                        .Select(u => new { u.UserId, FullName = u.FirstName + " " + u.LastName })
                        .ToList();
                ViewBag.Users = new SelectList(users, "UserId", "FullName");
            }
            else
            {
                ViewBag.ProjectName = "No project assigned yet.";
            }
            return View();
        }

        public List<ProjectResource> GetProjectResources(int projectId)
        {
            List<ProjectResource> resources = new List<ProjectResource>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetProjectResources", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", projectId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProjectResource user = new ProjectResource
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                UserName = reader["UserName"].ToString(),
                                DesignationId = Convert.ToInt32(reader["DesignationId"]),
                                DesignationName = reader["DesignationName"].ToString()
                            };
                            resources.Add(user);
                        }
                    }
                }
            }
            return resources;
        }


        public void AddProjectResource(int projectId, int userId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("AddProjectResource", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProjectId", projectId);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // GET: Add Resource Form (Partial)
        [HttpGet]
        public IActionResult AddResource(int projectId)
        {
            ViewBag.ProjectId = projectId;

            ViewBag.Users = GetProjectResources(projectId);

            return PartialView("_AddResourcePartial");
        }

        public IActionResult Tasks(int projectId)
        {
            var tasks = _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .ToList();

            ViewBag.ProjectId = projectId;
            ViewBag.Tasks = tasks;
            ViewBag.Resources = GetProjectResources(projectId);
            return View();
        }

        [HttpPost]
        public IActionResult AddTask(Tasks task)
        {
            var user = _context.UserInfo.FirstOrDefault(u => u.UserId == task.UserId);
            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("AddTask", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TaskName", task.TaskName);
                        cmd.Parameters.AddWithValue("@Description", task.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AssignTo", user.FirstName + " " + user.LastName);
                        cmd.Parameters.AddWithValue("@ProjectId", task.ProjectId);
                        cmd.Parameters.AddWithValue("@StartDate", task.StartDate);
                        cmd.Parameters.AddWithValue("@EndDate", task.EndDate);
                        cmd.Parameters.AddWithValue("@Status", task.Status);
                        cmd.Parameters.AddWithValue("@UserId", task.UserId);

                        conn.Open();
                        var result = cmd.ExecuteScalar();
                    }
                }
                return RedirectToAction("Tasks", new { projectId = task.ProjectId });
            }

            ViewBag.ProjectId = task.ProjectId;
            return View("Tasks");
        }
    }
}
