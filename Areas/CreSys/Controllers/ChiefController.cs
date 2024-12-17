using CrdlSys.Data;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using RemcSys.Data;
using RemcSys.Models;
using ResearchManagementSystem.Areas.CreSys.Data;
using ResearchManagementSystem.Areas.CreSys.Interfaces;
using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Areas.CreSys.ViewModels;
using ResearchManagementSystem.Areas.CreSys.ViewModels.FormClasses;
using ResearchManagementSystem.Areas.CreSys.ViewModels.ListViewModels;
using ResearchManagementSystem.Models;
using System.IO.Compression;
using System.Net.Mail;
using System.Security.Claims;

namespace ResearchManagementSystem.Areas.CreSys.Controllers
{
    [Area("CreSys")]
    [Authorize]
    public class ChiefController : Controller
    {
        private readonly CreDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEthicsEmailService _emailService;
        private readonly IAllServices _allServices;
        private readonly IPdfGenerationService _pdfGenerationService;
        private readonly CrdlDbContext _crdlDbContext;
        private readonly RemcDBContext _remcDbContext;
        private readonly ActionLoggerService _actionLogger;

        public ChiefController(CreDbContext context, UserManager<ApplicationUser> userManager, IEthicsEmailService emailService, IAllServices allServices, RoleManager<IdentityRole> roleManager, IPdfGenerationService pdfGenerationService, CrdlDbContext crdlDbContext, RemcDBContext remcDBContext, ActionLoggerService actionLogger)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _allServices = allServices;
            _roleManager = roleManager;
            _pdfGenerationService = pdfGenerationService;
            _crdlDbContext = crdlDbContext;
            _remcDbContext = remcDBContext;
            _actionLogger = actionLogger;
        }
        public IActionResult Index()
        {
            return View();
        }

        //viewing of the forms in table fomart
        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public async Task<IActionResult> EthicsForms()
        {
            // Fetch all EthicsForms from the database
            var ethicsForms = await _context.CRE_EthicsForms.ToListAsync();

            // Return the view with the list of EthicsForms
            return View(ethicsForms);
        }

        public async Task<IActionResult> EditForms(string id)
        {
            var form = await _context.CRE_EthicsForms.FindAsync(id);

            if (form == null)
            {
                return NotFound();
            }

            return View(form);
        }
        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public IActionResult UploadForm()
        {
            return View();
        }

        [Authorize(Roles = "CRE Chief")]
         
        [HttpPost]
        public async Task<IActionResult> UploadForm(string ethicsFormId, string formName, string formDescription, IFormFile file, bool isFileRequired)
        {
            // Check if the EthicsFormId already exists
            var existingForm = await _context.CRE_EthicsForms
                                             .FirstOrDefaultAsync(e => e.EthicsFormId == ethicsFormId);

            if (existingForm != null)
            {
                ModelState.AddModelError("ethicsFormId", "A form with this ID already exists.");
                return View();
            }

            // If the file is not uploaded, add an error
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Please upload a file.");
                return View();
            }

            // Check if the uploaded file is a valid .docx file
            if (file.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            {
                ModelState.AddModelError("file", "Only .docx files are allowed.");
                return View();
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream); // Read the file into memory

                // Create a new EthicsForm object
                var ethicsForm = new EthicsForms
                {
                    EthicsFormId = ethicsFormId,
                    FormName = formName,
                    FormDescription = formDescription,
                    EthicsFormFile = memoryStream.ToArray(), // Store file as byte array
                    DateCreated = DateTime.Now, // Automatically set the creation date
                    IsRequired = isFileRequired // Store whether the file is required
                };

                // Save the file data to the database
                _context.CRE_EthicsForms.Add(ethicsForm);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("EthicsForms"); // Redirect to the Index page after saving
        }

        public async Task<IActionResult> DownloadFile(string id)
        {
            var form = await _context.CRE_EthicsForms
                .Where(f => f.EthicsFormId == id)
                .FirstOrDefaultAsync();

            if (form == null)
            {
                return NotFound();
            }
            if (form.EthicsFormFile == null || form.EthicsFormFile.Length == 0)
            {
                return NotFound();
            }
            string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; contentType = "application/octet-stream";
            string fileName = form.FormName;
            if (!fileName.EndsWith(".docx", System.StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".docx";
            }
            return File(form.EthicsFormFile, contentType, fileName);
        }


        [Authorize(Roles = "CRE Chief")]
         
        [HttpPost]
        public async Task<IActionResult> EditForms(string id, IFormFile? newFile, string formName, string formDescription, bool isFileRequired)
        {
            var form = await _context.CRE_EthicsForms.FindAsync(id);

            if (form == null)
            {
                return NotFound();
            }

            form.FormName = formName;
            form.FormDescription = formDescription;
            form.IsRequired = isFileRequired;

            if (newFile != null && newFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await newFile.CopyToAsync(memoryStream);
                    form.EthicsFormFile = memoryStream.ToArray();  
                    form.FormName = newFile.FileName;            
                }
            }

            _context.Update(form);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Form updated successfully!";
            return RedirectToAction("EthicsForms");
        }

        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public async Task<IActionResult> DeleteForm(string id)
        {
            var form = await _context.CRE_EthicsForms.FirstOrDefaultAsync(f => f.EthicsFormId == id);

            if (form == null)
            {
                return NotFound();
            }
            return View(form);
        }

        [Authorize(Roles = "CRE Chief")]
         
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string EthicsFormId)
        {
            var form = await _context.CRE_EthicsForms.FindAsync(EthicsFormId);

            if (form == null)
            {
                return NotFound();
            }
            _context.CRE_EthicsForms.Remove(form);
            await _context.SaveChangesAsync();
            return RedirectToAction("EthicsForms");
        }

        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public async Task<IActionResult> AssignReviewType(string urecNo)
        {
            if (string.IsNullOrEmpty(urecNo))
            {
                return BadRequest("Invalid UrecNo.");
            }

            // Use `Select` to retrieve only necessary properties and avoid loading entire entity graphs
            var applicationDetails = await _context.CRE_EthicsApplication
                   .Where(app => app.UrecNo == urecNo)
                   .Include(app => app.NonFundedResearchInfo) // Include NonFundedResearchInfo
                    .ThenInclude(nonFundedResearch => nonFundedResearch.CoProponents) // Include CoProponents
                   .Include(app => app.ReceiptInfo)           // Include ReceiptInfo
                   .Include(app => app.InitialReview)         // Include InitialReview
                   .Include(app => app.EthicsApplicationForms) // Include EthicsApplicationForms
                   .Include(app => app.EthicsApplicationLogs)  // Include EthicsApplicationLogs
                  
                   .FirstOrDefaultAsync();


            if (applicationDetails == null)
            {
                return NotFound();
            }

            // Map to the view model using the retrieved data
            var viewModel = new AssignReviewTypeViewModel
            {
                NonFundedResearchInfo = applicationDetails.NonFundedResearchInfo,
                CoProponent = applicationDetails.NonFundedResearchInfo.CoProponents.ToList(),
                ReceiptInfo = applicationDetails.ReceiptInfo,
                EthicsApplication = applicationDetails,
                InitialReview = applicationDetails.InitialReview,
                EthicsApplicationForms = applicationDetails.EthicsApplicationForms,
                EthicsApplicationLogs = applicationDetails.EthicsApplicationLogs,
                ReviewType = applicationDetails.InitialReview?.ReviewType
            };

            return View(viewModel);
        }

        
        [HttpPost]
        public async Task<IActionResult> SubmitReviewType(SubmitReviewTypeViewModel model)
            {
            if (string.IsNullOrEmpty(model.ReviewType) || string.IsNullOrEmpty(model.UrecNo))
            {
                return BadRequest("Required fields (UREC No. and Review Type) are missing.");
            }

            // Fetch the InitialReview using the UrecNo from the model
            var initialReview = await _context.CRE_InitialReview
                .Include(ir => ir.EthicsApplication)
                .FirstOrDefaultAsync(ir => ir.EthicsApplication.UrecNo == model.UrecNo);

            if (initialReview != null)
            {
                // Update the ReviewType field
                initialReview.ReviewType = model.ReviewType;

                // Save the changes to the database
                _context.Update(initialReview);
                await _context.SaveChangesAsync();

                // Log the action
                var logEntry = new EthicsApplicationLogs
                {
                    UrecNo = initialReview.UrecNo,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    ChangeDate = DateTime.Now,
                    Status = "Review Type Assigned"
                };

                // Add the log entry
                _context.Add(logEntry);
                await _context.SaveChangesAsync();

                // Send email and notification to chairperson if ReviewType is "Full Review" or "Expedited"
                if (model.ReviewType == "Full Review" || model.ReviewType == "Expedited")
                {
                    // Get FieldOfStudy of the application
                    var fieldOfStudy = initialReview.EthicsApplication?.FieldOfStudy;

                    if (fieldOfStudy != null)
                    {
                        // Find the chairperson based on the FieldOfStudy
                        var chairperson = await _context.CRE_Chairperson
                            .FirstOrDefaultAsync(c => c.FieldOfStudy == fieldOfStudy);

                        if (chairperson != null)
                        {
                            // Retrieve the chairperson's email using Identity UserManager
                            var chairpersonUser = await _userManager.Users
                                .FirstOrDefaultAsync(u => u.Id == chairperson.UserId);

                            if (chairpersonUser != null && !string.IsNullOrEmpty(chairpersonUser.Email))
                            {
                                string recipientName = chairperson.Name; // Replace with appropriate name property
                                string chairpersonEmail = chairpersonUser.Email;
                                string subject = "New Ethics Application Pending for Review";
                                string body = $@"
                                    <p>Dear {recipientName},</p>
                                    <p>We would like to notify you that a new Ethics Application with UREC No: 
                                       <strong>{model.UrecNo}</strong> has been assigned for a <strong>{model.ReviewType}</strong> review.</p>
                                    <p>Please log in to the system to assign the application.</p>
                                    <p>Thank you.</p>";

                                // Send email notification
                                await _emailService.SendEmailAsync(chairpersonEmail, recipientName, subject, body);

                                // Add notification record for the chairperson
                                var notification = new EthicsNotifications
                                {
                                    UrecNo = model.UrecNo,
                                    UserId = chairperson.UserId,
                                    NotificationTitle = "New Application Assigned",
                                    NotificationMessage = $"A new application with UREC No: {model.UrecNo} has been assigned to you for {model.ReviewType} review.",
                                    NotificationCreationDate = DateTime.Now,
                                    NotificationStatus = false, // Unread
                                    Role = "Chairperson", // Ensure this matches the role in your system
                                    PerformedBy = "System" // Logged-in user who performed the action
                                };
                                await _context.CRE_EthicsNotifications.AddAsync(notification);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("ApplicationsApprovedForEvaluation");
        }


        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public async Task<IActionResult> FilteredApplications()
        {
            var model = new ApplicationListViewModel
            {
                ApplicationsApprovedForEvaluation = await GetApplicationsByInitialReviewTypeAsync("Pending"),
                ExemptApplications = await GetApplicationsBySubmitReviewTypeAsync("Exempt"),
                ExpeditedApplications = await GetApplicationsBySubmitReviewTypeAsync("Expedited"),
                FullReviewApplications = await GetApplicationsBySubmitReviewTypeAsync("Full Review"),
                AllApplications = await GetAllApplicationViewModelsAsync(),
                PendingIssuance = await _allServices.GetPendingApplicationsForIssuanceAsync()
            };

            return View(model);
        }
        public async Task<IActionResult> ApplicationsApprovedForEvaluation()
        {
            var model = await GetApplicationsByInitialReviewTypeAsync("Pending");   
            return View(model); 
        }

        public async Task<IActionResult> ExemptApplications()
        {
            var model = await GetApplicationsBySubmitReviewTypeAsync("Exempt");
            return View(model);
        }

        public async Task<IActionResult> ExpeditedApplications()
        {
            var model = await GetApplicationsBySubmitReviewTypeAsync("Expedited");
            return View(model); 
        }

        public async Task<IActionResult> FullReviewApplications()
        {
            var model = await GetApplicationsBySubmitReviewTypeAsync("Full Review");
            return View(model); 
        }

        public async Task<IActionResult> PendingIssuance()
        {
            var model = await _allServices.GetPendingApplicationsForIssuanceAsync();
            return View(model);
        }

        public async Task<IActionResult> AllApplications()
        {
            var model = await GetAllApplicationViewModelsAsync();
            return View(model); // Return a view specific to this tab
        }

        private async Task<List<ApplicationViewModel>> GetApplicationsByInitialReviewTypeAsync(string reviewType)
        {
            // Retrieve only necessary data to minimize memory usage and database load
            var applications = await _context.CRE_EthicsApplication
                .Where(a => a.InitialReview.ReviewType == reviewType &&  // Filter based on review type
                            a.InitialReview.Status == "Approved" &&     // Include only "Approved" status
                            a.InitialReview.Status != "Returned" &&     // Exclude "Returned" status
                            a.NonFundedResearchInfo != null)             // Ensure NonFundedResearchInfo exists
                .Select(a => new ApplicationViewModel
                {
                    EthicsApplication = a,
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    ReceiptInfo = a.ReceiptInfo,
                    EthicsApplicationForms = a.EthicsApplicationForms,  // Include forms directly
                    CoProponent = a.NonFundedResearchInfo.CoProponents.ToList(),
                    // Only take the latest "Approved for Evaluation" log
                    EthicsApplicationLogs = a.EthicsApplicationLogs
                        .Where(log => log.Status == "Approved for Evaluation")  // Filter logs early
                        .OrderByDescending(log => log.ChangeDate)               // Sort logs by change date
                        .Take(1)                                                // Take the latest log
                        .ToList(),
                    InitialReview = a.InitialReview
                })
                .ToListAsync();

            return applications;
        }

        private async Task<List<ApplicationViewModel>> GetApplicationsBySubmitReviewTypeAsync(string reviewType)
        {
            // Retrieve only necessary data to minimize memory usage and database load
            var applications = await _context.CRE_EthicsApplication
                .Where(a => a.InitialReview.ReviewType == reviewType &&  // Filter based on review type
                            a.NonFundedResearchInfo != null &&           // Ensure NonFundedResearchInfo exists
                            a.EthicsApplicationLogs.Any())               // Ensure at least one log exists
                .Select(a => new ApplicationViewModel
                {
                    EthicsApplication = a,
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    ReceiptInfo = a.ReceiptInfo,
                    EthicsApplicationForms = a.EthicsApplicationForms,  // Include forms directly
                    CoProponent = a.NonFundedResearchInfo.CoProponents.ToList(),
                    // Only take the latest log and filter based on "Approved for Evaluation"
                    EthicsApplicationLogs = a.EthicsApplicationLogs
                        .Where(log => log.Status == "Approved for Evaluation")  // Filter logs early
                        .OrderByDescending(log => log.ChangeDate)               // Sort logs by change date
                        .Take(1)                                                // Take the latest log
                        .ToList(),
                    InitialReview = a.InitialReview
                })
                .ToListAsync();

            return applications;
        }


        private async Task<List<ApplicationViewModel>> GetAllApplicationViewModelsAsync()
        {
            // Retrieve the data from the database
            var applications = await _context.CRE_EthicsApplication
                .Where(a => a.NonFundedResearchInfo != null &&  // Ensure NonFundedResearchInfo exists
                            a.EthicsApplicationLogs.Any())    // Ensure at least one log exists
                .Select(a => new ApplicationViewModel
                {
                    EthicsApplication = a,
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    ReceiptInfo = a.ReceiptInfo,
                    EthicsApplicationForms = a.EthicsApplicationForms.ToList(),  // Convert forms to list
                    CoProponent = a.NonFundedResearchInfo.CoProponents.ToList(),  // Select only the necessary co-proponent data
                    EthicsApplicationLogs = a.EthicsApplicationLogs
                        .OrderByDescending(log => log.ChangeDate)
                        .ToList(),  // Sort logs by ChangeDate in-memory
                    InitialReview = a.InitialReview
                })
                .ToListAsync();  // Executes the query and retrieves the data

            return applications;
        }

        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public async Task<IActionResult> Evaluations()
        {
            var viewModel = new ApplicationEvaluationListViewModel
            {
                ExemptApplications = await _allServices.GetExemptApplicationsAsync(),
                EvaluatedExemptApplications = await _allServices.GetEvaluatedExemptApplicationsAsync(),
                EvaluatedExpeditedApplications = await _allServices.GetEvaluatedExpeditedApplicationsAsync(),
                EvaluatedFullReviewApplications = await _allServices.GetEvaluatedFullReviewApplicationsAsync(),
                PendingIssuance = await _allServices.GetPendingApplicationsForIssuanceAsync() // Add this line
            };

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> EvaluatedExemptApplications()
        {
            var exemptApplications = await _allServices.GetEvaluatedExemptApplicationsAsync();
            return View(exemptApplications);
        }
        [HttpGet] 
        public async Task<IActionResult> PendingExemptEvaluations()
        {
            var exemptApplications = await _allServices.GetExemptApplicationsAsync();
            return View(exemptApplications);
        }

        [HttpGet]
        public async Task<IActionResult> EvaluatedExpeditedApplications()
        {
            var expeditedApplications = await _allServices.GetEvaluatedExpeditedApplicationsAsync();
            return View(expeditedApplications);
        }

        [HttpGet]
        public async Task<IActionResult> EvaluatedFullReviewApplications()
        {
            var fullReviewApplications = await _allServices.GetEvaluatedFullReviewApplicationsAsync();
            return View(fullReviewApplications);
        }

        [HttpGet]
        public async Task<IActionResult> PendingApplicationsForIssuance()
        {
            var pendingApplications = await _allServices.GetPendingApplicationsForIssuanceAsync();
            return View(pendingApplications);
        }
        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public async Task<IActionResult> ViewEvaluationDetails(string urecNo, int evaluationId)
        {
            // Fetch the evaluation based on both urecNo and evaluationId directly
            var evaluation = await _context.CRE_EthicsEvaluation
                .Include(e => e.EthicsApplication) // Include the EthicsApplication
                    .ThenInclude(a => a.NonFundedResearchInfo) // Include NonFundedResearchInfo
                .Include(e => e.EthicsApplication.EthicsApplicationLogs) // Include EthicsApplicationLogs
                .Include(e => e.EthicsApplication.InitialReview) // Include InitialReview from EthicsApplication
                .FirstOrDefaultAsync(e => e.UrecNo == urecNo && e.EvaluationId == evaluationId);

            // If the evaluation is not found, return NotFound
            if (evaluation == null)
            {
                return NotFound();
            }

            // Map the fetched evaluation data to a custom view model or anonymous object
            var evaluatedExemptApplication = new EvaluatedExemptApplication
            {
                EthicsApplication = evaluation.EthicsApplication,
                NonFundedResearchInfo = evaluation.EthicsApplication?.NonFundedResearchInfo,
                EthicsApplicationLog = evaluation.EthicsApplication?.EthicsApplicationLogs,
                EthicsEvaluation = evaluation,
                InitialReview = evaluation.EthicsApplication?.InitialReview, // Set InitialReview
            };

            // Return the evaluatedExemptApplication to the view
            return View(evaluatedExemptApplication);
        }
        [HttpGet]
        public async Task<IActionResult> ViewEvaluationSheet(string fileType, string urecNo, int evaluationId)
        {
            // Fetch the EthicsEvaluation based on urecNo and evaluationId directly
            var ethicsEvaluation = await _context.CRE_EthicsEvaluation
                .FirstOrDefaultAsync(e => e.UrecNo == urecNo && e.EvaluationId == evaluationId);

            // Check if the evaluation exists
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

        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public async Task<IActionResult> EvaluateApplication(string urecNo)
        {
            // Fetch all application details in one query
            var application = await _context.CRE_EthicsApplication
                .Include(e => e.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponents)
                .Include(e => e.EthicsEvaluation)
                .Include(e => e.DeclinedEthicsEvaluation)
                .Include(e => e.ReceiptInfo)
                .Include(e => e.EthicsApplicationLogs)
                .Include(e => e.EthicsApplicationForms)
                .Include(e => e.InitialReview)
                .FirstOrDefaultAsync(e => e.UrecNo == urecNo);

            // If application not found, return NotFound
            if (application == null)
            {
                return NotFound();
            }

            var evaluatorUserIds = application.EthicsEvaluation
               .Select(e => e.UserId) // Assuming 'UserId' is what links to the 'EthicsEvaluator'
               .Distinct()
               .ToList();
            // Fetch evaluators based on the user IDs retrieved
            var ethicsEvaluators = await _context.CRE_EthicsEvaluator
                .Where(e => evaluatorUserIds.Contains(e.UserID)) // Match user IDs
                .ToListAsync();

            var ethicsEvaluations = application.EthicsEvaluation.ToList(); // Use only if needed for other parts


            var viewModel = new ChiefEvaluationViewModel
            {
                NonFundedResearchInfo = application.NonFundedResearchInfo,
                CoProponent = application.NonFundedResearchInfo?.CoProponents ?? new List<CoProponent>(),
                ReceiptInfo = application.ReceiptInfo,
                EthicsEvaluators = ethicsEvaluators,
                EthicsApplication = application,
                InitialReview = application.InitialReview,
                EthicsApplicationForms = application.EthicsApplicationForms ?? new List<EthicsApplicationForms>(),
                EthicsApplicationLog = application.EthicsApplicationLogs.OrderByDescending(log => log.ChangeDate).ToList(),
            };

            return View(viewModel);
        }

        [Authorize(Roles = "CRE Chief")]
         
        [HttpPost]
        public async Task<IActionResult> EvaluateApplication(ChiefEvaluationViewModel model)
        {
            // Manual server-side validation for required fields
            if (model.EthicsApplication?.UrecNo == null ||
                string.IsNullOrWhiteSpace(model.EthicsEvaluation.ProtocolRecommendation) ||
                string.IsNullOrWhiteSpace(model.EthicsEvaluation.ConsentRecommendation))
            {
                ModelState.AddModelError("", "Please fill in all required fields.");
                return View(model); // Return the view with validation errors
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the current user details using UserManager
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (currentUser == null)
            {
                return NotFound("User not found.");
            }

            var fullName = currentUser.FirstName;

            if (!string.IsNullOrEmpty(currentUser.MiddleName))
            {
                fullName += " " + currentUser.MiddleName[0] + ".";
            }

            fullName += " " + currentUser.LastName;

            var ethicsEvaluation = new EthicsEvaluation
            {
                UrecNo = model.EthicsApplication.UrecNo,
                UserId = currentUserId,
                Name = fullName,
                EvaluationStatus = "Evaluated",
                ProtocolRecommendation = model.EthicsEvaluation.ProtocolRecommendation,
                ProtocolRemarks = model.EthicsEvaluation.ProtocolRemarks,
                ConsentRecommendation = model.EthicsEvaluation.ConsentRecommendation,
                ConsentRemarks = model.EthicsEvaluation.ConsentRemarks,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                ProtocolReviewSheet = model.ProtocolReviewSheet != null
                    ? await GetFileContentAsync(model.ProtocolReviewSheet)
                    : null,
                InformedConsentForm = model.InformedConsentForm != null
                    ? await GetFileContentAsync(model.InformedConsentForm)
                    : null,
            };

            _context.CRE_EthicsEvaluation.Add(ethicsEvaluation);

            var applicationLog = new EthicsApplicationLogs
            {
                UrecNo = model.EthicsApplication.UrecNo,
                UserId = currentUserId,
                Status = "Evaluated",
                ChangeDate = DateTime.Now,
                Comments = "The application has been evaluated."
            };
            _context.CRE_EthicsApplicationLogs.Add(applicationLog);

            var application = await _context.CRE_EthicsApplication
                .Include(app => app.NonFundedResearchInfo)
                .FirstOrDefaultAsync(app => app.UrecNo == model.EthicsApplication.UrecNo);

            if (application == null)
            {
                return NotFound("Application not found.");
            }

            var user = await _userManager.FindByIdAsync(application.NonFundedResearchInfo.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            string recipientName = application.NonFundedResearchInfo.Name;
            string userEmail = user.Email;
            string subject = "Your Ethics Application Has Been Evaluated";
            string body = $@"
                <p>We are pleased to inform you that the Ethics Application (UrecNo: <strong>{model.EthicsApplication.UrecNo}</strong>) has been evaluated.</p>
                <p>Your submission has been reviewed, and the process will continue accordingly.</p>
                <p>If any further steps are required, you will be notified accordingly. Please stay tuned for further updates.</p>
                <p>Thank you for your submission and cooperation.</p>";

            await _emailService.SendEmailAsync(userEmail, recipientName, subject, body);

            string userRole = await GetUserRole(application.NonFundedResearchInfo.UserId);

            var notification = new EthicsNotifications
            {
                UrecNo = model.EthicsApplication.UrecNo,
                UserId = application.NonFundedResearchInfo.UserId,
                NotificationTitle = "Application Evaluated",
                NotificationMessage = $"Your Ethics Application (UrecNo: {model.EthicsApplication.UrecNo}) has been evaluated and processed.",
                NotificationCreationDate = DateTime.Now,
                NotificationStatus = false,
                Role = userRole,
                PerformedBy = "System"
            };

            // Add notification to the database
            await _context.CRE_EthicsNotifications.AddAsync(notification);

            await _context.SaveChangesAsync();

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

        private async Task<string> GetUserRole(string userId)
        {
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId));

            // Check and return the first matching role
            if (roles.Contains("Student"))
            {
                return "Student";
            }
            if (roles.Contains("External Researcher"))
            {
                return "External Researcher";
            }
            if (roles.Contains("Faculty"))
            {
                return "Faculty";
            }

            // Return default or a fallback role if none found
            return "Unknown"; // Or some other default role
        }

        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public async Task<IActionResult> IssueApplication(string urecNo)
        {
            // Fetch the application and include related data directly in the controller
            var application = await _context.CRE_EthicsApplication
                .Include(app => app.EthicsEvaluation) // Include evaluations
                .Include(app => app.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponents)
                .Include(app => app.InitialReview)
                .Include(app => app.ReceiptInfo)
                .Include(app => app.EthicsApplicationForms)
                .Include(app => app.EthicsApplicationLogs)
                .Include(app => app.EthicsClearance)
                .FirstOrDefaultAsync(app => app.UrecNo == urecNo);

            // Check if the application exists
            if (application == null)
            {
                return NotFound(); // Handle application not found
            }

            // Retrieve the EthicsEvaluator using UserIds from EthicsEvaluation
            var evaluatorUserIds = application.EthicsEvaluation
                .Select(e => e.UserId) // Assuming UserId is a FK in EthicsEvaluation
                .Where(userId => userId != null) // Avoid null UserIds
                .Distinct()
                .ToList();

            var ethicsEvaluators = await _context.CRE_EthicsEvaluator
                .Where(ev => evaluatorUserIds.Contains(ev.UserID)) // Match UserIds to fetch evaluators
                .ToListAsync();

            // Map evaluators to evaluations in-memory
            var evaluationsWithEvaluators = application.EthicsEvaluation.Select(evaluation =>
            {
                var evaluator = ethicsEvaluators.FirstOrDefault(ev => ev.UserID == evaluation.UserId);
                return new
                {
                    Evaluation = evaluation,
                    Evaluator = evaluator
                };
            }).ToList();

            // Create the view model
            var viewModel = new EvaluationDetailsViewModel
            {
                NonFundedResearchInfo = application.NonFundedResearchInfo,
                EthicsApplication = application,
                InitialReview = application.InitialReview,
                ReceiptInfo = application.ReceiptInfo,
                EthicsApplicationForms = application.EthicsApplicationForms,
                EthicsApplicationLog = application.EthicsApplicationLogs,
                EthicsEvaluation = application.EthicsEvaluation,
                CurrentEvaluation = application.EthicsEvaluation.FirstOrDefault(),
                CoProponent = application.NonFundedResearchInfo?.CoProponents,
                HasEthicsClearance = application.EthicsClearance != null,
                EthicsClearance = application.EthicsClearance,
                EvaluationsWithEvaluators = evaluationsWithEvaluators.Select(x => new EvaluationWithEvaluatorViewModel
                {
                    Evaluation = x.Evaluation,
                    EvaluatorName = x.Evaluator?.Name,
                    EvaluatorUserId = x.Evaluator?.UserID
                }).ToList()
            };

            // Return the view with the populated model
            return View(viewModel);
        }

        [Authorize(Roles = "CRE Chief")]
         
        [HttpPost]
        public async Task<IActionResult> IssueApplication(string urecNo, IFormFile? uploadedFile, string applicationDecision, string remarks)
        {
            // Retrieve the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the application from the database
            var application = await _context.CRE_EthicsApplication
                .Include(app => app.NonFundedResearchInfo)
                .FirstOrDefaultAsync(app => app.UrecNo == urecNo);

            if (application == null)
            {
                ModelState.AddModelError("", "Application not found.");
                return View();
            }

            // Retrieve the user associated with the application
            var user = await _userManager.FindByIdAsync(application.NonFundedResearchInfo.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "User associated with the application not found.");
                return View();
            }

            string recipientName = application.NonFundedResearchInfo.Name;
            string userEmail = user.Email;

            if (applicationDecision == "Approve" && uploadedFile != null)
            {
                remarks = "Ethics Clearance Issued";

                // Create a new EthicsClearance record
                var ethicsClearance = new EthicsClearance
                {
                    UrecNo = urecNo,
                    IssuedDate = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddYears(1),
                };

                // Handle file upload
                if (uploadedFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await uploadedFile.CopyToAsync(memoryStream);
                        ethicsClearance.ClearanceFile = memoryStream.ToArray();
                    }
                }

                var fre = await _remcDbContext.REMC_FundedResearchEthics.FirstOrDefaultAsync(f => f.urecNo == urecNo);
                if(fre != null)
                {
                    if(uploadedFile.Length > 0)
                    {
                        using(var memoryStream = new MemoryStream())
                        {
                            await uploadedFile.CopyToAsync(memoryStream);
                            fre.clearanceFile = memoryStream.ToArray();
                        }
                    }

                    fre.file_Name = $"{urecNo}_EthicsClearance.pdf";
                    fre.file_Type = ".pdf";
                    fre.file_Status = "Checked";
                    fre.file_Uploaded = DateTime.Now;

                    var fra = await _remcDbContext.REMC_FundedResearchApplication.FirstAsync(f => f.fra_Id == fre.fra_Id);
                    await _actionLogger.LogActionAsync(fra.applicant_Name, fra.fra_Type, $"{fra.research_Title} already has Ethics Clearance", true, true, false, fra.fra_Id);
                }


                // Save clearance and log issuance
                _context.CRE_EthicsClearance.Add(ethicsClearance);
                await _context.SaveChangesAsync();  // Save to get the ID of the new EthicsClearance

                var nonFundedResearchInfo = await _context.CRE_NonFundedResearchInfo
                                             .FirstOrDefaultAsync(nf => nf.UrecNo == urecNo);

                if (nonFundedResearchInfo != null)
                {
                    // Set the EthicsClearanceId in NonFundedResearchInfo
                    nonFundedResearchInfo.EthicsClearanceId = ethicsClearance.EthicsClearanceId;

                    // Save the updated NonFundedResearchInfo record
                    await _context.SaveChangesAsync();
                }

                var applicationLog = new EthicsApplicationLogs
                {
                    UrecNo = urecNo,
                    Status = "Clearance Issued",
                    Comments = remarks,
                    ChangeDate = DateTime.Now,
                    UserId = userId,
                };

                _context.CRE_EthicsApplicationLogs.Add(applicationLog);
                await _context.SaveChangesAsync();

                // Send Email with Clearance Attachment
                string subject = "Ethics Clearance Issued";
                string body = $@"
                    <p>We are pleased to inform you that your Ethics Application (UrecNo: <strong>{urecNo}</strong>) has been approved.</p>
                    <p>The clearance is valid from <strong>{ethicsClearance.IssuedDate:MMMM dd, yyyy}</strong> to <strong>{ethicsClearance.ExpirationDate:MMMM dd, yyyy}</strong>.</p>
                    <p>Attached is your clearance file for reference.</p>
                    <p>Thank you!</p>";

                var attachments = new List<Attachment>();

                // List of MimePart attachments
                List<MimeKit.MimePart> mimeAttachments = new List<MimeKit.MimePart>();

                // Add the file as an attachment if available
                if (ethicsClearance.ClearanceFile != null && ethicsClearance.ClearanceFile.Length > 0)
                {
                    var mimePart = new MimePart("application", "pdf")
                    {
                        Content = new MimeContent(new MemoryStream(ethicsClearance.ClearanceFile)),
                        FileName = "EthicsClearance.pdf"
                    };

                    // Add MimePart to the list of mimeAttachments
                    mimeAttachments.Add(mimePart);
                }

                // Call the service to send email with attachments
                await _emailService.SendEmailWithAttachmentsAsync(userEmail, recipientName, subject, body, mimeAttachments);

                // Add a notification for clearance issuance
                string userRole = await GetUserRole(application.NonFundedResearchInfo.UserId);
                var notification = new EthicsNotifications
                {
                    UrecNo = urecNo,
                    UserId = application.NonFundedResearchInfo.UserId,
                    NotificationTitle = "Ethics Clearance Issued",
                    NotificationMessage = $"Your Ethics Application (UrecNo: {urecNo}) has been approved. Check your email for the clearance file.",
                    NotificationCreationDate = DateTime.Now,
                    NotificationStatus = false,
                    Role = userRole,
                    PerformedBy = "System",
                };

                _context.CRE_EthicsNotifications.Add(notification);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Ethics clearance issued successfully!";
                return RedirectToAction("PendingExemptEvaluations");
            }
            else if (applicationDecision == "Minor Revisions" || applicationDecision == "Major Revisions")
            {
                // Log the revision request
                var logEntry = new EthicsApplicationLogs
                {
                    UrecNo = urecNo,
                    UserId = userId,
                    Comments = remarks,
                    Status = applicationDecision,
                    ChangeDate = DateTime.Now,
                };

                _context.CRE_EthicsApplicationLogs.Add(logEntry);
                await _context.SaveChangesAsync();

                // Send Email for Revisions
                string subject = "Your Ethics Application Requires Revisions";
                string body = $@"
                    <p>Your Ethics Application (UrecNo: <strong>{urecNo}</strong>) requires <strong>{applicationDecision}</strong>.</p>
                    <p>Feedback: {remarks}</p>
                    <p>Please address the feedback and resubmit your application.</p>
                    <p>Thank you.</p>";

                await _emailService.SendEmailAsync(userEmail, recipientName, subject, body);

                // Add a notification for revisions
                string userRole = await GetUserRole(application.NonFundedResearchInfo.UserId);
                var notification = new EthicsNotifications
                {
                    UrecNo = urecNo,
                    UserId = application.NonFundedResearchInfo.UserId,
                    NotificationTitle = "Application Requires Revisions",
                    NotificationMessage = $"Your Ethics Application (UrecNo: {urecNo}) requires {applicationDecision}. Please check your email for feedback.",
                    NotificationCreationDate = DateTime.Now,
                    NotificationStatus = false,
                    Role = userRole,
                    PerformedBy = userId,
                };

                _context.CRE_EthicsNotifications.Add(notification);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"{applicationDecision} processed successfully!";
                return RedirectToAction("PendingExemptEvaluations");
            }
            else
            {
                ModelState.AddModelError("", "Please select a valid decision and ensure required files are uploaded.");
            }

            // If validation fails or there's an error, return to the same view
            return View();
        }

        public async Task<IActionResult> GetCompletionReport(string urecNo)
        {
            var report = await _allServices.GetCompletionReportByUrecNoAsync(urecNo);

            if (report == null)
            {
                return NotFound();
            }

            var viewModel = new CompletionReportViewModel
            {
                NonFundedResearchInfo = report.EthicsApplication.NonFundedResearchInfo,
                CoProponent = report.EthicsApplication.NonFundedResearchInfo.CoProponents,
                EthicsApplication = report.EthicsApplication,
                EthicsApplicationLog = report.EthicsApplication.EthicsApplicationLogs,
                CompletionReport = report,
                CompletionCertificate = report.EthicsApplication.CompletionCertificate
            };

            return View(viewModel);
        }

        public async Task<IActionResult> DownloadTerminalReport(string urecNo)
        {
            // Fetch the terminal report as a byte array (PDF format) from your database or file storage
            var reportBytes = await _allServices.GetTerminalReportAsync(urecNo);

            if (reportBytes == null)
            {
                return NotFound();
            }

            return File(reportBytes, "application/pdf");
        }

        [Authorize(Roles = "CRE Chief")]
         
        [HttpPost]
        public async Task<IActionResult> IssueCompletionCertificate(string urecNo, IFormFile certificateFile)
        {
            // Validate the UREC number
            if (string.IsNullOrEmpty(urecNo))
            {
                TempData["ErrorMessage"] = "UREC number is required.";
                return RedirectToAction("CompletionReports", "Chief");
            }

            // Validate the certificate file
            if (certificateFile == null || certificateFile.Length == 0)
            {
                TempData["ErrorMessage"] = "A certificate file is required.";
                return RedirectToAction("CompletionReports", "Chief");
            }

            try
            {
                // Fetch the CompletionReport based on the UREC number
                var completionReport = await _allServices.GetCompletionReportByUrecNoAsync(urecNo);

                if (completionReport == null)
                {
                    TempData["ErrorMessage"] = "Completion report not found.";
                    return RedirectToAction("CompletionReports", "Chief");
                }

                // Handle the file upload: Convert the IFormFile to a byte array for storage
                byte[] certificateBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await certificateFile.CopyToAsync(memoryStream);
                    certificateBytes = memoryStream.ToArray();
                }

                // Create a new CompletionCertificate or update the existing one
                var completionCertificate = new CompletionCertificate
                {
                    UrecNo = urecNo,
                    CertificateFile = certificateBytes, // Store the byte array of the certificate
                    IssuedDate = DateOnly.FromDateTime(DateTime.Now) // Set the issued date to today
                };

                // Save the certificate to the database
                _context.CRE_CompletionCertificates.Add(completionCertificate);
                await _context.SaveChangesAsync();  // Save to get the Id of the new CompletionCertificate

                // Now update the corresponding NonFundedResearchInfo
                var nonFundedResearchInfo = await _context.CRE_NonFundedResearchInfo
                                                          .FirstOrDefaultAsync(nf => nf.UrecNo == urecNo);

                if (nonFundedResearchInfo != null)
                {
                    // Set the CompletionCertId in NonFundedResearchInfo
                    nonFundedResearchInfo.CompletionCertId = completionCertificate.CompletionCertId;

                    // Save the updated NonFundedResearchInfo record
                    await _context.SaveChangesAsync();
                }

                // Add a log entry for this action
                var log = new EthicsApplicationLogs
                {
                    UrecNo = urecNo,
                    Status = "Completion Certificate Issued",
                    ChangeDate = DateTime.Now,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Comments = "Completion certificate has been issued."
                };

                _context.CRE_EthicsApplicationLogs.Add(log); // Add the log entry


                var application = await _context.CRE_EthicsApplication
                  .Include(app => app.NonFundedResearchInfo)
                  .FirstOrDefaultAsync(app => app.UrecNo == urecNo);

                if (application == null)
                {
                    return NotFound("Application not found.");
                }

                var user = await _userManager.FindByIdAsync(application.NonFundedResearchInfo.UserId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                string recipientName = application.NonFundedResearchInfo.Name;
                string userEmail = user.Email;
                string subject = "Completion Certificate Issued";
                string body = $@"
                    <p>Congratulations!</p>
                    <p>We are pleased to inform you that your completion certificate for Ethics Application (UrecNo: <strong>{urecNo}</strong>) has been successfully issued.</p>
                    <p>This marks an important milestone in your academic journey. Please find your certificate attached to this email for your records.</p>
                    <p>We commend your hard work and dedication. Best wishes for your continued success!</p>";


                string fileName = "CompletionCertificate.pdf";

                // MimeKit parts to send the email with attachment
                var mimeAttachments = new List<MimeKit.MimePart>
                {
                    new MimeKit.MimePart("application", "pdf")
                    {
                        Content = new MimeKit.MimeContent(new MemoryStream(certificateBytes)),
                        FileName = fileName
                    }
                };

                // Call the service to send the email with the certificate attached
                await _emailService.SendEmailWithAttachmentsAsync(userEmail, recipientName, subject, body, mimeAttachments);
                string userRole = await GetUserRole(application.NonFundedResearchInfo.UserId);

                var notification = new EthicsNotifications
                {
                    UrecNo = urecNo,
                    UserId = application.NonFundedResearchInfo.UserId,
                    NotificationTitle = "Completion Certificate Issued",
                    NotificationMessage = $"Your Completion Certificate of Ethics Application (UrecNo: {urecNo}) has been issued.",
                    NotificationCreationDate = DateTime.Now,
                    NotificationStatus = false, // Notification is not read yet
                    Role = userRole,
                    PerformedBy = "System"
                };

                await _context.CRE_EthicsNotifications.AddAsync(notification);
                await _context.SaveChangesAsync();

                // Success message
                TempData["SuccessMessage"] = "Completion certificate issued and email notification sent successfully.";
                return RedirectToAction("CompletionReports", "Chief");
            }
            catch (Exception ex)
            {
                // Log the exception (optional) and show an error message
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("CompletionReports", "Chief");
            }
        }


        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public async Task<IActionResult> CompletionReports()
        {
            // Fetch the completion reports from the service
            var completionReports = await _allServices.GetCompletionReportsAsync();

            // Convert to ViewModel
            var viewModel = completionReports.Select(cr => new CompletionReportViewModel
            {
                NonFundedResearchInfo = cr.NonFundedResearchInfo,
                CoProponent = cr.NonFundedResearchInfo?.CoProponents ?? Enumerable.Empty<CoProponent>(), // Fallback to empty list
                EthicsApplication = cr.EthicsApplication,
                CompletionReport = cr.CompletionReport,
                CompletionCertificate = cr.CompletionCertificate,
                EthicsApplicationLog = cr.EthicsApplicationLog ?? Enumerable.Empty<EthicsApplicationLogs>() // Fallback to empty logs
            }).ToList();

            return View(viewModel);
        }

        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public async Task<IActionResult> Reports()
        {
            var reports = await _context.CRE_EthicsReport
                .Where(r => !r.IsArchived)
                .ToListAsync();
            return View(reports);
        }

        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public IActionResult ReportGeneration()
        {
            var viewModel = new ReportGenerationViewModel
            {
                SelectedCollege = null,
                SelectedCampus = null,
                InternalResearcherType = null
            };
            return View(viewModel);
        }
     

        [Authorize(Roles = "CRE Chief")]
        [HttpPost]
        public async Task<IActionResult> ReportGeneration(ReportGenerationViewModel model)
        {

            DateTime? startDate = model.StartDate;
            DateTime? endDate = model.EndDate;

            var researchData = await _allServices.GetFilteredResearchData(model);

            string reportType = model.SelectedReportType;
            // Generate the file contents (Excel file)
            var fileContents = _allServices.GenerateExcelFile(researchData, startDate, endDate, reportType, out string fileName);

            // Save the report to the database
            var report = new EthicsReport
            {
                EthicsReportId = Guid.NewGuid().ToString(),  // Generate a new unique ID
                ReportName = fileName,      // Use the simplified name
                ReportFileType = "Excel",                    // File type (Excel or others)
                ReportFile = fileContents,
                ReportStartDate = startDate.Value,
                ReportEndDate = endDate.Value,
                College = model.SelectedCollege ?? "No College Selected",  // Use full college name here (or "For External Researcher Applications")
                DateGenerated = DateTime.Now,
                IsArchived = false  // Default to false
            };

            // Save report to the database (assumes your database context is named _context)
            _context.CRE_EthicsReport.Add(report);
            _context.SaveChanges();

            // Add a success message to TempData
            TempData["SuccessMessage"] = "Report generated successfully!";
            // Redirect to the Reports view after the file is generated
            return RedirectToAction("Reports", "Chief");
        }

   
        [Authorize(Roles = "CRE Chief")]
         
        [HttpPost]
        public IActionResult DeleteReport(string reportId)
        {
            var report = _context.CRE_EthicsReport.FirstOrDefault(r => r.EthicsReportId == reportId);
            if (report == null)
            {
                TempData["ErrorMessage"] = "Report not found.";
                return RedirectToAction("Reports", "Chief");
            }

            // Delete the report from the database
            _context.CRE_EthicsReport.Remove(report);
            _context.SaveChanges();

            // Add a success message to TempData
            TempData["SuccessMessage"] = "Report deleted successfully!";
            return RedirectToAction("Reports", "Chief");
        }

        public IActionResult SubmitReport(string reportId)
        {
            // Add a message to TempData indicating that this feature is not yet implemented
            TempData["InfoMessage"] = "This feature is not yet implemented.";

            // Redirect to the Reports view
            return RedirectToAction("Reports", "Chief");
        }
        public IActionResult DownloadReport(string reportId)
        {
            // Set EPPlus License context to NonCommercial (free version) for this action
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            if (string.IsNullOrEmpty(reportId))
            {
                TempData["ErrorMessage"] = "Report not found.";
                return RedirectToAction("Reports", "Chief");
            }

            // Retrieve the report from the database using the reportId
            var report = _context.CRE_EthicsReport.FirstOrDefault(r => r.EthicsReportId == reportId);
            if (report == null)
            {
                TempData["ErrorMessage"] = "Report not found.";
                return RedirectToAction("Reports", "Chief");
            }

            // Ensure the report file is not null
            if (report.ReportFile == null || report.ReportFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Report file is empty.";
                return RedirectToAction("Reports", "Chief");
            }

            // Ensure the file is downloaded as .xlsx
            string fileName = Path.ChangeExtension(report.ReportName, ".xlsx");

            // Return the file as a downloadable Excel file with the .xlsx extension
            return File(report.ReportFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        public IActionResult ViewEthicsEvaluators(string sortColumn = "Completed", string sortDirection = "asc")
        {
            // Fetch all the Ethics Evaluators from the database
            var evaluators = _context.CRE_EthicsEvaluator.AsQueryable();

            // Sort based on the specified column and direction
            switch (sortColumn.ToLower())
            {
                case "completed":
                    evaluators = sortDirection == "asc" ? evaluators.OrderBy(e => e.Completed) : evaluators.OrderByDescending(e => e.Completed);
                    break;
                case "pending":
                    evaluators = sortDirection == "asc" ? evaluators.OrderBy(e => e.Pending) : evaluators.OrderByDescending(e => e.Pending);
                    break;
                case "declined":
                    evaluators = sortDirection == "asc" ? evaluators.OrderBy(e => e.Declined) : evaluators.OrderByDescending(e => e.Declined);
                    break;
                default:
                    evaluators = evaluators.OrderBy(e => e.Completed); // Default sort by Completed
                    break;
            }

            // Return the sorted list to the view
            return View(evaluators.ToList());
        }

        public IActionResult ViewChairperson()
        {
            // Fetch all chairpersons from the database
            var chairpersons = _context.CRE_Chairperson.ToList();

            // Pass the data to the view
            return View(chairpersons);
        }
        public async Task<IActionResult> AddEvaluators()
        {
            // Get the "Ethics Evaluator" role
            var role = await _roleManager.FindByNameAsync("Ethics Evaluator");
            if (role == null)
            {
                TempData["ErrorMessage"] = "No Ethics Evaluator role found.";
                return RedirectToAction("ViewEthicsEvaluators");
            }

            // Get users in the "Ethics Evaluator" role
            var evaluatorUsers = await _userManager.GetUsersInRoleAsync("Ethics Evaluator");

            // Filter out users who already have a record in the EthicsEvaluator table
            var existingEvaluatorUserIds = _context.CRE_EthicsEvaluator
                .Select(e => e.UserID)
                .ToHashSet(); // Efficient for lookup

            // Select only users not already in the EthicsEvaluator table
            var newEvaluators = evaluatorUsers
                .Where(u => !existingEvaluatorUserIds.Contains(u.Id))
                .Select(u => new AddEvaluatorViewModel
                {
                    UserID = u.Id,
                    Name = $"{u.UserName}", // Adjust based on your IdentityUser properties
                    Email = u.Email,       // Adjust as needed
                })
                .ToList();

            return View(newEvaluators);
        }

        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public async Task<IActionResult> Dashboard(int? selectedYear, int? selectedMonth)
        {

            // Get the current user
            var currentUser = await _userManager.GetUserAsync(User);

            // Concatenate the user's first name and last name
            var chiefName = $"{currentUser.FirstName} {currentUser.LastName}";

            // Use the current year and month as fallback if no year or month is provided
            int year = selectedYear ?? DateTime.Now.Year;
            int month = selectedMonth ?? DateTime.Now.Month;

            // Get the total number of applications with logs
            var totalApplications = _context.CRE_EthicsApplication
                .Where(application => application.EthicsApplicationLogs.Any()) // Only count applications with logs
                .Count();

            // Get the total number of applications with logs for the selected year and month
            var applicationsPerMonth = _context.CRE_EthicsApplication
                .Where(application => application.EthicsApplicationLogs.Any()) // Only count applications with logs
                .Where(application => application.SubmissionDate.Year == year && application.SubmissionDate.Month == month) // Filter by year and month
                .Count();

            var totalApplicationsForYear = _context.CRE_EthicsApplication
              .Where(app => app.SubmissionDate.Year == year)
              .Count();
            // Fetch the number of applications per month for the selected year (for chart data)
            var applicationsPerMonthByYear = _context.CRE_EthicsApplication
              .Where(application => application.EthicsApplicationLogs.Any()) // Only count applications with logs
              .GroupBy(application => new { Year = application.SubmissionDate.Year, Month = application.SubmissionDate.Month }) // Group by Year and Month
              .Select(group => new ApplicationsPerMonth
              {
                  Year = group.Key.Year,
                  Month = group.Key.Month,
                  ApplicationCount = group.Count() // Count the number of applications per group
              })
              .OrderBy(group => group.Year)
              .ThenBy(group => group.Month)
              .ToList();

            // Get the total number of ethics clearances for the selected year and month
            var totalClearancesIssued = _context.CRE_EthicsClearance
                .Where(clearance => clearance.IssuedDate.HasValue && clearance.IssuedDate.Value.Year == year)
                .Count();

            // Get the total number of terminal reports for the selected year and month
            var totalTerminalReports = _context.CRE_CompletionReports
                .Where(report => report.SubmissionDate.Year == year)
                .Count();

            // Get the total number of certificates issued for the selected year and month
            var totalCertificatesIssued = _context.CRE_CompletionCertificates
                .Where(cert => cert.IssuedDate.Year == year)
                .Count();

            // Fetch the top performing fields of study for the selected year and month
            var topFields = _context.CRE_EthicsApplication
              .Where(application => application.SubmissionDate.Year == year) // Only filter by year
              .GroupBy(application => application.FieldOfStudy)
              .Select(group => new
              {
                  FieldOfStudy = group.Key,
                  ApplicationCount = group.Count()
              })
              .OrderByDescending(x => x.ApplicationCount) // Order by most applications
              .Take(3) // Get the top 3 fields
              .ToList();


            var bestPerformingEvaluators = _context.CRE_EthicsEvaluator
               .Where(evaluator => evaluator.AccountStatus == "Active").ToList()
               .OrderByDescending(evaluator => evaluator.Completed) // Order by most completed evaluations
               .Take(3) // Get the top 3 evaluators
               .Select((evaluator, index) => new BestEvaluatorViewModel
               {
                   EvaluatorName = evaluator.Name,
                   CompletedCount = evaluator.Completed,
                   Rank = index + 1
               })
               .ToList();
            // Prepare the model to pass to the view
            var model = new ChiefDashboardViewModel
            { 
                ChiefName = chiefName,
                TotalApplications = totalApplications,
                ApplicationsPerMonth = applicationsPerMonth,
                ApplicationsPerMonthByYear = applicationsPerMonthByYear,
                TotalApplicationsForYear = totalApplicationsForYear,
                TotalClearancesIssued = totalClearancesIssued,
                TotalTerminalReports = totalTerminalReports,
                TotalCertificatesIssued = totalCertificatesIssued,
                BestPerformingEvaluators = bestPerformingEvaluators,
                TopFields = topFields.Select((field, index) => new TopFieldViewModel
                {
                    FieldName = field.FieldOfStudy,
                    ApplicationCount = field.ApplicationCount,
                    Rank = index + 1
                }).ToList(),
                SelectedYear = year,
                SelectedMonth = month, // Add selected month to the model
                AvailableYears = _context.CRE_EthicsApplication
                    .Select(application => application.SubmissionDate.Year)
                    .Distinct()
                    .OrderBy(year => year)
                    .ToList(),
                AvailableMonths = Enumerable.Range(1, 12)
                    .Select(m => new MonthViewModel
                    {
                        Month = m,
                        MonthName = new DateTime(2000, m, 1).ToString("MMMM")
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetCalendarEvents()
        {
            var events = await _crdlDbContext.ResearchEvent
                .Where(e => !e.IsArchived)
                .ToListAsync();

            System.Diagnostics.Debug.WriteLine($"Found {events.Count} events");

            var formattedEvents = events.Select(e => new
            {
                id = e.ResearchEventId,
                title = e.EventName,
                start = e.EventDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = e.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                description = e.EventDescription,
                location = e.EventLocation,
                eventType = e.EventType,
                backgroundColor = GetEventTypeColor(e.EventType)
            }).ToList();

            return Json(formattedEvents);
        }
        private string GetEventTypeColor(string eventType)
        {
            return eventType.ToLower() switch
            {
                "workshop" => "#4e73df",    // Blue
                "seminar" => "#1cc88a",     // Green
                "publication" => "#f6c23e",  // Yellow
                _ => "#e74a3b"              // Red for Others
            };
        }            //ITO LANG PWEDE MONG GALAWIN

        public async Task<IActionResult> EvaluateApplicationPdfGen(string UrecNo)
        {
            var baseModel = _context.CRE_EthicsApplication
                      .Include(e => e.NonFundedResearchInfo)
                         .ThenInclude(nf => nf.CoProponents)
                      .Include(e => e.InitialReview)
                      .Include(e => e.EthicsEvaluation)
                         .ThenInclude(e => e.EthicsEvaluator)
                      .Include(e => e.EthicsApplicationForms)
                      .Include(e => e.EthicsApplicationLogs)
                      .Where(e => e.UrecNo == UrecNo)
                      .FirstOrDefault();

            if (baseModel == null)
            {
                return NotFound();
            }

            // Get the current user's ID (logged-in user)
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Assuming you're using Claims-based authentication

            // Fetch the current user's record
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            // Concatenate the full name with middle initial
            var middleInitial = string.IsNullOrEmpty(currentUser.MiddleName) ? "" : $"{currentUser.MiddleName[0]}.";
            var fullName = $"{currentUser.FirstName} {middleInitial} {currentUser.LastName}".Trim();

            var ethicsEvaluation = baseModel.EthicsEvaluation.FirstOrDefault();
            // Check if EthicsEvaluation already exists
            if (ethicsEvaluation == null)
            {
                // Create new EthicsEvaluation if it doesn't exist
                ethicsEvaluation = new EthicsEvaluation
                {
                    Name = fullName, // Set the concatenated full name
                    UrecNo = UrecNo,
                    EvaluationStatus = "Pending",

                };

                // Save the new evaluation to the database
                _context.CRE_EthicsApplication.Update(baseModel);
                await _context.SaveChangesAsync();
            }
            else
            {
                // If EthicsEvaluation exists, just set the name
                ethicsEvaluation.Name = fullName;
            }

            var evaluatorUserIds = baseModel.EthicsEvaluation
                .Select(e => e.UserId) // Assuming 'UserId' is what links to the 'EthicsEvaluator'
                .Distinct()
                .ToList();

            // Fetch evaluators based on the user IDs retrieved
            var ethicsEvaluators = await _context.CRE_EthicsEvaluator
                .Where(e => evaluatorUserIds.Contains(e.UserID)) // Match user IDs
                .ToListAsync();

            var evalSheets = new EvaluationSheetsViewModel
            {
                InformedConsentForm = new InformedConsentFormViewModel
                {
                    EthicsApplication = baseModel,
                    NonFundedResearchInfo = baseModel.NonFundedResearchInfo,
                    CoProponents = baseModel.NonFundedResearchInfo.CoProponents.ToList(),
                    InitialReview = baseModel.InitialReview,
                    EthicsEvaluation = ethicsEvaluation,  // Only one evaluation
                    EthicsApplicationForms = baseModel.EthicsApplicationForms.ToList(),
                    EthicsApplicationLog = baseModel.EthicsApplicationLogs.ToList(),
                    ReceiptInfo = baseModel.ReceiptInfo
                },
                ProtocolReviewForm = new ProtocolReviewFormViewModel
                {
                    EthicsApplication = baseModel,
                    NonFundedResearchInfo = baseModel.NonFundedResearchInfo,
                    CoProponents = baseModel.NonFundedResearchInfo.CoProponents.ToList(),
                    InitialReview = baseModel.InitialReview,
                    EthicsEvaluation = ethicsEvaluation,  // Only one evaluation
                    EthicsApplicationForms = baseModel.EthicsApplicationForms.ToList(),
                    EthicsApplicationLog = baseModel.EthicsApplicationLogs.ToList(),
                    ReceiptInfo = baseModel.ReceiptInfo
                }
            };

            return View(evalSheets);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EvaluateApplicationPdfGen(EvaluationSheetsViewModel model, string action)
        {
          

            var coProponents = _context.CRE_NonFundedResearchInfo
                   .Include(nf => nf.CoProponents)
                   .Where(e => e.UrecNo == model.InformedConsentForm.EthicsApplication.UrecNo)
                   .FirstOrDefault();

            model.InformedConsentForm.CoProponents = coProponents?.CoProponents.ToList(); // Populate for InformedConsentForm
            model.ProtocolReviewForm.CoProponents = coProponents?.CoProponents.ToList(); // Populate for ProtocolReviewForm

            var informedConsentPdf = await _pdfGenerationService.GenerateInformedConsentPdf(model.InformedConsentForm);
            var protocolReviewPdf = await _pdfGenerationService.GenerateProtocolReviewPdf(model.ProtocolReviewForm);
            // For Protocol Review Form
            if (action == "Preview")
            {
                using (var zipMemoryStream = new MemoryStream())
                {
                    using (var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, true))
                    {
                        // Add the Informed Consent PDF to the ZIP file
                        var informedConsentEntry = zipArchive.CreateEntry("InformedConsent.pdf");
                        using (var entryStream = informedConsentEntry.Open())
                        {
                            // Convert the byte[] to a MemoryStream and copy the content
                            using (var pdfStream = new MemoryStream(informedConsentPdf))
                            {
                                await pdfStream.CopyToAsync(entryStream);
                            }
                        }

                        // Add the Protocol Review PDF to the ZIP file
                        var protocolReviewEntry = zipArchive.CreateEntry("ProtocolReview.pdf");
                        using (var entryStream = protocolReviewEntry.Open())
                        {
                            // Convert the byte[] to a MemoryStream and copy the content
                            using (var pdfStream = new MemoryStream(protocolReviewPdf))
                            {
                                await pdfStream.CopyToAsync(entryStream);
                            }
                        }
                    }

                    // Return the ZIP file as a downloadable file
                    return File(zipMemoryStream.ToArray(), "application/zip", "EvaluationSheets.zip");
                }
            }
            var protocolRecommendationQuestion = GetQuestionByKeyword(model.ProtocolReviewForm.Questions, "Recommendation");
            var protocolRemarksQuestion = GetQuestionByKeyword(model.ProtocolReviewForm.Questions, "Remarks");

            // Get the answer for Protocol Recommendation (which is MultiDropDown)
            var protocolRecommendation = protocolRecommendationQuestion?.Answer != null
                ? string.Join(", ", protocolRecommendationQuestion.Answer)  // Join the selected values for MultiDropDown
                : "No Recommendation";

            // Get the FollowUpAnswer for Protocol Remarks
            var protocolRemarks = protocolRemarksQuestion?.Answer ?? "No Remarks";

            // For Informed Consent Form
            var consentRecommendationQuestion = GetQuestionByKeyword(model.InformedConsentForm.Questions, "Recommendation");
            var consentRemarksQuestion = GetQuestionByKeyword(model.InformedConsentForm.Questions, "Remarks");

            // Get the answer for Consent Recommendation (which is MultiDropDown)
            var consentRecommendation = consentRecommendationQuestion?.Answer != null
                ? string.Join(", ", consentRecommendationQuestion.Answer)  // Join the selected values for MultiDropDown
                : "No Recommendation";

            // Get the FollowUpAnswer for Consent Remarks
            var consentRemarks = consentRemarksQuestion?.Answer ?? "No Remarks";

            // Fetch the application based on the UrecNo from the model
            var application = await _context.CRE_EthicsApplication
                .Include(a => a.EthicsEvaluation)
                .FirstOrDefaultAsync(a => a.UrecNo == model.InformedConsentForm.EthicsApplication.UrecNo);

            if (application == null)
            {
                return NotFound();
            }
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the current user details using UserManager
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (currentUser == null)
            {
                return NotFound("User not found.");
            }

            var fullName = currentUser.FirstName;

            if (!string.IsNullOrEmpty(currentUser.MiddleName))
            {
                fullName += " " + currentUser.MiddleName[0] + ".";
            }

            fullName += " " + currentUser.LastName;

            var ethicsEvaluation = new EthicsEvaluation
            {
                UrecNo = model.InformedConsentForm.EthicsApplication.UrecNo,
                UserId = currentUserId,
                Name = fullName,
                EvaluationStatus = "Evaluated",
                ProtocolRecommendation = protocolRecommendation,
                ProtocolRemarks = protocolRemarks,
                ConsentRecommendation = consentRecommendation,
                ConsentRemarks = consentRemarks,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                ProtocolReviewSheet = protocolReviewPdf, // The byte array for the informed consent PDF
                InformedConsentForm = informedConsentPdf // The byte array for the protocol review PDF
            };
            _context.CRE_EthicsEvaluation.Add(ethicsEvaluation);

            var applicationLog = new EthicsApplicationLogs
            {
                UrecNo = model.InformedConsentForm.EthicsApplication.UrecNo,
                UserId = currentUserId,
                Status = "Evaluated",
                ChangeDate = DateTime.Now,
                Comments = "The application has been evaluated."
            };
            _context.CRE_EthicsApplicationLogs.Add(applicationLog);

            if (application == null)
            {
                return NotFound("Application not found.");
            }

            var user = await _userManager.FindByIdAsync(model.InformedConsentForm.NonFundedResearchInfo.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            string recipientName = model.InformedConsentForm.NonFundedResearchInfo.Name;
            string userEmail = user.Email;
            string subject = "Your Ethics Application Has Been Evaluated";
            string body = $@"
                <p>We are pleased to inform you that the Ethics Application (UrecNo: <strong>{model.InformedConsentForm.EthicsApplication.UrecNo}</strong>) has been evaluated.</p>
                <p>Your submission has been reviewed, and the process will continue accordingly.</p>
                <p>If any further steps are required, you will be notified accordingly. Please stay tuned for further updates.</p>
                <p>Thank you for your submission and cooperation.</p>";
            string userRole = await GetUserRole(model.InformedConsentForm.NonFundedResearchInfo.UserId);
            await _emailService.SendEmailAsync(userEmail, recipientName, subject, body);
            var notification = new EthicsNotifications
            {
                UrecNo = model.InformedConsentForm.EthicsApplication.UrecNo,
                UserId = model.InformedConsentForm.NonFundedResearchInfo.UserId,
                NotificationTitle = "Application Evaluated",
                NotificationMessage = $"Your Ethics Application (UrecNo: {model.InformedConsentForm.EthicsApplication.UrecNo}) has been evaluated and processed.",
                NotificationCreationDate = DateTime.Now,
                NotificationStatus = false,
                Role = userRole,
                PerformedBy = "System"
            };

            // Add notification to the database
            await _context.CRE_EthicsNotifications.AddAsync(notification);

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Evaluation successful.";

            return RedirectToAction("PendingExemptEvaluations", new { success = true });
            
        }
        private Question GetQuestionByKeyword(List<Question> questions, string keyword)
        {
            return questions.FirstOrDefault(q => q.QuestionText.Contains(keyword));
        }

        [Route("Chief/GenerateClearancePDF/{urecNo}")]
        [HttpGet]
        public async Task<IActionResult> GenerateClearancePDF(string urecNo, string remarks)
        {
            // Fetch the data including related records
            var ethicsApplication = await _context.CRE_EthicsApplication
                .Include(e => e.InitialReview)
                .Include(e => e.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponents)
                .FirstOrDefaultAsync(e => e.UrecNo == urecNo);

            var currentUser = await _userManager.GetUserAsync(User); // User is the current logged-in user
            string middleInitial = !string.IsNullOrEmpty(currentUser.MiddleName) ? currentUser.MiddleName[0].ToString() + "." : string.Empty;

            // Concatenate the first name, middle initial, and last name
            string chiefName = $"{currentUser.FirstName} {middleInitial} {currentUser.LastName}";

            if (ethicsApplication == null)
            {
                return NotFound();
            }

            // Populate the ViewModel
            var viewModel = new EthicsClearanceViewModel
            {
                EthicsApplication = ethicsApplication,
                InitialReview = ethicsApplication.InitialReview,
                NonFundedResearchInfo = ethicsApplication.NonFundedResearchInfo,
                CoProponents = ethicsApplication.NonFundedResearchInfo.CoProponents,
                ChiefName = chiefName
            };

            return View(viewModel);
        }


        [Route("Chief/GenerateClearancePDF/{urecNo}")]
        [HttpPost]
        public async Task<IActionResult> GenerateClearancePDF(string urecNo, EthicsClearanceViewModel viewModel)
        {
            // Fetch the data including related records
            var ethicsApplication = await _context.CRE_EthicsApplication
                .Include(e => e.InitialReview)
                .Include(e => e.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponents)
                .FirstOrDefaultAsync(e => e.UrecNo == urecNo);


            viewModel.EthicsApplication = ethicsApplication;
            viewModel.InitialReview = ethicsApplication.InitialReview;
            viewModel.NonFundedResearchInfo = ethicsApplication.NonFundedResearchInfo;
            viewModel.CoProponents = ethicsApplication.NonFundedResearchInfo.CoProponents;
                
            // Ensure the data is available in the view model
            if (viewModel == null || viewModel.EthicsApplication == null)
            {
                return BadRequest("Invalid data");
            }

            // Call the service to generate the PDF based on the data from the view model
            var pdfBytes = await _pdfGenerationService.GenerateClearancePdf(viewModel);

            if (pdfBytes == null)
            {
                return BadRequest("PDF generation failed");
            }

            // Return the generated PDF as a file download
            return File(pdfBytes, "application/pdf", "Ethics_Clearance.pdf");
        }

        [HttpPost]
        public async Task<IActionResult> SendClearanceToResearcher(string urecNo, EthicsClearanceViewModel viewModel)
        {
            // Retrieve the current user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the application and related data for the ViewModel
            var application = await _context.CRE_EthicsApplication
                .Include(app => app.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponents)
                .Include(app => app.InitialReview)
                .FirstOrDefaultAsync(app => app.UrecNo == urecNo);
            var currentUser = await _userManager.GetUserAsync(User); // User is the current logged-in user
            string middleInitial = !string.IsNullOrEmpty(currentUser.MiddleName) ? currentUser.MiddleName[0].ToString() + "." : string.Empty;

            // Concatenate the first name, middle initial, and last name
            string chiefName = $"{currentUser.FirstName} {middleInitial} {currentUser.LastName}";
            string recipientName = application.NonFundedResearchInfo.Name;
            var user = await _userManager.FindByIdAsync(application.NonFundedResearchInfo.UserId);  
            string userEmail = user.Email;
            if (application == null)
            {
                ModelState.AddModelError("", "Application not found.");
                return View();
            }
            // Populate the viewModel with all necessary information
            viewModel.EthicsApplication = application;
            viewModel.InitialReview = application.InitialReview;
            viewModel.NonFundedResearchInfo = application.NonFundedResearchInfo;
            viewModel.CoProponents = application.NonFundedResearchInfo.CoProponents;
            viewModel.ChiefName = chiefName;
            // Generate the PDF using the existing service
            byte[] pdfData = await _pdfGenerationService.GenerateClearancePdf(viewModel);

            // Save the generated PDF in the database
            var ethicsClearance = new EthicsClearance
            {
                UrecNo = urecNo,
                IssuedDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddYears(1),
                ClearanceFile = pdfData, // Save the generated PDF
            };

            var fre = await _remcDbContext.REMC_FundedResearchEthics.FirstOrDefaultAsync(f => f.urecNo == urecNo);
            if (fre != null)
            {
                fre.clearanceFile = pdfData;
                fre.file_Name = $"{urecNo}_EthicsClearance.pdf";
                fre.file_Type = ".pdf";
                fre.file_Status = "Checked";
                fre.file_Uploaded = DateTime.Now;

                var fra = await _remcDbContext.REMC_FundedResearchApplication.FirstAsync(f => f.fra_Id == fre.fra_Id);
                await _actionLogger.LogActionAsync(fra.applicant_Name, fra.fra_Type, $"{fra.research_Title} already has Ethics Clearance", true, true, false, fra.fra_Id);
            }

            _context.CRE_EthicsClearance.Add(ethicsClearance);
            await _context.SaveChangesAsync();

            var nonFundedResearchInfo = await _context.CRE_NonFundedResearchInfo
                                     .FirstOrDefaultAsync(nf => nf.UrecNo == urecNo);

            if (nonFundedResearchInfo != null)
            {
                // Set the EthicsClearanceId in NonFundedResearchInfo
                nonFundedResearchInfo.EthicsClearanceId = ethicsClearance.EthicsClearanceId;

                // Save the updated NonFundedResearchInfo record
                await _context.SaveChangesAsync();
            }
            // Log the clearance issuance with hardcoded remarks
            var applicationLog = new EthicsApplicationLogs
            {
                UrecNo = urecNo,
                Status = "Clearance Issued",
                Comments = "Ethics Clearance Issued",
                ChangeDate = DateTime.Now,
                UserId = userId,
            };

            _context.CRE_EthicsApplicationLogs.Add(applicationLog);
            await _context.SaveChangesAsync();
            string subject = "Ethics Clearance Issued";
            string body = $@"
            <p>We are pleased to inform you that your Ethics Application (UrecNo: <strong>{urecNo}</strong>) has been approved.</p>
            <p>The clearance is valid from <strong>{ethicsClearance.IssuedDate:MMMM dd, yyyy}</strong> to <strong>{ethicsClearance.ExpirationDate:MMMM dd, yyyy}</strong>.</p>
            <p>Attached is your clearance file for reference.</p>
            <p>Thank you!</p>";

            var attachments = new List<Attachment>();

            // List of MimePart attachments
            List<MimeKit.MimePart> mimeAttachments = new List<MimeKit.MimePart>();

            // Add the file as an attachment if available
            if (ethicsClearance.ClearanceFile != null && ethicsClearance.ClearanceFile.Length > 0)
            {
                var mimePart = new MimePart("application", "pdf")
                {
                    Content = new MimeContent(new MemoryStream(ethicsClearance.ClearanceFile)),
                    FileName = "EthicsClearance.pdf"
                };

                // Add MimePart to the list of mimeAttachments
                mimeAttachments.Add(mimePart);
            }

            // Call the service to send email with attachments
            await _emailService.SendEmailWithAttachmentsAsync(userEmail, recipientName, subject, body, mimeAttachments);

            // Add a notification for clearance issuance
            string userRole = await GetUserRole(application.NonFundedResearchInfo.UserId);
            var notification = new EthicsNotifications
            {
                UrecNo = urecNo,
                UserId = application.NonFundedResearchInfo.UserId,
                NotificationTitle = "Ethics Clearance Issued",
                NotificationMessage = $"Your Ethics Application (UrecNo: {urecNo}) has been approved. Check your email for the clearance file.",
                NotificationCreationDate = DateTime.Now,
                NotificationStatus = false,
                Role = userRole,
                PerformedBy = "System",
            };

            _context.CRE_EthicsNotifications.Add(notification);
            await _context.SaveChangesAsync();
            // Add additional actions like notifications or emails
            TempData["SuccessMessage"] = "Ethics clearance issued and PDF generated successfully!";
            return RedirectToAction("PendingExemptEvaluations");
        }

        [Authorize(Roles = "CRE Chief")]
        public async Task<IActionResult> EditEvaluator(int id)
        {
            // Fetch the evaluator by their ID
            var evaluator = await _context.CRE_EthicsEvaluator
                .FirstOrDefaultAsync(e => e.EthicsEvaluatorId == id);

            if (evaluator == null)
            {
                return NotFound();
            }

            // Fetch the evaluator's expertise (this will be a list of expertise IDs associated with the evaluator)
            var evaluatorExpertises = await _context.CRE_EthicsEvaluatorExpertise
                .Where(ee => ee.EthicsEvaluatorId == id)
                .Select(ee => ee.ExpertiseId)
                .ToListAsync();

            // Fetch all expertise data from the CRE_Expertise table
            var expertiseList = await _context.CRE_Expertise.ToListAsync();

            // Prepare the ViewModel with evaluator and expertise data
            var viewModel = new EditEvaluatorViewModel
            {
                Evaluator = evaluator,
                ExpertiseList = expertiseList,
                SelectedExpertise = evaluatorExpertises
            };

            return View(viewModel);
        }
        [Authorize(Roles = "CRE Chief")]
        [HttpPost]
        public async Task<IActionResult> EditEvaluator(int id, EditEvaluatorViewModel model)
        {
            if (id != model.Evaluator.EthicsEvaluatorId)
            {
                return NotFound();
            }

            // Fetch the evaluator and update their details
            var evaluator = await _context.CRE_EthicsEvaluator
                .FirstOrDefaultAsync(e => e.EthicsEvaluatorId == id);

            if (evaluator == null)
            {
                return NotFound();
            }

            // Remove the existing expertise associations
            var existingExpertise = await _context.CRE_EthicsEvaluatorExpertise
                .Where(ee => ee.EthicsEvaluatorId == id)
                .ToListAsync();

            _context.CRE_EthicsEvaluatorExpertise.RemoveRange(existingExpertise);

            // Add the new expertise associations
            foreach (var expertiseId in model.SelectedExpertise)
            {
                _context.CRE_EthicsEvaluatorExpertise.Add(new EthicsEvaluatorExpertise
                {
                    EthicsEvaluatorId = id,
                    ExpertiseId = expertiseId
                });
            }

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewEvaluator", new { id = id });
        }

        [Authorize(Roles = "CRE Chief")]
        public async Task<IActionResult> ViewEvaluator(int id)
        {
            // Fetch the evaluator and their expertise
            var evaluator = await _context.CRE_EthicsEvaluator
                .FirstOrDefaultAsync(e => e.EthicsEvaluatorId == id);

            if (evaluator == null)
            {
                return NotFound();
            }

            // Fetch the evaluator's expertise
            var evaluatorExpertises = await _context.CRE_EthicsEvaluatorExpertise
                .Where(ee => ee.EthicsEvaluatorId == id)
                .Select(ee => ee.ExpertiseId)
                .ToListAsync();

            var expertiseList = await _context.CRE_Expertise.ToListAsync();

            // Prepare the ViewModel with evaluator and expertise data
            var viewModel = new ViewEvaluatorViewModel
            {
                Evaluator = evaluator,
                ExpertiseList = expertiseList,
                SelectedExpertise = evaluatorExpertises
            };

            return View(viewModel);
        }

        [Authorize(Roles = "CRE Chief")]
        [HttpGet]
        public IActionResult EditChairperson(int id)
        {
            // Retrieve the chairperson from the database
            var chairperson = _context.CRE_Chairperson.FirstOrDefault(c => c.ChairpersonId == id);

            // If the chairperson doesn't exist, return not found
            if (chairperson == null)
            {
                return NotFound();
            }

            // Predefined list of fields of study
            var allFieldsOfStudy = new List<string>
    {
        "Education",
        "Computer Science, Information Systems, and Technology",
        "Engineering, Architecture, and Design",
        "Humanities, Language, and Communication",
        "Business",
        "Social Sciences",
        "Science, Mathematics, and Statistics"
    };

            // Create a view model to pass data to the view (if needed, or directly pass data)
            var viewModel = new ChairpersonEditViewModel
            {
                ChairpersonId = chairperson.ChairpersonId,
                Name = chairperson.Name,
                FieldOfStudy = chairperson.FieldOfStudy,
                AllFieldsOfStudy = allFieldsOfStudy
            };

            return View(viewModel);
        }
        [Authorize(Roles = "CRE Chief")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditChairperson(ChairpersonEditViewModel model)
        {
            // Find the chairperson to update based on ChairpersonId
            var chairperson = _context.CRE_Chairperson.FirstOrDefault(c => c.ChairpersonId == model.ChairpersonId);

            if (chairperson == null)
            {
                return NotFound();
            }

            // Update chairperson details
            chairperson.Name = model.Name;
            chairperson.FieldOfStudy = model.FieldOfStudy;

            // Save changes to the database
            _context.SaveChanges();

            // Redirect to the chairpersons view (or another page as needed)
            return RedirectToAction("ViewChairperson");
        }

        // GET: Confirm Delete
        [HttpGet]
        public IActionResult ConfirmDelete(int id)
        {
            var chairperson = _context.CRE_Chairperson.FirstOrDefault(c => c.ChairpersonId == id);

            if (chairperson == null)
            {
                return NotFound();
            }

            return View(chairperson);
        }

        // POST: Delete Chairperson
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteChairperson(int ChairpersonId)
        {
            var chairperson = _context.CRE_Chairperson.FirstOrDefault(c => c.ChairpersonId == ChairpersonId);

            if (chairperson == null)
            {
                return NotFound();
            }

            // Remove chairperson from the database
            _context.CRE_Chairperson.Remove(chairperson);
            _context.SaveChanges();

            // Redirect to the list page
            return RedirectToAction("ViewChairperson");
        }
        [Authorize(Roles = "Director")]
        [HttpGet]
        public async Task<IActionResult> GenerateReportDirector()
        {
            var reports = await _context.CRE_EthicsReport
                .Where(r => !r.IsArchived)
                .ToListAsync();
            return View(reports);
        }

        [Authorize(Roles = "Director")]
        [HttpGet]
        public IActionResult GenerateReportDirectorFilter()
        {
            var viewModel = new ReportGenerationViewModel
            {
                SelectedCollege = null,
                SelectedCampus = null,
                InternalResearcherType = null
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Director")]
        [HttpPost]
        public async Task<IActionResult> GenerateReportDirectorFilter(ReportGenerationViewModel model)
        {

            DateTime? startDate = model.StartDate;
            DateTime? endDate = model.EndDate;

            var researchData = await _allServices.GetFilteredResearchData(model);

            string reportType = model.SelectedReportType;
            // Generate the file contents (Excel file)
            var fileContents = _allServices.GenerateExcelFile(researchData, startDate, endDate, reportType, out string fileName);

            // Save the report to the database
            var report = new EthicsReport
            {
                EthicsReportId = Guid.NewGuid().ToString(),  // Generate a new unique ID
                ReportName = fileName,      // Use the simplified name
                ReportFileType = "Excel",                    // File type (Excel or others)
                ReportFile = fileContents,
                ReportStartDate = startDate.Value,
                ReportEndDate = endDate.Value,
                College = model.SelectedCollege ?? "No College Selected",  // Use full college name here (or "For External Researcher Applications")
                DateGenerated = DateTime.Now,
                IsArchived = false  // Default to false
            };

            // Save report to the database (assumes your database context is named _context)
            _context.CRE_EthicsReport.Add(report);
            _context.SaveChanges();

            // Add a success message to TempData
            TempData["SuccessMessage"] = "Report generated successfully!";

            return RedirectToAction("GenerateReportDirector", "Chief");
        }
        [Authorize(Roles = "CRE Chief")]
        public IActionResult UploadMemo()
        {
            return View();
        }

        [Authorize(Roles = "CRE Chief")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadMemo(IFormFile uploadedFile, string memoNumber, string? memoName, string memoDescription)
        {
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                ModelState.AddModelError("uploadedFile", "File is required.");
                return View();
            }

            using var memoryStream = new MemoryStream();
            await uploadedFile.CopyToAsync(memoryStream);
            var memo = new EthicsMemoranda
            {
                MemoNumber = memoNumber,
                MemoName = memoName,
                MemoDescription = memoDescription,
                MemoFile = memoryStream.ToArray() 
            };

            // Save to database
            _context.CRE_EthicsMemoranda.Add(memo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Memo uploaded successfully.";
            return RedirectToAction("ListMemos");
        }

        [Authorize(Roles = "CRE Chief")]
        public IActionResult ListMemos()
        {
            var memos = _context.CRE_EthicsMemoranda.ToList(); 
            return View(memos);
        }
        [Authorize(Roles = "CRE Chief")]
        public IActionResult EditMemo(int id)
        {
            var memo = _context.CRE_EthicsMemoranda.FirstOrDefault(m => m.MemoId == id);

            if (memo == null)
                return NotFound();

            return View(memo); // Redirect to an edit view with the memo
        }
        [Authorize(Roles = "CRE Chief")]
        [HttpPost]
        public IActionResult EditMemo(EthicsMemoranda updatedMemo, IFormFile uploadedFile)
        {
           
            var memo = _context.CRE_EthicsMemoranda.FirstOrDefault(m => m.MemoId == updatedMemo.MemoId);

            if (memo == null)
            {
                return NotFound(); // If memo is not found, return 404
            }

            // Update memo details
            memo.MemoName = updatedMemo.MemoName;
            memo.MemoDescription = updatedMemo.MemoDescription;

            // If a new file is uploaded, update the memo file
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    uploadedFile.CopyTo(memoryStream);
                    memo.MemoFile = memoryStream.ToArray(); // Store the uploaded file as a byte array
                }
            }

            _context.SaveChanges(); // Save changes to the database
            TempData["SuccessMessage"] = "Memo updated successfully!";
            return RedirectToAction("ListMemos"); // Redirect to the list of memos
            
        }



        [Authorize(Roles = "CRE Chief")]
        [HttpPost]
        public IActionResult DeleteMemo(int id)
        {
            var memo = _context.CRE_EthicsMemoranda.FirstOrDefault(m => m.MemoId == id);

            if (memo != null)
            {
                _context.CRE_EthicsMemoranda.Remove(memo);
                _context.SaveChanges();
            }

            return RedirectToAction("ListMemos");
        }

        
        public IActionResult DownloadMemo(int id)
        {
            var memo = _context.CRE_EthicsMemoranda.FirstOrDefault(m => m.MemoId == id);

            if (memo == null || memo.MemoFile == null)
            {
                return NotFound("Memo or file not found.");
            }

            // You can change the file type based on your file (e.g., PDF, DOCX, etc.)
            return File(memo.MemoFile, "application/octet-stream", $"{memo.MemoName ?? "Memo"}_{memo.MemoNumber}.pdf");
        }

    }
}
