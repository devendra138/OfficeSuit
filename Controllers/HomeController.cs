using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OfficeSuit.Models;
using System.Data;

namespace OfficeSuit.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly string connectionString = "Server=DESKTOP-IDJHK8U;Database=OfficeSuit;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";



        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
                TempData["Info"] = "Login Successful.";
                return RedirectToAction("Login");
            }
            else
            {
                TempData["Info"] = "Wrong username or password. Please try again.";
                return RedirectToAction("Login");
            }
        }

        public IActionResult Registration()
        {
            return View();
        }

        public IActionResult UserRegistration(UserInfo user)
        {
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
                    cmd.Parameters.AddWithValue("@Designation", user.Designation);

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            TempData["Info"] = "Registration successful.";
                            return RedirectToAction("Registration");
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
                        // Log error (ex)
                        return RedirectToAction("Registration");
                    }
                }
            }
        }
    }
}
