using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using OfficeOpenXml;
using ResearchManagementSystem.Areas.CreSys.Data;
using ResearchManagementSystem.Areas.CreSys.Interfaces;
using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Areas.CreSys.ViewModels;
using ResearchManagementSystem.Areas.CreSys.ViewModels.ListViewModels;
using ResearchManagementSystem.Models;
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

        public ChiefController(CreDbContext context, UserManager<ApplicationUser> userManager, IEthicsEmailService emailService, IAllServices allServices, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _allServices = allServices;
            _roleManager = roleManager;

        }
        public IActionResult Index()
        {
            return View();
        }

        //viewing of the forms in table fomart
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
        [HttpGet]
        public IActionResult UploadForm()
        {
            return View();
        }

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


        [HttpPost]
        [ValidateAntiForgeryToken]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                .Select(app => new
                {
                    app.UrecNo,
                    app.NonFundedResearchInfo,
                    app.ReceiptInfo,
                    app.InitialReview,
                    app.EthicsApplicationForms,
                    app.EthicsApplicationLogs,
                    CoProponents = app.NonFundedResearchInfo.CoProponents.Select(cp => cp).ToList()
                })
        .FirstOrDefaultAsync();

            if (applicationDetails == null)
            {
                return NotFound();
            }

            // Map to the view model using the retrieved data
            var viewModel = new AssignReviewTypeViewModel
            {
                NonFundedResearchInfo = applicationDetails.NonFundedResearchInfo,
                CoProponent = applicationDetails.CoProponents,
                ReceiptInfo = applicationDetails.ReceiptInfo,
                EthicsApplication = new EthicsApplication
                {
                    UrecNo = applicationDetails.UrecNo,
                    InitialReview = applicationDetails.InitialReview
                },
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

            return RedirectToAction("FilteredApplications");
        }


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
                    CoProponent = a.NonFundedResearchInfo.CoProponents.Select(c => new CoProponent
                    {
                        CoProponentName = c.CoProponentName
                    }).ToList(),
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
                    CoProponent = a.NonFundedResearchInfo.CoProponents.Select(c => new CoProponent
                    {
                        CoProponentName = c.CoProponentName
                    }).ToList(),
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
                    CoProponent = a.NonFundedResearchInfo.CoProponents
                        .Select(c => new CoProponent { CoProponentName = c.CoProponentName })
                        .ToList(),  // Select only the necessary co-proponent data
                    EthicsApplicationLogs = a.EthicsApplicationLogs
                        .OrderByDescending(log => log.ChangeDate)
                        .ToList(),  // Sort logs by ChangeDate in-memory
                    InitialReview = a.InitialReview
                })
                .ToListAsync();  // Executes the query and retrieves the data

            return applications;
        }

       
        

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

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
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
                return RedirectToAction("Evaluations");
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
                return RedirectToAction("Evaluations");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    IssuedDate = DateOnly.FromDateTime(DateTime.UtcNow) // Set the issued date to today
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
                    ChangeDate = DateTime.UtcNow,
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


        public async Task<IActionResult> Reports()
        {
            var reports = await _context.CRE_EthicsReport
                .Where(r => !r.IsArchived)
                .ToListAsync();
            return View(reports);
        }

        [HttpGet]
        public IActionResult ReportGeneration()
        {
            var viewModel = new ReportGenerationViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ReportGeneration(ReportGenerationViewModel model)
        {

            DateTime? startDate = model.StartDate;
            DateTime? endDate = model.EndDate;

            var researchData = _allServices.GetFilteredResearchData(model);

            // Get the college and field of study acronyms and full names
            string collegeAcronym = string.Empty;
            string collegeFullName = string.Empty;

            // Check if the college is null or external researcher is selected
            if (model.ExternalApplications)
            {
                collegeFullName = "For External Researcher Applications";
                collegeAcronym = "ER";  // Append "ER" for External Researcher
            }
            else
            {
                if (string.IsNullOrEmpty(model.SelectedCollege) || model.SelectedCollege == "All Colleges")
                {
                    // If college is null, use campus acronyms
                    string campusAcronym = GetCampusAcronym(model.SelectedCampus);
                    collegeFullName = campusAcronym;  // Set college full name to campus acronym
                    collegeAcronym = campusAcronym;
                }
                else
                {
                    // Get full name and acronym for the selected college
                    collegeFullName = GetCollegeFullName(model.SelectedCollege);
                    collegeAcronym = GetCollegeAcronym(model.SelectedCollege);
                }
            }

            // Get field of study acronym
            string fieldAcronym = GetFieldOfStudyAcronym(model.SelectedFieldOfStudy);

            // Remove the DateTime.Now part from the file name
            string fileNameSuffix = string.Empty;

            // Add college acronym to the file name if a specific college is selected (or campus if external application)
            if (!string.IsNullOrEmpty(collegeAcronym))
            {
                fileNameSuffix += $"_{collegeAcronym}";
            }

            // Add field acronym if a specific field of study is selected
            if (model.SelectedFieldOfStudy != "All Field of Study")
            {
                fileNameSuffix += $"_{fieldAcronym}";
            }

            // Generate the file contents (Excel file)
            var fileContents = _allServices.GenerateExcelFile(researchData, startDate, endDate, out string fileName);

            // Save the report to the database
            var report = new EthicsReport
            {
                EthicsReportId = Guid.NewGuid().ToString(),  // Generate a new unique ID
                ReportName = fileName + fileNameSuffix,      // Use the simplified name
                ReportFileType = "Excel",                    // File type (Excel or others)
                ReportFile = fileContents,
                ReportStartDate = startDate.Value,
                ReportEndDate = endDate.Value,
                College = collegeFullName,  // Use full college name here (or "For External Researcher Applications")
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

        // Helper to get campus acronym from selected campus name
        private string GetCampusAcronym(string campus)
        {
            var campuses = new Dictionary<string, string>
            {
                { "Sta. Mesa (MAIN CAMPUS)", "MAIN" },
                { "Taguig City (BRANCH)", "TGG" },
                { "Quezon City (BRANCH)", "QZN" },
                { "San Juan City (BRANCH)", "SNJN" },
                { "Parañaque City (CAMPUS)", "PRNQUE" },
                { "Bataan (BRANCH)", "BTN" },
                { "Sta. Maria, Bulacan (CAMPUS)", "STMRA" },
                { "Pulilan, Bulacan (CAMPUS)", "PLLN" },
                { "Cabiao, Nueva Ecija (BRANCH)", "CBIO" },
                { "Lopez, Quezon (BRANCH)", "LPZ" },
                { "Malunay, Quezon (BRANCH)", "MLNY" },
                { "Unisan, Quezon (BRANCH)", "UNSN" },
                { "Ragay, Camarines Sur (BRANCH)", "RGY" },
                { "Sto. Tomas, Batangas (BRANCH)", "STOMS" },
                { "Maragondon, Cavite (BRANCH)", "MRGNDN" },
                { "Bansud, Oriental Mindoro (BRANCH)", "BNSD" },
                { "Sablayan, Occidental Mindoro (BRANCH)", "SBLYN" },
                { "Biñan, Laguna (CAMPUS)", "BÑN" },
                { "San Pedro, Laguna (CAMPUS)", "SNPRD" },
                { "Sta. Rosa, Laguna (CAMPUS)", "STRSA" },
                { "Calauan, Laguna (CAMPUS)", "CLUN" }
            };
            return campuses.ContainsKey(campus) ? campuses[campus] : string.Empty;
        }
        // Helper to get college acronym
        private string GetCollegeFullName(string college)
        {
            var colleges = new Dictionary<string, string>
    {
        { "All Colleges", "All Colleges" },
        { "College of Accountancy and Finance (CAF)", "College of Accountancy and Finance (CAF)" },
        { "College of Architecture, Design and the Built Environment (CADBE)", "College of Architecture, Design and the Built Environment (CADBE)" },
        { "College of Arts and Letters (CAL)", "College of Arts and Letters (CAL)" },
        { "College of Business Administration (CBA)", "College of Business Administration (CBA)" },
        { "College of Communication (COC)", "College of Communication (COC)" },
        { "College of Computer and Information Sciences (CCIS)", "College of Computer and Information Sciences (CCIS)" },
        { "College of Education (COED)", "College of Education (COED)" },
        { "College of Engineering (CE)", "College of Engineering (CE)" },
        { "College of Human Kinetics (CHK)", "College of Human Kinetics (CHK)" },
        { "College of Law (CL)", "College of Law (CL)" },
        { "College of Political Science and Public Administration (CPSPA)", "College of Political Science and Public Administration (CPSPA)" },
        { "College of Social Sciences and Development (CSSD)", "College of Social Sciences and Development (CSSD)" },
        { "College of Science (CS)", "College of Science (CS)" },
        { "College of Tourism, Hospitality and Transportation Management (CTHTM)", "College of Tourism, Hospitality and Transportation Management (CTHTM)" },
        { "Institute of Technology", "Institute of Technology" }
    };

            return colleges.ContainsKey(college) ? colleges[college] : college;
        }

        private string GetCollegeAcronym(string college)
        {
            var colleges = new Dictionary<string, string>
{
    { "All Colleges", "" },
    { "College of Accountancy and Finance (CAF)", "CAF" },
    { "College of Architecture, Design and the Built Environment (CADBE)", "CADBE" },
    { "College of Arts and Letters (CAL)", "CAL" },
    { "College of Business Administration (CBA)", "CBA" },
    { "College of Communication (COC)", "COC" },
    { "College of Computer and Information Sciences (CCIS)", "CCIS" },
    { "College of Education (COED)", "COED" },
    { "College of Engineering (CE)", "CE" },
    { "College of Human Kinetics (CHK)", "CHK" },
    { "College of Law (CL)", "CL" },
    { "College of Political Science and Public Administration (CPSPA)", "CPSPA" },
    { "College of Social Sciences and Development (CSSD)", "CSSD" },
    { "College of Science (CS)", "CS" },
    { "College of Tourism, Hospitality and Transportation Management (CTHTM)", "CTHTM" },
    { "Institute of Technology", "ITECH" }
};

            return colleges.ContainsKey(college) ? colleges[college] : "";
        }
        // Helper to get field of study acronym
        private string GetFieldOfStudyAcronym(string fieldOfStudy)
        {
            var fields = new Dictionary<string, string>
    {
        { "All Field of Study", "" },
        { "Education", "EDU" },
        { "Computer Science, Information Systems, and Technology", "CSIST" },
        { "Engineering, Architecture, and Design", "EAD" },
        { "Humanities, Language, and Communication", "HLC" },
        { "Business", "BUS" },
        { "Social Sciences", "SOCSCI" },
        { "Science, Mathematics, and Statistics", "SMS" }
    };

            return fields.ContainsKey(fieldOfStudy) ? fields[fieldOfStudy] : "";
        }
        
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


        public IActionResult ViewEthicsEvaluators()
        {
            // Fetch all the Ethics Evaluators from the database
            var evaluators = _context.CRE_EthicsEvaluator.ToList();

            // Pass the evaluators to the view
            return View(evaluators);
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

        public IActionResult Dashboard()
        {
            // Get the total number of applications with logs
            var totalApplications = _context.CRE_EthicsApplication
                .Where(application => application.EthicsApplicationLogs.Any()) // Only count applications with logs
                .Count();

            // Get the total number of ethics clearances
            var totalClearancesIssued = _context.CRE_EthicsClearance.Count();

            // Get the total number of terminal reports
            var totalTerminalReports = _context.CRE_CompletionReports.Count();

            // Get the total number of certificates issued
            var totalCertificatesIssued = _context.CRE_CompletionCertificates.Count();

            // Fetch the top performing fields of study based on non-funded applications
            var topFields = _context.CRE_EthicsApplication
                .GroupBy(application => application.FieldOfStudy)
                .Select(group => new
                {
                    FieldOfStudy = group.Key,
                    ApplicationCount = group.Count()
                })
                .OrderByDescending(x => x.ApplicationCount) // Order by most applications
                .Take(3) // Get the top 3 fields
                .ToList();

            // Prepare the model
            var model = new ChiefDashboardViewModel
            {
                TotalApplications = totalApplications,
                TotalClearancesIssued = totalClearancesIssued,
                TotalTerminalReports = totalTerminalReports,
                TotalCertificatesIssued = totalCertificatesIssued,
                TopFields = topFields.Select((field, index) => new TopFieldViewModel
                {
                    FieldName = field.FieldOfStudy,
                    ApplicationCount = field.ApplicationCount,
                    Rank = index + 1
                }).ToList() // Map to the view model
            };

            return View(model);
        }


    }
}
