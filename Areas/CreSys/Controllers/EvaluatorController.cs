using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemcSys.Models;
using ResearchManagementSystem.Areas.CreSys.Data;
using ResearchManagementSystem.Areas.CreSys.Interfaces;
using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Areas.CreSys.ViewModels;
using ResearchManagementSystem.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace ResearchManagementSystem.Areas.CreSys.Controllers
{
    [Area("CreSys")]
    [Authorize]
    public class EvaluatorController : Controller
    {
        private readonly CreDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEthicsEmailService _emailService;
        private readonly IAllServices _allServices;
        public EvaluatorController(CreDbContext context, UserManager<ApplicationUser> userManager, IEthicsEmailService emailService, IAllServices allServices)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _allServices = allServices;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetStarted()
        {
            // Get the current user ID from the claims (assuming you're using Identity)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the user exists in the CRE_EthicsEvaluator table
            var evaluator = await _context.CRE_EthicsEvaluator
                .FirstOrDefaultAsync(e => e.UserID == userId);

            if (evaluator != null)
            {
                // If the evaluator exists, retrieve their expertise and preselect checkboxes
                var evaluatorExpertises = await _context.CRE_EthicsEvaluatorExpertise
                    .Where(ee => ee.EthicsEvaluatorId == evaluator.EthicsEvaluatorId)
                    .Select(ee => ee.ExpertiseId)
                    .ToListAsync();

                var expertiseList = await _context.CRE_Expertise.ToListAsync();

                var viewModel = new GetStartedViewModel
                {
                    ExpertiseList = expertiseList,
                    SelectedExpertise = evaluatorExpertises
                };

                return View(viewModel);
            }

            // If evaluator doesn't exist, show the list of expertise to choose from
            var expertiseOptions = await _context.CRE_Expertise.ToListAsync();

            var viewModelNew = new GetStartedViewModel
            {
                ExpertiseList = expertiseOptions,
                SelectedExpertise = new List<int>() // Empty for new users
            };

            return View(viewModelNew);
        }

        [HttpPost]
        public async Task<IActionResult> GetStarted(GetStartedViewModel model)
        {
            if (model.SelectedExpertise == null || !model.SelectedExpertise.Any())
            {
                ModelState.AddModelError("", "Please select at least one area of expertise.");
                return View(model);
            }

            // Get the current user ID from the claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["Message"] = "User not found!";
                return RedirectToAction("GetStarted");
            }

            // Concatenate the full name: FirstName + MiddleName (initial) + LastName
            var fullName = $"{user.FirstName} {user.MiddleName?.Substring(0, 1)}. {user.LastName}".Trim();
            // Check if the user is already an evaluator
            var existingEvaluator = await _context.CRE_EthicsEvaluator
                .FirstOrDefaultAsync(e => e.UserID == userId);

            if (existingEvaluator == null)
            {
                // Add a new evaluator record
                var evaluator = new EthicsEvaluator
                {
                    UserID = userId,
                    Name = fullName,
                    Completed = 0,
                    Pending = 0,
                    Declined = 0,
                    AccountStatus = "Active"
                };

                _context.CRE_EthicsEvaluator.Add(evaluator);
                await _context.SaveChangesAsync();

                // Add the selected expertise for this evaluator
                foreach (var expertiseId in model.SelectedExpertise)
                {
                    var evaluatorExpertise = new EthicsEvaluatorExpertise
                    {
                        EthicsEvaluatorId = evaluator.EthicsEvaluatorId,
                        ExpertiseId = expertiseId
                    };

                    _context.CRE_EthicsEvaluatorExpertise.Add(evaluatorExpertise);
                }
            }
            else
            {
                // Update existing evaluator's expertise
                var existingExpertise = await _context.CRE_EthicsEvaluatorExpertise
                    .Where(ee => ee.EthicsEvaluatorId == existingEvaluator.EthicsEvaluatorId)
                    .ToListAsync();

                // Remove the expertise that is not selected anymore
                _context.CRE_EthicsEvaluatorExpertise.RemoveRange(existingExpertise
                    .Where(ee => !model.SelectedExpertise.Contains(ee.ExpertiseId)));

                // Add new expertise if it's not already assigned
                foreach (var expertiseId in model.SelectedExpertise)
                {
                    if (!existingExpertise.Any(ee => ee.ExpertiseId == expertiseId))
                    {
                        var evaluatorExpertise = new EthicsEvaluatorExpertise
                        {
                            EthicsEvaluatorId = existingEvaluator.EthicsEvaluatorId,
                            ExpertiseId = expertiseId
                        };

                        _context.CRE_EthicsEvaluatorExpertise.Add(evaluatorExpertise);
                    }
                }
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = "Your expertise has been updated!";
            return RedirectToAction("Index", "Home", new { area = "CreSys" });
        }
        [HttpGet]
        public async Task<IActionResult> EvaluatorView()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var evaluator = await _context.CRE_EthicsEvaluator
                .FirstOrDefaultAsync(e => e.UserID == userId);
            if (evaluator == null)
            {
                // Store the message in TempData
                TempData["ErrorMessage"] = "Please press 'Get Started' to select your Expertise.";

                // Redirect to the same page (you can use the current URL or an action)
                var refererUrl = Request.Headers["Referer"].ToString();

                // If the Referer is not null or empty, redirect to that URL
                if (!string.IsNullOrEmpty(refererUrl))
                {
                    return Redirect(refererUrl);
                }
                else
                {
                    // If no referer is available, redirect to a default page
                    return RedirectToAction("Index", "Home"); // or any default page
                }
            }
            // Extract the evaluatorId from the evaluator object
            var evaluatorId = evaluator.EthicsEvaluatorId; // Adjust according to your model

            var viewModel = new TabbedEvaluationViewModel
            {
                EvaluationAssignments = await _allServices.GetAssignedEvaluationsAsync(evaluatorId),
                ToBeEvaluated = await _allServices.GetAcceptedEvaluationsAsync(evaluatorId),
                Evaluated = await _allServices.GetCompletedEvaluationsAsync(evaluatorId),
                DeclinedEvaluations = await _allServices.GetDeclinedEvaluationsAsync(evaluatorId)
            };

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> RespondToAssignment(string id, int evaluationId)
        {
            // Fetch the application details based on urecNo and evaluationId
            var viewModel = await _allServices.GetEvaluationDetailsWithUrecNoAsync(id, evaluationId);

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

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("User ID not found.");
            }

            // Check if the current user is assigned as an EthicsEvaluator
            var evaluator = await _context.CRE_EthicsEvaluator
                .FirstOrDefaultAsync(e => e.UserID == currentUserId);

            if (evaluator == null)
            {
                return NotFound("Evaluator not found for the current user.");
            }

            // Extract the evaluator ID from the first evaluator in the collection
            var ethicsEvaluatorId = evaluator.EthicsEvaluatorId;
            // Check if an evaluation already exists for this urecNo and evaluator
            var existingEvaluation = await _allServices.GetEvaluationByUrecNoAndEvaluatorIdAsync(urecNo, ethicsEvaluatorId);

            if (existingEvaluation != null)
            {
                if (acceptanceStatus == "Declined")
                {
                    // Mark application as "Unassigned"
                    await _allServices.UpdateApplicationStatusAsync(existingEvaluation.EvaluationId, urecNo, "Unassigned");
                    await _allServices.UpdateEvaluationStatusAsync(existingEvaluation.EvaluationId, "Declined", reasonForDecline, evaluator.UserID);

                    evaluator = await _context.CRE_EthicsEvaluator.FindAsync(ethicsEvaluatorId);
                    if (evaluator != null)
                    {
                        evaluator.Declined += 1;
                    }
                }
                else
                {
                    // Update evaluation status to "Accepted"
                    await _allServices.UpdateEvaluationStatusAsync(existingEvaluation.EvaluationId, "Accepted", null, evaluator.UserID);

                    // Find the evaluator in the database
                    evaluator = await _context.CRE_EthicsEvaluator.FindAsync(ethicsEvaluatorId);
                    if (evaluator != null)
                    {
                        evaluator.Pending += 1;
                    }

                    // Send email feature to notify the evaluator of the 10-day evaluation deadline
                    var evaluatorUser = await _userManager.Users
                        .FirstOrDefaultAsync(u => u.Id == evaluator.UserID);

                    if (evaluatorUser != null && !string.IsNullOrEmpty(evaluatorUser.Email))
                    {
                        string evaluatorName = evaluator.Name; // Replace with the correct name property
                        string evaluatorEmail = evaluatorUser.Email;
                        string subject = "Ethics Application Assignment Accepted";
                        string body = $@"
                    <p>You have accepted the assignment for the Ethics Application with UREC No: <strong>{existingEvaluation.UrecNo}</strong>.</p>
                    <p>Please note that you have <strong>10 days</strong> from now to complete the evaluation.</p>
                    <p>We appreciate your timely response in reviewing the application.</p>";

                        // Send the email notification
                        await _emailService.SendEmailAsync(evaluatorEmail, evaluatorName, subject, body);
                    }

                    // Add notification for the evaluator
                    var notification = new EthicsNotifications
                    {
                        UrecNo = existingEvaluation.UrecNo, // Ensure this is the correct UREC No
                        UserId = evaluator.UserID, // Use the evaluator's UserId
                        NotificationTitle = "New Ethics Application Assigned",
                        NotificationMessage = $"A new Ethics Application with UREC No: {existingEvaluation.UrecNo} has been assigned to you. You have 10 days to complete the evaluation.",
                        NotificationCreationDate = DateTime.Now,
                        NotificationStatus = false, // Unread
                        Role = "Evaluator", // Ensure this is the correct role for the evaluator
                        PerformedBy = "System" // Can be updated to the actual logged-in user if needed
                    };

                    // Add notification to the context
                    _context.CRE_EthicsNotifications.Add(notification);
                }
            }
            else
            {
                // Create a new evaluation entry if none exists
                var newEvaluation = new EthicsEvaluation
                {
                    EvaluationStatus = acceptanceStatus,
                    StartDate = DateTime.UtcNow,
                    EthicsEvaluatorId = ethicsEvaluatorId,
                    Name = evaluator.Name,
                    UrecNo = urecNo
                };

                // Add the new evaluation to the context
                await _context.CRE_EthicsEvaluation.AddAsync(newEvaluation);

                if (acceptanceStatus == "Declined")
                {
                    // Mark application as "Unassigned"
                    await _allServices.UpdateApplicationStatusAsync(evalId, urecNo, "Unassigned");
                    await _allServices.UpdateEvaluationStatusAsync(evalId, "Declined", reasonForDecline, evaluator.UserID);

                    evaluator = await _context.CRE_EthicsEvaluator.FindAsync(ethicsEvaluatorId);
                    if (evaluator != null)
                    {
                        evaluator.Declined += 1;
                    }
                }
            }

            // Save all changes in one go after all the necessary updates
            await _context.SaveChangesAsync();

            // Fetch application details for the view model (to show after response)
            var applicationDetails = await _allServices.GetApplicationDetailsAsync(urecNo);

            // Create and populate the view model with application details
            var viewModel = new EvaluationDetailsViewModel
            {
                NonFundedResearchInfo = applicationDetails.NonFundedResearchInfo,
                CoProponent = applicationDetails.CoProponent,
                ReceiptInfo = applicationDetails.ReceiptInfo,
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

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("User ID not found.");
            }

            // Check if the current user is assigned as an EthicsEvaluator
            var evaluator = await _context.CRE_EthicsEvaluator
                .FirstOrDefaultAsync(e => e.UserID == currentUserId);

            if (evaluator == null)
            {
                return NotFound("Evaluator not found for the current user.");
            }

            // Extract the evaluator ID from the first evaluator in the collection
            var ethicsEvaluatorId = evaluator.EthicsEvaluatorId;

            // Retrieve application details for the given application ID
            var applicationDetails = await _allServices.GetApplicationDetailsAsync(id);

            // Check if application details were found
            if (applicationDetails == null)
            {
                return NotFound(); // Return a 404 if not found
            }

            // Retrieve the specific EthicsEvaluation for the current evaluator and application
            var currentEvaluation = applicationDetails.EthicsEvaluation?
                .FirstOrDefault(e => e.EthicsEvaluatorId == ethicsEvaluatorId);

            // Create the view model with the details
            var viewModel = new EvaluationDetailsViewModel
            {
                NonFundedResearchInfo = applicationDetails.NonFundedResearchInfo,
                CoProponent = applicationDetails.CoProponent,
                ReceiptInfo = applicationDetails.ReceiptInfo,
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

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("User ID not found.");
            }

            // Check if the current user is assigned as an EthicsEvaluator
            var evaluator = await _context.CRE_EthicsEvaluator
                .FirstOrDefaultAsync(e => e.UserID == currentUserId);

            if (evaluator == null)
            {
                return NotFound("Evaluator not found for the current user.");
            }

            // Extract the evaluator ID from the first evaluator in the collection
            var ethicsEvaluatorId = evaluator.EthicsEvaluatorId;
            var selectedEvaluation = model.CurrentEvaluation;

            if (selectedEvaluation == null)
            {
                // Log or handle the case where the evaluation is not found
                return NotFound("Current evaluation not found.");
            }

            // Get the evaluator's full name using UserManager
            var evaluatorUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == evaluator.UserID);

            if (evaluatorUser != null)
            {
                // Construct the evaluator's full name
                string evaluatorFullName = $"{evaluatorUser.FirstName} {evaluatorUser.MiddleName?.FirstOrDefault()}. {evaluatorUser.LastName}";

                // Assign the full name to the evaluation
                selectedEvaluation.Name = evaluatorFullName;  // Ensure this column is not null
            }

            // Assign other values to the selected evaluation
            selectedEvaluation.EthicsEvaluatorId = ethicsEvaluatorId;
            selectedEvaluation.ProtocolRecommendation = model.CurrentEvaluation.ProtocolRecommendation;
            selectedEvaluation.ProtocolRemarks = model.CurrentEvaluation.ProtocolRemarks;
            selectedEvaluation.ConsentRecommendation = model.CurrentEvaluation.ConsentRecommendation;
            selectedEvaluation.ConsentRemarks = model.CurrentEvaluation.ConsentRemarks;
            selectedEvaluation.EndDate = DateTime.Today;

            // Process uploaded files
            if (model.ProtocolReviewSheet != null)
            {
                selectedEvaluation.ProtocolReviewSheet = await GetFileContentAsync(model.ProtocolReviewSheet);
            }
            if (model.InformedConsentForm != null)
            {
                selectedEvaluation.InformedConsentForm = await GetFileContentAsync(model.InformedConsentForm);
            }

            // Declare a flag to track changes
            bool hasChanges = false;

            // Add or update the evaluation
            if (model.CurrentEvaluation == null)
            {
                _context.CRE_EthicsEvaluation.Add(selectedEvaluation);
                hasChanges = true;
            }
            else
            {
                // Mark the evaluation status as "Evaluated"
                selectedEvaluation.EvaluationStatus = "Evaluated";
                _context.CRE_EthicsEvaluation.Update(selectedEvaluation);

                // Update the evaluator's status
                evaluator.Pending -= 1;
                evaluator.Completed += 1;

                hasChanges = true;
            }

            // Send email to evaluator for completing the evaluation
            if (evaluatorUser != null && !string.IsNullOrEmpty(evaluatorUser.Email))
            {
                string subject = "Ethics Evaluation Completed";
                string body = $@"
        <p>Thank you for completing the evaluation for the Ethics Application with UREC No: <strong>{selectedEvaluation.UrecNo}</strong>.</p>
        <p>The evaluation status has been successfully marked as <strong>Evaluated</strong>.</p>";

                // Send the email notification
                await _emailService.SendEmailAsync(evaluatorUser.Email, evaluator.Name, subject, body);
            }

            // Add notification for the evaluator
            var notification = new EthicsNotifications
            {
                UrecNo = selectedEvaluation.UrecNo, // Ensure this is the correct UREC No
                UserId = evaluator.UserID, // Use the evaluator's UserId
                NotificationTitle = "Evaluation Completed",
                NotificationMessage = $"You have successfully completed the evaluation for the Ethics Application with UREC No: {selectedEvaluation.UrecNo}. The evaluation status is now marked as 'Evaluated'.",
                NotificationCreationDate = DateTime.Now,
                NotificationStatus = false, // Unread
                Role = "Evaluator", // Ensure this is the correct role for the evaluator
                PerformedBy = "System" // Can be updated to the actual logged-in user if needed
            };

            _context.CRE_EthicsNotifications.Add(notification);
            hasChanges = true;

            // Check for all evaluations completed
            if (await _allServices.AreAllEvaluationsEvaluatedAsync(model.EthicsApplication.UrecNo))
            {
                var applicationLog = new EthicsApplicationLogs
                {
                    UrecNo = model.EthicsApplication.UrecNo,
                    UserId = currentUserId,
                    Status = "Application Evaluated",
                    ChangeDate = DateTime.Now,
                    Comments = "The application has been evaluated and marked as submitted."
                };
                _context.CRE_EthicsApplicationLogs.Add(applicationLog); // Add the log to the DbSet
                hasChanges = true;
            }

            // Save all changes in one go
            if (hasChanges)
            {
                await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Evaluated(string urecNo, int evaluationId)
        {
            var evaluatedApplication = await _allServices.GetEvaluationAndEvaluatorDetailsAsync(urecNo, evaluationId);
            if (evaluatedApplication == null)
            {
                return NotFound();
            }

            // Map the evaluatedApplication to EvaluationDetailsViewModel with null checks
            var evaluationDetailsViewModel = new EvaluationDetailsViewModel
            {
                EthicsApplication = evaluatedApplication.EthicsApplication,
                NonFundedResearchInfo = evaluatedApplication.NonFundedResearchInfo,
                EthicsApplicationLog = evaluatedApplication.EthicsApplicationLog ?? new List<EthicsApplicationLogs>(),
                EthicsEvaluation = evaluatedApplication.EthicsEvaluation,
                InitialReview = evaluatedApplication.InitialReview,
                EthicsEvaluator = evaluatedApplication.EthicsEvaluator ?? new EthicsEvaluator()
            };

            return View(evaluationDetailsViewModel);
        }
    }
}
