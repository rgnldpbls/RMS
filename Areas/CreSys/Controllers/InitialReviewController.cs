using CRE.Interfaces;
using CRE.Services;
using CRE.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRE.Controllers
{
    [Area("CreSys")]
    public class InitialReviewController : Controller
    {
        private readonly IInitialReviewServices _initialReviewServices;

        public InitialReviewController(IInitialReviewServices initialReviewServices)
        {
            _initialReviewServices = initialReviewServices;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Secretariat")]
        [HttpGet]
        public async Task<IActionResult> InitialReview()
        {
            var viewModel = new InitialReviewListViewModel
            {
                PendingApplications = await _initialReviewServices.GetPendingApplicationsAsync(),
                ApprovedApplications = await _initialReviewServices.GetApprovedApplicationsAsync(),
                ReturnedApplications = await _initialReviewServices.GetReturnedApplicationsAsync(),
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Secretariat")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveApplication(string urecNo, string comments)
        {
            if (string.IsNullOrEmpty(urecNo))
            {
                return BadRequest("Invalid UrecNo.");
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _initialReviewServices.ApproveApplicationAsync(urecNo, comments, userId);

            return RedirectToAction("InitialReview");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnApplication(string urecNo, string comments)
        {
            if (string.IsNullOrEmpty(urecNo))
            {
                return BadRequest("Invalid UrecNo.");
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _initialReviewServices.ReturnApplicationAsync(urecNo, comments, userId);

            return RedirectToAction("InitialReview");
        }

        [Authorize(Roles = "Secretariat")]
        [HttpGet]
        public async Task<IActionResult> Details(string urecNo)
        {
            if (string.IsNullOrEmpty(urecNo))
            {
                return NotFound();
            }

            try
            {
                // Fetch application details from the service layer
                var applicationDetails = await _initialReviewServices.GetApplicationDetailsAsync(urecNo);
                return View(applicationDetails);
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., application not found)
                return NotFound(ex.Message);
            }
        }


    }
}
