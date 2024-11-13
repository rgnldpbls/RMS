using Microsoft.AspNetCore.Identity; // Import the Identity namespace
using Microsoft.Extensions.Configuration;
using CRE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CRE.Interfaces;
using ResearchManagementSystem.Models;

namespace CRE.Controllers
{
    [Area("CreSys")]
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
                // Store user information in ViewBag for use in the view

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);
                ViewBag.UserRoles = roles; // Store roles in ViewBag for use in the view

                // Get the current role from the session or default to Researcher
                string currentRole = HttpContext.Session.GetString("CurrentRole");

                // Set the default if no current role is set
                if (string.IsNullOrEmpty(currentRole))
                {
                    if (!roles.Contains("Researcher") && roles.Contains("Chief"))
                    {
                        HttpContext.Session.SetString("CurrentRole", "Chief"); // Default to Chief if no Researcher
                    }
                    else
                    {
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

            // Redirect back to the Index action
            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new CRE.Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
