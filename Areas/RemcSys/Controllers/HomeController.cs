using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResearchManagementSystem.Areas.RemcSys.Data;
using RemcSys.Models;
using System.Diagnostics;
using ResearchManagementSystem.Models;

namespace RemcSys.Controllers
{
    [Area("RemcSys")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RemcDBContext _context;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager, 
            RemcDBContext context)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index() // Index Page
        {
            if (User.IsInRole("REMC Chief"))
            {
                return RedirectToAction("Chief");
            }
            else if (User.IsInRole("REMC Evaluator"))
            {
                return RedirectToAction("Evaluator");
            }
            else if (User.IsInRole("Faculty"))
            {
                return RedirectToAction("Faculty");
            }
            return RedirectToAction("AccessDenied");
        }

        public IActionResult Privacy() // Privacy Page
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new RemcSys.Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDenied() // Access Denied Page
        {
            return View();
        }

        public IActionResult UnderMaintenance() // UnderMaintenance Page
        {
            return View();
        }

        public IActionResult UFRAppClosed() // UniversityFundedResearch Application closed Page
        {
            return View();
        }

        public IActionResult EFRAppClosed() // ExternallyFundedResearch Application closed Page
        {
            return View();
        }

        public IActionResult UFRLAppClosed() // UniversityFundedResearchLoad Application closed Page
        {
            return View();
        }

        [Authorize(Roles = "Faculty")]
        public IActionResult Faculty() // Faculty HomePage
        {
            return View();
        }

        [Authorize(Roles = "Faculty")]
        public IActionResult Forms() // Memorandums
        {
            var guidelines = _context.Guidelines.Where(g => g.document_Type == "Memorandum")
                .OrderBy(g => g.file_Name).ToList();

            return View(guidelines);
        }

        public IActionResult PreviewFile(string id) // Preview PDF Files
        {
            var guidelines = _context.Guidelines.FirstOrDefault(g => g.Id == id);
            if (guidelines != null)
            {
                if (guidelines.file_Type == ".pdf")
                {
                    return File(guidelines.data, "application/pdf");
                }
            }

            return BadRequest("Only PDF files can be previewed.");
        }

        [Authorize(Roles = "Faculty")]
        public IActionResult FRType() // Select Funded Research Type
        {
           return View();
        }

        [Authorize(Roles = "Faculty")]
        public IActionResult Eligibility(string type) // Eligibility based on Funded Research Type
        {
            var isUFRApp = _context.Settings.First().isUFRApplication;
            if(!isUFRApp && type == "University Funded Research")
            {
                return RedirectToAction("UFRAppClosed", "Home");
            }

            var isEFRApp = _context.Settings.First().isEFRApplication;
            if(!isEFRApp && type == "Externally Funded Research")
            {
                return RedirectToAction("EFRAppClosed", "Home");
            }

            var isUFRLApp = _context.Settings.First().isUFRLApplication;
            if(!isUFRLApp && type == "University Funded Research Load")
            {
                return RedirectToAction("UFRLAppClosed", "Home");

            }

            ViewBag.Type = type;
            return View();
        }
        
        [Authorize(Roles = "REMC Evaluator")]
        public async Task<IActionResult> Evaluator() // Evaluator HomePage
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var haveEvaluator = await _context.Evaluator.AnyAsync(e => e.UserId == user.Id);
            if (!haveEvaluator)
            {
                var evaluator = new Evaluator
                {
                    evaluator_Name = user.FirstName + " " + user.LastName,
                    evaluator_Email = user.Email,
                    field_of_Interest = ["Computer Science and Information System Technology"],
                    UserId = user.Id,
                    UserType = null,
                    center = ["REMC"]
                };
                _context.Evaluator.Add(evaluator);
                _context.SaveChanges();
                return View();
            }
            return View();
        }

        [Authorize(Roles = "REMC Chief")]
        public IActionResult Chief() // Chief HomePage
        {
            return View();
        }
    }
}
