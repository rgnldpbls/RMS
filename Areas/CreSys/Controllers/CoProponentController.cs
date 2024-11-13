using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    [Area("CreSys")]
    public class CoProponentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
