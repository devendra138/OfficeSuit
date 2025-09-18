using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeSuit.Models;
using System.Data;

namespace OfficeSuit.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;


        public DashboardController(ILogger<DashboardController> logger, IConfiguration configuration, AppDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public IActionResult SetView()
        {
            int designationId = (int)HttpContext.Session.GetInt32("DesignationId");
            int employeeId = (int)HttpContext.Session.GetInt32("EmployeeId");

            switch (designationId)
            {
                case 1:
                    return RedirectToAction("Index");

                case 2:
                    return RedirectToAction("Index", "Manager", new { id = employeeId });

                case 3:
                    return RedirectToAction("DeveloperPage");

                case 4:
                    return RedirectToAction("TesterPage");

                default:
                    return RedirectToAction("");
            }
        }

        public IActionResult ManagerPage()
        {
            TempData["Login"] = null;
            return View();
        }

        public IActionResult DeveloperPage()
        {
            TempData["Login"] = null;
            return View();
        }

        public IActionResult TesterPage()
        {
            TempData["Login"] = null;
            return View();
        }

        public IActionResult Index()
        {
            TempData["Login"] = null;
            var users = GetAllUsers();
            return View(users);
        }

        public IActionResult Profile()
        {
            var profile = new UserProfile
            {
                Name = $"{HttpContext.Session.GetString("FirstName")}",
                Email = HttpContext.Session.GetString("Email"),
                Designation = HttpContext.Session.GetString("Designation"),
                ProfileImage = "/images/profile.jpg"
            };
            return View(profile);
        }

        public IActionResult EditProfile(UserProfile userProfile)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("UpdateUserInfo", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FirstName", userProfile.Name);
                cmd.Parameters.AddWithValue("@Designation", userProfile.Designation);
                cmd.Parameters.AddWithValue("@IsActive", userProfile.Email);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            return View();
        }

        public List<UserInfo> GetAllUsers()
        {
            List<UserInfo> userList = new List<UserInfo>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetAllUsers", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserInfo user = new UserInfo
                            {
                                UserId = Convert.ToInt32(reader["UserId"]),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString(),
                                DesignationId = Convert.ToInt32(reader["DesignationId"]),
                                IsActive = Convert.ToInt32(reader["IsActive"])
                            };
                            userList.Add(user);
                        }
                    }
                }
            }

            return userList;
        }

        public IActionResult UpdateUserStatus(int userId, int status)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("UpdateUserStatus", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@IsActive", status);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Attendance(int projectId)
        {
            var attendanceList = GetAllAttendance(projectId);

            ViewBag.Attendances = attendanceList;
            ViewBag.ProjectName = _context.Projects
                                  .FirstOrDefault(p => p.ProjectId == projectId)?.ProjectName;
            return View(attendanceList);
        }

        public List<Attendance> GetAllAttendance(int projectId)
        {
            var attendances = new List<Attendance>();

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("GetAllAttendance", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProjectId", projectId);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            attendances.Add(new Attendance
                            {
                                AttendanceId = Convert.ToInt32(reader["AttendanceId"]),
                                EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                                Date = Convert.ToDateTime(reader["Date"]),
                                Status = Convert.ToInt32(reader["Status"]),
                                UserName = reader["UserName"].ToString()
                            });
                        }
                    }
                }
            }

            return attendances;
        }
    }
}
