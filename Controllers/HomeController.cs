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
        public IActionResult LoginUser(string email, string password)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("CheckLoginAccess", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                SqlParameter outputParam = new SqlParameter("@Result", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);

                try
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int employeeId = 0;
                        string firstName = "";
                        string lastName = "";
                        int designationId = 0;

                        if (reader.Read())
                        {
                            firstName = reader["FirstName"].ToString();
                            lastName = reader["LastName"].ToString();
                            designationId = Convert.ToInt32(reader["DesignationId"]);
                            employeeId = Convert.ToInt32(reader["UserId"]);
                        }

                        // Important: CLOSE the reader before accessing output param
                        reader.Close();
                        int result = (int)cmd.Parameters["@Result"].Value;

                        switch (result)
                        {
                            case 1:
                                // Store in session
                                HttpContext.Session.SetInt32("EmployeeId", employeeId);
                                HttpContext.Session.SetString("FirstName", firstName);
                                HttpContext.Session.SetString("LastName", lastName);
                                HttpContext.Session.SetString("Designation", GetDesignationName(Convert.ToInt32(designationId)));
                                HttpContext.Session.SetString("Email", email); // optional

                                TempData["Login"] = "Login successful.";
                                MarkAttendance(employeeId);
                                return RedirectToAction("Index", "Dashboard");

                            case -1:
                                TempData["Login"] = "Invalid email or password.";
                                return RedirectToAction("Login");

                            case -2:
                                TempData["Login"] = "Duplicate user entry detected.";
                                return RedirectToAction("Login");

                            default:
                                TempData["Login"] = "Unknown login status.";
                                return RedirectToAction("Login");
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Login"] = "An error occurred: " + ex.Message;
                    return RedirectToAction("Login");
                }
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Info"] = null;
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Registration()
        {
            ViewBag.DesignationList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Manager", Value = "2" },
                new SelectListItem { Text = "Software Developer", Value = "3" },
                new SelectListItem { Text = "Tester", Value = "4" }
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

                    // OUTPUT parameter
                    SqlParameter resultParam = new SqlParameter("@Result", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(resultParam);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        int result = (int)resultParam.Value;

                        if (result == 1)
                        {
                            TempData["Success"] = "User registered successfully.";
                            return RedirectToAction("Registration");
                        }
                        else if (result == -1)
                        {
                            TempData["Info"] = "Email already exists. Please try a different one.";
                            return RedirectToAction("Registration");
                        }
                        else
                        {
                            TempData["Error"] = "Registration failed. Please try again.";
                            return RedirectToAction("Registration");
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "An error occurred during registration. Please try again.";
                        return RedirectToAction("Registration");
                    }
                }
            }
        }


        string GetDesignationName(int designationCode)
        {
            switch (designationCode)
            {
                case 1: return "Admin";
                case 2: return "Manager";
                case 3: return "Software Developer";
                case 4: return "Tester";
                default: return "Unknown";
            }
        }

        void MarkAttendance(int employeeId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("InsertAttendance", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    SqlParameter output = new SqlParameter("@Result", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(output);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)cmd.Parameters["@Result"].Value;
                }
            }
        }
    }
}
