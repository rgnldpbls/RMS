using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using CRE.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResearchManagementSystem.Models;
using System.Security.Claims;
namespace CRE.Controllers
{
    [Area("CreSys")]
    public class ChiefController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IEthicsApplicationServices _ethicsApplicationServices;
        private readonly INonFundedResearchInfoServices _nonFundedResearchInfoServices;
        private readonly IReceiptInfoServices _receiptInfoServices;
        private readonly IEthicsApplicationLogServices _ethicsApplicationLogServices;
        private readonly ICoProponentServices _coProponentServices;
        private readonly IEthicsApplicationFormsServices _ethicsApplicationFormsServices;
        private readonly IInitialReviewServices _initialReviewServices;
        private readonly IEthicsEvaluationServices _ethicsEvaluationServices;
        private readonly IEthicsClearanceServices _ethicsClearanceServices;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICompletionCertificateServices _completionCertificateServices;
        private readonly ICompletionReportServices _completionReportServices;
        private readonly CreDbContext _context;

        public ChiefController(
            IConfiguration configuration,
            IEthicsApplicationServices ethicsApplicationServices,
            INonFundedResearchInfoServices nonFundedResearchInfoServices,
            IReceiptInfoServices receiptInfoServices,
            IEthicsApplicationLogServices ethicsApplicationLogServices,
            ICoProponentServices coProponentServices,
            IEthicsApplicationFormsServices ethicsApplicationFormsServices,
            IInitialReviewServices initialReviewServices,
            IEthicsEvaluationServices ethicsEvaluationServices,
            UserManager<ApplicationUser> userManager,
            IEthicsClearanceServices ethicsClearanceServices,
            ICompletionCertificateServices completionCertificateServices,
            ICompletionReportServices completionReportServices,
            CreDbContext context)
        {
            _configuration = configuration;
            _ethicsApplicationServices = ethicsApplicationServices;
            _nonFundedResearchInfoServices = nonFundedResearchInfoServices;
            _receiptInfoServices = receiptInfoServices;
            _ethicsApplicationLogServices = ethicsApplicationLogServices;
            _coProponentServices = coProponentServices;
            _ethicsApplicationFormsServices = ethicsApplicationFormsServices;
            _initialReviewServices = initialReviewServices;
            _ethicsEvaluationServices = ethicsEvaluationServices;
            _userManager = userManager;
            _context = context;
            _ethicsClearanceServices = ethicsClearanceServices;
            _completionCertificateServices = completionCertificateServices;
            _completionReportServices = completionReportServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles ="CRE Chief")]
        public async Task<IActionResult> Details(string urecNo)
        {
            var applicationDetails = await _initialReviewServices.GetApplicationDetailsAsync(urecNo);
            if (applicationDetails == null)
            {
                return NotFound();
            }
            var viewModel = new AssignReviewTypeViewModel
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
                EthicsApplicationLog = applicationDetails.EthicsApplicationLog
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReviewType(string reviewType, string urecNo)
        {
            if (string.IsNullOrEmpty(reviewType) || string.IsNullOrEmpty(urecNo))
            {
                return BadRequest("Required fields (UREC No. and Review Type) are missing.");
            }

            var initialReview = await _initialReviewServices.GetInitialReviewByUrecNoAsync(urecNo);

            if (initialReview != null)
            {
                initialReview.ReviewType = reviewType;
                await _initialReviewServices.UpdateInitialReviewAsync(initialReview);

                var logEntry = new EthicsApplicationLog
                {
                    urecNo = initialReview.urecNo,
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    changeDate = DateTime.Now,
                    status = "Review Type Assigned"
                };

                await _ethicsApplicationLogServices.AddLogAsync(logEntry);
            }

            return RedirectToAction("FilteredApplications");
        }



        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public async Task<IActionResult> EvaluateApplication(string urecNo)
        {
            // Fetch the application details using the same service method as in Details
            var applicationDetails = await _initialReviewServices.GetApplicationDetailsAsync(urecNo);

            if (applicationDetails == null)
            {
                return NotFound();
            }

            // Create the view model
            var viewModel = new ChiefEvaluationViewModel
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

            return View(viewModel);
        }



        [Authorize(Roles = "CRE Chief")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EvaluateApplication(ChiefEvaluationViewModel model)
        {
            ModelState.Remove("EthicsApplication.CompletionCertificate");
            ModelState.Remove("EthicsEvaluation.EthicsEvaluator");
            ModelState.Remove("EthicsApplication.fieldOfStudy");
            ModelState.Remove("EthicsApplication.Name");
            if (!ModelState.IsValid)
            {
                // Return the same view with the model to show validation errors
                return View(model);
            }
            // Retrieve the current user ID from the claims
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Access the chiefId

            // Create the EthicsEvaluation entity
            var ethicsEvaluation = new EthicsEvaluation
            {
                urecNo = model.EthicsApplication.urecNo, // Ensure this property exists in your model
                //chiefId = currentUserId, // Use the chiefId from the retrieved Chief
                evaluationStatus = "Evaluated",
                ProtocolRecommendation = model.EthicsEvaluation.ProtocolRecommendation,
                ProtocolRemarks = model.EthicsEvaluation.ProtocolRemarks,
                ConsentRecommendation = model.EthicsEvaluation.ConsentRecommendation,
                ConsentRemarks = model.EthicsEvaluation.ConsentRemarks,
                startDate = DateOnly.FromDateTime(DateTime.Today),
                endDate = DateOnly.FromDateTime(DateTime.Today),
                ProtocolReviewSheet = model.ProtocolReviewSheet != null ? await GetFileContentAsync(model.ProtocolReviewSheet) : null,
                InformedConsentForm = model.InformedConsentForm != null ? await GetFileContentAsync(model.InformedConsentForm) : null,
            };

            // Save the evaluation to the database
            await _ethicsEvaluationServices.SaveEvaluationAsync(ethicsEvaluation);

            // Add an entry to the EthicsApplicationLog
            // Create a log entry to record the evaluation submission
            var applicationLog = new EthicsApplicationLog
            {
                urecNo = model.EthicsApplication?.urecNo, // Link to the evaluated application
                userId = currentUserId, // ID of the user performing the evaluation
                status = "Evaluated", // Status of the application update
                changeDate = DateTime.Now, // Current date and time
                comments = "The application has been evaluated and marked as submitted."
            };
            // Save the log entry to the database
            await _ethicsApplicationLogServices.AddLogAsync(applicationLog);
            return RedirectToAction("Evaluations", new { success = true });

        }
        private async Task<byte[]> GetFileContentAsync(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }


        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public async Task<IActionResult> ViewEvaluationDetails(string urecNo, int evaluationId)
        {
            var evaluatedApplication = await _ethicsEvaluationServices.GetEvaluationDetailsAsync(urecNo, evaluationId);
            if (evaluatedApplication == null)
            {
                return NotFound();
            }
            return View(evaluatedApplication);
        }
        // Existing method for retrieving evaluation sheets
        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public async Task<IActionResult> ViewEvaluationSheet(string fileType, string urecNo, int evaluationId)
        {
            // Fetch the EthicsEvaluation based on urecNo and evaluationId
            var ethicsEvaluation = await _ethicsEvaluationServices.GetEvaluationByUrecNoAndIdAsync(urecNo, evaluationId);

            if (ethicsEvaluation == null)
            {
                return NotFound();
            }

            byte[] fileData = null;
            string contentType = "application/pdf"; // Set content type for PDF files

            // Determine which file to retrieve
            if (fileType == "ProtocolReviewSheet")
            {
                fileData = ethicsEvaluation.ProtocolReviewSheet;
            }
            else if (fileType == "InformedConsentForm")
            {
                fileData = ethicsEvaluation.InformedConsentForm;
            }

            // Check if fileData was retrieved successfully
            if (fileData == null)
            {
                return NotFound();
            }

            return File(fileData, contentType);
        }

        [Authorize]
        // New method for retrieving Form15
        [HttpGet]
        public async Task<IActionResult> ViewForm15(string urecNo)
        {
            // Fetch Form15 using the ethicsFormId from the EthicsApplicationForms table
            var form15 = await _ethicsApplicationFormsServices.GetForm15ByUrecNoAsync(urecNo);

            if (form15 == null || form15.file == null)
            {
                return NotFound();
            }

            byte[] fileData = form15.file; // Assuming FileData holds the byte array for Form15
            string contentType = "application/pdf"; // Set content type for PDF files

            return File(fileData, contentType);
        }

        public async Task<IActionResult> Evaluations()
        {
            var viewModel = new ApplicationEvaluationViewModel
            {
                ExemptApplications = await _ethicsEvaluationServices.GetExemptApplicationsAsync(),
                EvaluatedExemptApplications = await _ethicsEvaluationServices.GetEvaluatedExemptApplicationsAsync(),
                EvaluatedExpeditedApplications = await _ethicsEvaluationServices.GetEvaluatedExpeditedApplicationsAsync(),
                EvaluatedFullReviewApplications = await _ethicsEvaluationServices.GetEvaluatedFullReviewApplicationsAsync(),
                PendingIssuance = await _ethicsEvaluationServices.GetPendingApplicationsForIssuanceAsync() // Add this line
            };

            return View(viewModel);
        }

        public async Task<IActionResult> FilteredApplications()
        {
            var model = new ApplicationListViewModel
            {
                ApplicationsApprovedForEvaluation = await _ethicsApplicationServices.GetApplicationsByInitialReviewTypeAsync("Pending"),
                ExemptApplications = await _ethicsApplicationServices.GetApplicationsBySubmitReviewTypeAsync("Exempt"),
                ExpeditedApplications = await _ethicsApplicationServices.GetApplicationsBySubmitReviewTypeAsync("Expedited"),
                FullReviewApplications = await _ethicsApplicationServices.GetApplicationsBySubmitReviewTypeAsync("Full Review"),
                AllApplications = await _ethicsApplicationServices.GetAllApplicationViewModelsAsync(),
                PendingIssuance = await _ethicsEvaluationServices.GetPendingApplicationsForIssuanceAsync() // Added line
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "CRE Chief")]
        public async Task<IActionResult> IssueApplication(string urecNo)
        {
            // Call the service method
            var viewModel = await _ethicsApplicationServices.GetEvaluationDetailsAsync(urecNo);

            if (viewModel == null)
            {
                return NotFound(); // Handle application not found
            }

            return View(viewModel); // Return the view with the populated model
        }
        [HttpPost]
        public async Task<IActionResult> IssueApplication(string urecNo, IFormFile? uploadedFile, string applicationDecision, string remarks)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (applicationDecision == "Approve" && uploadedFile != null)
            {
                var ethicsClearance = new EthicsClearance
                {
                    urecNo = urecNo, // Link urecNo to EthicsClearance
                    issuedDate = DateOnly.FromDateTime(DateTime.Now), // Set issued date as DateOnly
                    expirationDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)) // Set expiration date one year from now as DateOnly
                };

                var success = await _ethicsClearanceServices.IssueEthicsClearanceAsync(ethicsClearance, uploadedFile, remarks, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Ethics clearance issued successfully!";
                    return RedirectToAction("Evaluations");
                }
            }
            else if (applicationDecision == "Minor Revisions" || applicationDecision == "Major Revisions")
            {
                // Logic for handling revisions
                var result = await _ethicsClearanceServices.HandleRevisionsAsync(urecNo, applicationDecision, remarks, userId);

                if (result)
                {
                    TempData["SuccessMessage"] = $"{applicationDecision} processed successfully!";
                    return RedirectToAction("Evaluations");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to process the revisions.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select a valid decision.");
            }

            return View();
        }


        public async Task<IActionResult> GetCompletionReport(string urecNo)
        {
            var report = await _completionReportServices.GetCompletionReportByUrecNoAsync(urecNo);

            if (report == null)
            {
                return NotFound();
            }

            var viewModel = new CompletionReportViewModel
            {
                NonFundedResearchInfo = report.EthicsApplication.NonFundedResearchInfo,
                CoProponent = report.EthicsApplication.NonFundedResearchInfo.CoProponent,
                EthicsApplication = report.EthicsApplication,
                EthicsApplicationLog = report.EthicsApplication.EthicsApplicationLog,
                CompletionReport = report,
                CompletionCertificate = report.EthicsApplication.CompletionCertificate
            };

            return View(viewModel);
        }

        public async Task<IActionResult> CompletionReports()
        {
            var completionReports = await _completionReportServices.GetCompletionReportsAsync();

            // Convert to ViewModel
            var viewModel = completionReports.Select(report => new CompletionReportViewModel
            {
                NonFundedResearchInfo = report.NonFundedResearchInfo,
                CoProponent = report.CoProponent,
                EthicsApplication = report.EthicsApplication,
                CompletionReport = report.CompletionReport,
                CompletionCertificate = report.CompletionCertificate
            }).ToList();

            return View(viewModel);
        }
        public async Task<IActionResult> DownloadTerminalReport(string urecNo)
        {
            // Fetch the terminal report as a byte array (PDF format) from your database or file storage
            var reportBytes = await _completionReportServices.GetTerminalReportAsync(urecNo);

            if (reportBytes == null)
            {
                return NotFound();
            }

            return File(reportBytes, "application/pdf");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IssueCompletionCertificate(CompletionReportViewModel model)
        {
            ModelState.Remove("CompletionReport.terminalReport");
            ModelState.Remove("CompletionReport.EthicsApplication");
            ModelState.Remove("EthicsApplication.fieldOfStudy");
            ModelState.Remove("EthicsApplication.CompletionCertificate");
            if (ModelState.IsValid)
            {
                // Fetch the CompletionReport based on the UREC number
                var completionReport = await _completionReportServices.GetCompletionReportByUrecNoAsync(model.EthicsApplication.urecNo);

                if (completionReport == null)
                {
                    ModelState.AddModelError("", "Completion report not found.");
                    return View(model);
                }

                // Handle the file upload: Convert the IFormFile to a byte array for storage
                byte[] certificateBytes = null;

                if (model.CertificateFile != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.CertificateFile.CopyToAsync(memoryStream);
                        certificateBytes = memoryStream.ToArray();
                    }
                }

                // Create a new CompletionCertificate or update the existing one
                var completionCertificate = new CompletionCertificate
                {
                    urecNo = model.EthicsApplication.urecNo,
                    file = certificateBytes, // Store the byte array of the certificate
                    issuedDate = model.CompletionCertificate?.issuedDate ?? DateOnly.FromDateTime(DateTime.UtcNow)
                };

                // Save the certificate to the database (via service or repository)
                await _completionCertificateServices.SaveCompletionCertificateAsync(completionCertificate);

                // Now, update the researchEndDate in the CompletionReport to match the issuedDate
                completionReport.researchEndDate = model.CompletionReport.researchEndDate;

                // Save the updated CompletionReport
                await _completionReportServices.SaveCompletionReportAsync(completionReport);


                // Add a log entry for this action (optional)
                var log = new EthicsApplicationLog
                {
                    urecNo = model.EthicsApplication.urecNo,
                    status = "Completion Certificate Issued",
                    changeDate = DateTime.UtcNow,
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    comments = "Completion certificate has been issued."
                };

                await _ethicsApplicationLogServices.AddLogAsync(log);

                // Redirect to the page that lists completion certificates (or another appropriate action)
                return RedirectToAction("CompletionReports", "Chief");
            }

            // If model is invalid, return the form with validation messages
            return View(model);
        }
    }
}