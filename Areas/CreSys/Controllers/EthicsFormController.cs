using CRE.Interfaces;
using CRE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRE.Controllers
{
    [Area("CreSys")]
    public class EthicsFormController : Controller
    {
        private readonly IEthicsFormServices _ethicsForm;
        public EthicsFormController(IEthicsFormServices ethicsForm)
        {
            _ethicsForm = ethicsForm;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> DownloadForms()
        {
            var forms = await _ethicsForm.GetAllFormsAsync(); // Fetch all forms
            return View(forms); // Pass the forms to the view
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DownloadForm(string id)
        {
            var form = await _ethicsForm.GetEthicsFormByIdAsync(id);
            if (form == null || form.file == null)
            {
                return NotFound();
            }

            return File(form.file, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", form.formName + ".docx");
        }
    }
}
