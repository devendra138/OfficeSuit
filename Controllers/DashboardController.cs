using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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
        public IActionResult Index()
        {
            var users = GetAllUsers();
            return View(users);
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
                                DesignationId = Convert.ToInt32(reader["DesignationId"])
                            };
                            userList.Add(user);
                        }
                    }
                }
            }

            return userList;
        }
    }
}
