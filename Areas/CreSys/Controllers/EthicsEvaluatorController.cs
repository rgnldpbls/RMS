using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using CRE.Services;
using CRE.ViewModels;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResearchManagementSystem.Models;
using System.Diagnostics;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CRE.Controllers
{
    [Area("CreSys")]
    public class EthicsEvaluatorController : Controller
    {
        private readonly IEthicsEvaluationServices _ethicsEvaluationServices;
        private readonly IInitialReviewServices _initialReviewServices;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEthicsApplicationLogServices _ethicsApplicationLogServices;
        private readonly IEthicsApplicationServices _ethicsApplicationServices;
        private readonly CreDbContext _context;
        public EthicsEvaluatorController(IEthicsEvaluationServices ethicsEvaluationServices, IInitialReviewServices initialReviewServices,
            UserManager<ApplicationUser> userManager, IEthicsApplicationLogServices ethicsApplicationLogServices, IEthicsApplicationServices ethicsApplicationServices,
            CreDbContext context)
        {
            _ethicsEvaluationServices = ethicsEvaluationServices;
            _initialReviewServices = initialReviewServices;
            _userManager = userManager;
            _ethicsApplicationLogServices = ethicsApplicationLogServices;
            _ethicsApplicationServices = ethicsApplicationServices;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EvaluatorView()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var evaluator = await _ethicsEvaluationServices.GetEvaluatorByUserIdAsync(userId);
            if (evaluator == null)
            {
                // Handle the case when the evaluator is not found
                return NotFound("Evaluator not found");
            }
            // Extract the evaluatorId from the evaluator object
            var evaluatorId = evaluator.ethicsEvaluatorId; // Adjust according to your model

            var viewModel = new TabbedEvaluationViewModel
            {
                EvaluationAssignments = await _ethicsEvaluationServices.GetAssignedEvaluationsAsync(evaluatorId),
                ToBeEvaluated = await _ethicsEvaluationServices.GetAcceptedEvaluationsAsync(evaluatorId),
                Evaluated = await _ethicsEvaluationServices.GetCompletedEvaluationsAsync(evaluatorId),
                DeclinedEvaluations = await _ethicsEvaluationServices.GetDeclinedEvaluationsAsync(evaluatorId)
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RespondToAssignment(string id, int evaluationId)
        {
            // Fetch the application details based on urecNo and evaluationId
            var viewModel = await _ethicsEvaluationServices.GetEvaluationDetailsWithUrecNoAsync(id, evaluationId);

            // Check if evaluation details were found
            if (viewModel == null)
            {
                return NotFound(); // Return a 404 if not found
            }

            // Pass the view model to the view
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> RespondToAssignment(string acceptanceStatus, string urecNo, int evalId, string? reasonForDecline)
        {
            // Retrieve the current user ID
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ethicsEvaluator = _context.EthicsEvaluator
                .Include(e => e.Faculty)
                .First(e => e.Faculty.userId == currentUserId);

            // Check if an evaluation already exists for this urecNo and evaluator
            var existingEvaluation = await _ethicsEvaluationServices.GetEvaluationByUrecNoAndEvaluatorIdAsync(urecNo, ethicsEvaluator.ethicsEvaluatorId);

            if (existingEvaluation != null)
            {
                if (acceptanceStatus == "Declined")
                {
                    // Mark application as "Unassigned"
                    await _ethicsApplicationServices.UpdateApplicationStatusAsync(existingEvaluation.evaluationId, urecNo, "Unassigned");
                    await _ethicsEvaluationServices.UpdateEvaluationStatusAsync(existingEvaluation.evaluationId, "Declined", reasonForDecline, ethicsEvaluator.ethicsEvaluatorId);

                    await _ethicsEvaluationServices.IncrementDeclinedAssignmentCountAsync(ethicsEvaluator.ethicsEvaluatorId);

                }
                else
                {
                    // Update the existing evaluation's status for accepted applications
                    await _ethicsEvaluationServices.UpdateEvaluationStatusAsync(existingEvaluation.evaluationId, "Accepted", null, ethicsEvaluator.ethicsEvaluatorId);
                }
            }
            else
            {
                // Create a new evaluation entry if none exists
                var newEvaluation = new EthicsEvaluation
                {
                    evaluationStatus = acceptanceStatus,
                    startDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    ethicsEvaluatorId = ethicsEvaluator.ethicsEvaluatorId,
                    urecNo = urecNo,
                    reasonForDecline = acceptanceStatus == "Declined" ? reasonForDecline : null // Set reason for decline if applicable
                };

                await _ethicsEvaluationServices.CreateEvaluationAsync(newEvaluation);

                // If declined, update application status to "Unassigned"
                if (acceptanceStatus == "Declined")
                {
                    await _ethicsApplicationServices.UpdateApplicationStatusAsync(newEvaluation.evaluationId, urecNo, "Unassigned"); // Ensure application is marked unassigned
                }
            }

            // Fetch application details for the view model (to show after response)
            var applicationDetails = await _initialReviewServices.GetApplicationDetailsAsync(urecNo);

            // Create and populate the view model with application details
            var viewModel = new EvaluationDetailsViewModel
            {
                Secretariat = applicationDetails.Secretariat,
                NonFundedResearchInfo = applicationDetails.NonFundedResearchInfo,
                CoProponent = applicationDetails.CoProponent,
                ReceiptInfo = applicationDetails.ReceiptInfo,
                Chairperson = applicationDetails.Chairperson,
                EthicsEvaluator = applicationDetails.EthicsEvaluator,
                EthicsApplication = applicationDetails.EthicsApplication,
                InitialReview = applicationDetails.InitialReview,
                EthicsApplicationForms = applicationDetails.EthicsApplicationForms,
                EthicsApplicationLog = applicationDetails.EthicsApplicationLog,
            };

            // Add a success message to TempData
            TempData["SuccessMessage"] = "Respond to evaluation successfully.";

            // Redirect to the EvaluatorView
            return RedirectToAction("EvaluatorView");
        }


        public async Task<IActionResult> EvaluationDetails(string id)
        {
            // Retrieve the current user's ID from the claims
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get the current user along with their associated EthicsEvaluator through Faculty
            var currentUser = await _context.EthicsEvaluator
                .Include(u => u.Faculty)
                .FirstOrDefaultAsync(u => u.Faculty.userId == currentUserId);

            if (currentUser?.Faculty.EthicsEvaluator == null)
            {
                return NotFound("Evaluator not found for the current user.");
            }

            var ethicsEvaluatorId = currentUser.Faculty.EthicsEvaluator.ethicsEvaluatorId;

            // Retrieve application details for the given application ID
            var applicationDetails = await _initialReviewServices.GetApplicationDetailsAsync(id);

            // Check if application details were found
            if (applicationDetails == null)
            {
                return NotFound(); // Return a 404 if not found
            }

            // Retrieve the specific EthicsEvaluation for the current evaluator and application
            var currentEvaluation = applicationDetails.EthicsEvaluation?
                .FirstOrDefault(e => e.ethicsEvaluatorId == ethicsEvaluatorId);

            // Create the view model with the details
            var viewModel = new EvaluationDetailsViewModel
            {
                Secretariat = applicationDetails.Secretariat,
                NonFundedResearchInfo = applicationDetails.NonFundedResearchInfo,
                CoProponent = applicationDetails.CoProponent,
                ReceiptInfo = applicationDetails.ReceiptInfo,
                Chairperson = applicationDetails.Chairperson,
                EthicsEvaluator = applicationDetails.EthicsEvaluator,
                EthicsApplication = applicationDetails.EthicsApplication,
                InitialReview = applicationDetails.InitialReview,
                EthicsApplicationForms = applicationDetails.EthicsApplicationForms,
                EthicsApplicationLog = applicationDetails.EthicsApplicationLog,
                EthicsEvaluation = applicationDetails.EthicsEvaluation,
                CurrentEvaluation = currentEvaluation, // Assign the specific evaluation for this evaluator
            };

            // Pass the details to the view
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> EvaluationDetails(EvaluationDetailsViewModel model)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _context.EthicsEvaluator
                .Include(u => u.Faculty)
                .FirstOrDefaultAsync(u => u.Faculty.userId == currentUserId);

            if (currentUser?.Faculty.EthicsEvaluator == null)
            {
                return NotFound("Evaluator not found for the current user.");
            }

            var ethicsEvaluatorId = currentUser.Faculty.EthicsEvaluator.ethicsEvaluatorId;
            var selectedEvaluation = model.CurrentEvaluation;

            if (selectedEvaluation == null)
            {
                // Log or handle the case where the evaluation is not found
                return NotFound("Current evaluation not found.");
            }

            // Log model data for debugging
            Debug.WriteLine($"StartDate: {selectedEvaluation.startDate}, UrecNo: {selectedEvaluation.urecNo}");

            selectedEvaluation.ethicsEvaluatorId = ethicsEvaluatorId;

            // Assign other values
            selectedEvaluation.ProtocolRecommendation = model.CurrentEvaluation.ProtocolRecommendation;
            selectedEvaluation.ProtocolRemarks = model.CurrentEvaluation.ProtocolRemarks;
            selectedEvaluation.ConsentRecommendation = model.CurrentEvaluation.ConsentRecommendation;
            selectedEvaluation.ConsentRemarks = model.CurrentEvaluation.ConsentRemarks;
            selectedEvaluation.endDate = DateOnly.FromDateTime(DateTime.Today);

            // Process uploaded files
            if (model.ProtocolReviewSheet != null)
            {
                selectedEvaluation.ProtocolReviewSheet = await GetFileContentAsync(model.ProtocolReviewSheet);
            }
            if (model.InformedConsentForm != null)
            {
                selectedEvaluation.InformedConsentForm = await GetFileContentAsync(model.InformedConsentForm);
            }

            // Save or update evaluation
            if (model.CurrentEvaluation == null)
            {
                await _ethicsEvaluationServices.AddEvaluationAsync(selectedEvaluation);
            }
            else
            {
                selectedEvaluation.evaluationStatus = "Evaluated";
                await _ethicsEvaluationServices.UpdateEvaluationAsync(selectedEvaluation);
            }

            // Check for all evaluations completed
            if (await _ethicsEvaluationServices.AreAllEvaluationsEvaluatedAsync(model.EthicsApplication.urecNo))
            {
                var applicationLog = new EthicsApplicationLog
                {
                    urecNo = model.EthicsApplication.urecNo,
                    userId = currentUserId,
                    status = "Application Evaluated",
                    changeDate = DateTime.Now,
                    comments = "The application has been evaluated and marked as submitted."
                };
                await _ethicsApplicationLogServices.AddLogAsync(applicationLog);
            }

            TempData["SuccessMessage"] = "Evaluation successful.";
            return RedirectToAction("EvaluatorView", new { success = true });
        }




        private async Task<byte[]> GetFileContentAsync(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        [Authorize]
        public async Task<IActionResult> Evaluated(string urecNo, int evaluationId)
        {
            var evaluatedApplication = await _ethicsEvaluationServices.GetEvaluationAndEvaluatorDetailsAsync(urecNo, evaluationId);
            if (evaluatedApplication == null)
            {
                return NotFound();
            }

            // Map the evaluatedApplication to EvaluationDetailsViewModel with null checks
            var evaluationDetailsViewModel = new EvaluationDetailsViewModel
            {
                EthicsApplication = evaluatedApplication.EthicsApplication,
                NonFundedResearchInfo = evaluatedApplication.NonFundedResearchInfo,
                EthicsApplicationLog = evaluatedApplication.EthicsApplicationLog ?? new List<EthicsApplicationLog>(),
                EthicsEvaluation = evaluatedApplication.EthicsEvaluation,
                InitialReview = evaluatedApplication.InitialReview,
                EthicsEvaluator = evaluatedApplication.EthicsEvaluator ?? new EthicsEvaluator(), // Provide default instance if null
            };

            return View(evaluationDetailsViewModel);
        }

    }
}
