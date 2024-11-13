using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ResearchManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using ResearchManagementSystem.Services;

namespace ResearchManagementSystem.Controllers
{

    public class UserRolesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserService _userService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserRolesController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, UserService userService, SignInManager<ApplicationUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userService = userService;
            _signInManager = signInManager; // Initialize SignInManager
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();
            foreach (ApplicationUser user in users)
            {
                var thisViewModel = new UserRolesViewModel();
                thisViewModel.Id = user.Id;
                thisViewModel.Email = user.Email;
                thisViewModel.FirstName = user.FirstName;
                thisViewModel.LastName = user.LastName;
                thisViewModel.Roles = await GetUserRoles(user);
                userRolesViewModel.Add(thisViewModel);
            }
            return View(userRolesViewModel);
        }
        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }


        public async Task<IActionResult> Manage(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User not found.";
                return View("Error");  // You may want to redirect to a different view
            }

            var roles = _roleManager.Roles.ToList();  // Get all roles
            var userRoles = await _userManager.GetRolesAsync(user);  // Get the user's current roles

            var model = roles.Select(role => new ManageUserRolesViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Selected = userRoles.Contains(role.Name)  // Check if the user has the role
            }).ToList();

            ViewBag.UserName = user.UserName;
            ViewBag.UserId = userId;

            return View(model);  // Pass the model to the view
        }


        [HttpPost]
        public async Task<IActionResult> Manage(List<ManageUserRolesViewModel> model, string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return View();
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction("Index");
        }



        // List all faculty
        public async Task<IActionResult> FacultyList()
        {
            var faculty = await _userService.GetUsersInRoleAsync("Faculty");
            return View(faculty); // Pass the list to the view
        }

        // List all students
        public async Task<IActionResult> StudentList()
        {
            var students = await _userService.GetUsersInRoleAsync("Student");
            return View(students); // Pass the list to the view
        }

        public IActionResult SwitchRole(string selectedRole)
        {
            if (!string.IsNullOrEmpty(selectedRole))
            {
               if(selectedRole == "SuperAdmin")
               {
                    return RedirectToAction("Index", "RoleManager");
               }
               else if(selectedRole == "Faculty")
               {
                    return RedirectToAction("Index", "Faculty");
               }
               else if(selectedRole == "RMCC")
               {
                    return RedirectToAction("Index", "ClusterCoordinator");
               }
               else if(selectedRole == "REMC Chief")
                {
                    return RedirectToAction("Chief", "Home", new { area = "RemcSys" });
                }
               else if(selectedRole == "REMC Evaluator")
                {
                    return RedirectToAction("Evaluator", "Home", new { area = "RemcSys" });
                }
               else if(selectedRole == "RSC Chief")
                {
                    return RedirectToAction("Index", "RSCChief", new { area = "RscSys" });
                }
               else if(selectedRole == "RSC Evaluator")
                {
                    return RedirectToAction("Index", "RSCEvaluator", new { area = "RscSys" });
                }
                
            }
            return RedirectToAction("Index", "Home"); // Redirect after setting role
        }

        // Action to get user roles for the dropdown
        public async Task<IActionResult> RoleSelection()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.UserRoles = roles;

            return View();
        }

        // Action to set the selected role in session
        // Action to set the selected role in session
        [HttpPost]
        public IActionResult SetActiveRole(string selectedRole)
        {
            if (!string.IsNullOrEmpty(selectedRole))
            {
                HttpContext.Session.SetString("ActiveRole", selectedRole);
            }
            return RedirectToAction("Index", "Home"); // Redirect to any desired page after setting the role
        }


        [HttpGet]
        public async Task<IActionResult> SearchUser(string term)
        {
            var users = _userManager.Users
                .Where(u => u.UserName.Contains(term) || u.FirstName.Contains(term) || u.LastName.Contains(term))
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    FullName = $"{u.FirstName} {u.LastName}",
                    u.Email
                })
                .Take(10)  // Limit results
                .ToList();

            return Json(users);
        }

        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }


    }
}