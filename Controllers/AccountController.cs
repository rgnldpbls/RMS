using DocumentTrackingSystemBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RemcSys.Data;
using ResearchManagementSystem.Areas.CreSys.Data;
using ResearchManagementSystem.Data;
using ResearchManagementSystem.Models;
using ResearchManagementSystem.Services;
using rscSys_final.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;

namespace ResearchManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public AccountController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Authenticate user (your authentication logic)
                // If login is successful:

                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User); // Get the current logged-in user
                    var userId = user?.Id; // Get the user's ID

                    if (!string.IsNullOrEmpty(userId))
                    {
                        var log = new UserActivityLog
                        {
                            UserId = userId,
                            Activity = "Login",
                            Timestamp = DateTime.UtcNow
                        };

                        _context.ActivityLog.Add(log);
                        _context.SaveChanges();
                    }
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User); // Get the current logged-in user
                var userId = user?.Id; // Get the user's ID

                if (!string.IsNullOrEmpty(userId))
                {
                    var log = new UserActivityLog
                    {
                        UserId = userId,
                        Activity = "Logout",
                        Timestamp = DateTime.UtcNow
                    };

                    _context.ActivityLog.Add(log);
                    _context.SaveChanges();
                }
            }

            // Clear user session or authentication token
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ActivityLogs()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user’s ID

            var logs = _context.ActivityLog
                .Where(log => log.UserId == userId)
                .OrderByDescending(log => log.Timestamp)
                .ToList();

            return View(logs);
        }



        public async Task<IActionResult> UserActivityHistory()
        {
            var user = await _signInManager.UserManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            // Retrieve activity log for the current user
            var dbContext = HttpContext.RequestServices.GetService<ApplicationDbContext>();
            var activities = dbContext.ActivityLog
                .Where(a => a.UserId == user.Id)
                .OrderByDescending(a => a.Timestamp)
                .ToList();

            // Extract the latest login and logout times
            var lastLogin = activities.FirstOrDefault(a => a.Activity == "Logged in")?.Timestamp;
            var lastLogout = activities.FirstOrDefault(a => a.Activity == "Logged out")?.Timestamp;

            // Pass the data to the view
            ViewBag.LastLoginTime = lastLogin;
            ViewBag.LastLogoutTime = lastLogout;

            return View(activities);
        }


        public async Task<IActionResult> AllUserActivityHistory()
        {
            var user = await _signInManager.UserManager.GetUserAsync(User);

            if (user == null || !await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                return Unauthorized();
            }

            var dbContext = HttpContext.RequestServices.GetService<ApplicationDbContext>();
            var allActivities = dbContext.ActivityLog
                .OrderByDescending(a => a.Timestamp)
                .ToList();

            var userLogs = allActivities
                .GroupBy(a => a.UserId)
                .Select(g => new UserActivityViewModel
                {
                    UserId = g.Key,
                    UserName = _userManager.FindByIdAsync(g.Key).Result.UserName, // Fetch username
                    LastLogin = g.Where(a => a.Activity == "Logged in").OrderByDescending(a => a.Timestamp).FirstOrDefault()?.Timestamp,
                    LastLogout = g.Where(a => a.Activity == "Logged out").OrderByDescending(a => a.Timestamp).FirstOrDefault()?.Timestamp,
                    Activities = g.ToList()
                })
                .ToList();

            return View(userLogs);
        }

        public async Task<IActionResult> EditFacultyRank(string id)
        {
            // Retrieve the faculty member by their ID
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Calculate the duration (in years and months)
            string rankDuration = "";
            if (user.RankStartDate.HasValue)
            {
                var startDate = user.RankStartDate.Value;
                var endDate = user.RankEndDate ?? DateTime.Now; // Use current date if no end date
                var duration = endDate - startDate;

                // Calculate years and months
                int years = (int)(duration.Days / 365.25);  // Considering leap years
                int months = (int)((duration.Days % 365.25) / 30);  // Approximate months

                rankDuration = $"{years} year(s) {months} month(s)";
            }

            // Create a ViewModel to hold the data
            var model = new EditFacultyRankViewModel
            {
                Id = user.Id,
                Rank = user.Rank,
                RankStartDate = user.RankStartDate,
                RankEndDate = user.RankEndDate,
                RankDuration = rankDuration // Include the duration in the ViewModel
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFacultyRank (EditFacultyRankViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the faculty member by their ID
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user == null)
                {
                    return NotFound();
                }

                // Update the user's rank, start date, and end date
                user.Rank = model.Rank;
                user.RankStartDate = model.RankStartDate;
                user.RankEndDate = model.RankEndDate;

                // Save the changes
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("FacultyList"); // Redirect to the list or another page after success
                }
                else
                {
                    // Handle errors
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If model state is invalid, return to the form with validation errors
            return View(model);
        }


    }

}





