using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    [Area("CreSys")]
    public class ExpertiseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
