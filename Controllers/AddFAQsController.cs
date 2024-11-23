using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ResearchManagementSystem.Data;
using ResearchManagementSystem.Models;
using ResearchManagementSystem.Services;

namespace ResearchManagementSystem.Controllers
{
    public class AddFAQsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserService _userService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;


        public AddFAQsController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, UserService userService, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userService = userService;
            _signInManager = signInManager; // Initialize SignInManager
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            // Get the roles of the current user
            var roles = await _userManager.GetRolesAsync(user);

            // Conditionally set the layout based on the user's role
            string layout;
            if (roles.Contains("SuperAdmin"))
            {
                layout = "SuperAdmin_Layout";
                ViewData["UserRole"] = "SuperAdmin";
            }
            else if (roles.Contains("Faculty"))
            {
                layout = "Faculty_Layout";
                ViewData["UserRole"] = "Faculty";
            }
            else if (roles.Contains("Student"))
            {
                layout = "Student_Layout";
                ViewData["UserRole"] = "Student";
            }
            else if (roles.Contains("RMCC"))
            {
                layout = "RMCC_Layout";
                ViewData["UserRole"] = "RMCC";
            }
            else
            {
                layout = "Director_Layout";
                ViewData["UserRole"] = "Director";
            }

            ViewData["Layout"] = layout;

            // Return the view with the appropriate layout
            return View(await _context.FAQs.ToListAsync());
        }
        // GET: AddFAQs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faq = await _context.FAQs
                .Include(f => f.superadmin)
                .FirstOrDefaultAsync(m => m.FAQ_id == id);
            if (faq == null)
            {
                return NotFound();
            }

            return View(faq);
        }

        // GET: AddFAQs/Create
        //[Authorize(Roles = "SuperAdmin")]
        public IActionResult Create()
        {
            ViewData["added_by"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: AddFAQs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create([Bind("question_id, answer_id")] AddFAQs addFAQs)
        {
            addFAQs.FAQ_id = Guid.NewGuid().ToString();
            addFAQs.added_by = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _context.Add(addFAQs);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: AddFAQs/Edit/5
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addFAQs = await _context.FAQs.FindAsync(id);
            if (addFAQs == null)
            {
                return NotFound();
            }
            ViewData["added_by"] = new SelectList(_context.Users, "Id", "Id", addFAQs.added_by);
            return View(addFAQs);
        }

        // POST: AddFAQs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Edit(string id, [Bind("FAQ_id,added_by,question_id,answer_id")] AddFAQs addFAQs)
        {
            if (id != addFAQs.FAQ_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(addFAQs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddFAQsExists(addFAQs.FAQ_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["added_by"] = new SelectList(_context.Users, "Id", "Id", addFAQs.added_by);
            return View(addFAQs);
        }

        // GET: AddFAQs/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addFAQs = await _context.FAQs
                .Include(a => a.superadmin)
                .FirstOrDefaultAsync(m => m.FAQ_id == id);
            if (addFAQs == null)
            {
                return NotFound();
            }

            return View(addFAQs);
        }

        // POST: AddFAQs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var addFAQs = await _context.FAQs.FindAsync(id);
            if (addFAQs != null)
            {
                _context.FAQs.Remove(addFAQs);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AddFAQsExists(string id)
        {
            return _context.FAQs.Any(e => e.FAQ_id == id);
        }
    }
}
