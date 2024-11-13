using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    [Area("CreSys")]
    public class EthicsEvaluatorExpertiseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
