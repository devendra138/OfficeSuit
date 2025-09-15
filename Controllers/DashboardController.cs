using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OfficeSuit.Models;
using System.Data;

namespace OfficeSuit.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IConfiguration _configuration;

        public DashboardController(ILogger<DashboardController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult SetView()
        {
            int designationId = (int)HttpContext.Session.GetInt32("DesignationId");
            switch (designationId)
            {
                case 1:
                    return RedirectToAction("Index");

                case 2:
                    return RedirectToAction("ManagerPage");

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

        public IActionResult Attendance()
        {
            return View();
        }
    }
}
