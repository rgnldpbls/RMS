using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rscSys_final.Models;
using System.Diagnostics;

namespace rscSys_final.Controllers
{
    [Area("RscSys")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if(User.IsInRole("RSC Chief"))
            {
                return RedirectToAction("Index", "RSCChief", new { area = "RscSys" });
            }
            else if(User.IsInRole("RSC Evaluator"))
            {
                return RedirectToAction("Index", "RSCEvaluator", new { area = "RscSys" });
            }
            else
            {
                return RedirectToAction("Index", "RSCResearcher", new { area = "RscSys" });
            }
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
