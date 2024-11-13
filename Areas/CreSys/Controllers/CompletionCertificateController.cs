using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    [Area("CreSys")]
    public class CompletionCertificateController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
