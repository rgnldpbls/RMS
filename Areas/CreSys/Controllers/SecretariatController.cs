using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    [Area("CreSys")]
    public class SecretariatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
