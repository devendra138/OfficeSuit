using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using OfficeSuit.Models;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace OfficeSuit.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginUser(string username, string password)
        {
            if (username == "Darshan" && password == "Darshan@123")
            {
                HttpContext.Session.SetString("UserName", "Darshan");
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                TempData["Info"] = "Wrong username or password. Please try again.";
                return RedirectToAction("Login");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Registration()
        {
            ViewBag.DesignationList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Software Developer", Value = "1" },
                new SelectListItem { Text = "Manager", Value = "2" },
                new SelectListItem { Text = "Tester", Value = "3" }
            };
            return View();
        }

        public IActionResult UserRegistration(UserInfo user)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("InsertUserInfo", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@Contact", user.Contact);
                    cmd.Parameters.AddWithValue("@Gender", user.Gender);
                    cmd.Parameters.AddWithValue("@DesignationId", user.DesignationId);

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["Info"] = "Registration failed. Please try again.";
                            return RedirectToAction("Registration");
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Info"] = "An error occurred during registration. Please try again.";
                        return RedirectToAction("Registration");
                    }
                }
            }
        }
    }
}
