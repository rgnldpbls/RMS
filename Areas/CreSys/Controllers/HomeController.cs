using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // Import the Identity namespace
using Microsoft.AspNetCore.Mvc;
using ResearchManagementSystem.Models;
using System.Diagnostics;

namespace CRE.Controllers
{
    [Area("CreSys")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration; // Configuration variable
        private readonly UserManager<ApplicationUser> _userManager; // Add UserManager for identity user

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager; // Initialize UserManager
        }
        public async Task<IActionResult> Index()
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);
                ViewBag.UserRoles = roles; // Store roles in ViewBag for use in the view

                // Get the current role from the session or default to Researcher
                string currentRole = HttpContext.Session.GetString("CurrentRole");

                // Set the default if no current role is set
                if (string.IsNullOrEmpty(currentRole))
                {
                    if (!roles.Contains("Student") && roles.Contains("CRE Chief"))
                    {
                        HttpContext.Session.SetString("CurrentRole", "CRE Chief"); // Default to Chief if no Researcher
                    }
                    else if (roles.Contains("External Researcher"))
                    {
                        HttpContext.Session.SetString("CurrentRole", roles.FirstOrDefault(r => r == "Researcher") ?? roles.FirstOrDefault());
                    }
                    else
                    {
                        // Default to "Researcher" if available, else choose the first role
                        HttpContext.Session.SetString("CurrentRole", roles.FirstOrDefault(r => r == "Researcher") ?? roles.FirstOrDefault());
                    }
                }
            }
            else
            {
                ViewBag.UserRoles = new List<string>(); // No user logged in
                ViewBag.User = null; // Clear user data if not logged in
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SwitchRole(string roleName)
        {
            // Check if the roleName is not null or empty
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Role name cannot be null or empty.");
            }

            // Store the selected role in session
            HttpContext.Session.SetString("CurrentRole", roleName);
            return RedirectToAction("Index", "Home", new { area = "CreSys" });
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
