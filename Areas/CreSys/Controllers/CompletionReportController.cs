using CRE.Interfaces;
using CRE.Models;
using CRE.Services;
using CRE.ViewModels;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRE.Controllers
{
    [Area("CreSys")]
    public class CompletionReportController : Controller
    {
        private readonly ICompletionReportServices _completionReportServices;
        private readonly IEthicsApplicationLogServices _ethicsApplicationLogServices;
        private readonly ICompletionCertificateServices _completionCertificateServices;

        // Only one constructor should exist
        public CompletionReportController(
            ICompletionReportServices completionReportServices,
            IEthicsApplicationLogServices ethicsApplicationLogServices,
            ICompletionCertificateServices completionCertificateServices)
        {
            _completionReportServices = completionReportServices;
            _ethicsApplicationLogServices = ethicsApplicationLogServices;
            _completionCertificateServices = completionCertificateServices;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ApplyForCompletionCertificate(string urecNo)
        {
            var model = await _completionReportServices.GetApplyForCompletionCertificateViewModelAsync(urecNo);

            if (model == null)
            {
                // Handle case where data is not found (e.g., return a NotFound result or show an error message)
                return NotFound();
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyForCompletionCertificate(string urecNo, IFormFile form18, DateOnly researchStartDate)
            {
            ModelState.Remove("CompletionReport.terminalReport");
            ModelState.Remove("CompletionReport.researchEndDate");
            ModelState.Remove("CompletionReport.EthicsApplication");
            ModelState.Remove("EthicsApplication.fieldOfStudy");

            if (ModelState.IsValid)
            {
                // Pass the model's properties and form18 (the file) to the service
                bool success = await _completionReportServices.SubmitTerminalReportAsync(
                    urecNo, // UREC number
                    form18, // The uploaded file (Form 18)
                    researchStartDate // Research start date
                );

                if (success)
                {
                    // Add a log entry for the successful submission of the terminal report
                    var log = new EthicsApplicationLog
                    {
                        urecNo = urecNo,
                        status = "Terminal Report Submitted",
                        changeDate = DateTime.UtcNow,
                        userId = User.FindFirstValue(ClaimTypes.NameIdentifier), // Get the logged-in user's ID
                        comments = "Terminal report submitted successfully."
                    };

                    // Save the log entry to the database
                    await _ethicsApplicationLogServices.AddLogAsync(log);

                    // Set the success message in TempData
                    TempData["SuccessMessage"] = "Terminal report submitted successfully.";

                    // Redirect to the target action with urecNo parameter
                    return RedirectToAction("UploadForms", "EthicsApplicationForms", new { urecNo = urecNo });
                }
                else
                {
                    // Add an error if submission fails
                    ModelState.AddModelError("", "There was an issue submitting the terminal report.");
                }
            }

            // If validation failed or submission failed, re-render the form
            TempData["ErrorMessage"] = "There was an error processing your request.";
            return RedirectToAction("UploadForms", "EthicsApplicationForms", new { urecNo = urecNo });
        }

        [Authorize]
        public async Task<IActionResult> DownloadCertificateFile(string urecNo)
        {
            // Retrieve the CompletionCertificate based on the urecNo
            var completionCertificate = await _completionCertificateServices
                .GetCompletionCertificateByUrecNoAsync(urecNo);

            if (completionCertificate == null || completionCertificate.file == null)
            {
                // If no certificate file is found, return a not found status
                return NotFound("Certificate file not found.");
            }

            // Return the file as a downloadable PDF (you can change the MIME type if needed)
            return File(completionCertificate.file, "application/pdf", $"{urecNo}_CompletionCertificate.pdf");
        }


    }
}
