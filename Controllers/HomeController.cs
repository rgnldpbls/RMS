using Microsoft.AspNetCore.Mvc;
using ResearchManagementSystem.Models;
using System.Diagnostics;

namespace ResearchManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ArticlesEvents()
        {
            return View();
        }

        public IActionResult OrgStruct()
        {
            return View();
        }

        public IActionResult RMOSection()
        {
            return View();
        }

        public IActionResult RSSPage()
        {
            return View();
        }

        public IActionResult RLDSPage()
        {
            return View();
        }

        public IActionResult RESPage()
        {
            return View();
        }

        public IActionResult REMSPage()
        {
            return View();
        }

        public IActionResult RMOActivities()
        {
            return View();
        }

        public IActionResult RMOGuidelines()
        {
            return View();
        }

        public IActionResult EngagementColleges()
        {
            return View();
        }

        public IActionResult EngagementCampuses()
        {
            return View();
        }

        public IActionResult EngagementOUS()
        {
            return View();
        }

        public IActionResult EngagementInstitutes()
        {
            return View();
        }

        public IActionResult EngagementOverview()
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
