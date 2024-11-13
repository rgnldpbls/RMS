using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    [Area("CreSys")]
    public class NonFundedResearchInfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
