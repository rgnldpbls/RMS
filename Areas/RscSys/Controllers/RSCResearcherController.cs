using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MimeKit;
using NuGet.LibraryModel;
using rscSys_final.Data;
using rscSys_final.Models;
using System;
using MailKit.Net.Smtp;
using System.Security.Claims;
using ResearchManagementSystem.Models;
using ResearchManagementSystem.Areas.CreSys.Data;

namespace rscSys_final.Controllers
{
    [Area("RscSys")]
    [Authorize(Roles = "Faculty,Student")]
    public class RSCResearcherController : Controller
    {
        private readonly rscSysfinalDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RSCResearcherController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly CreDbContext _creDbContext;

        public RSCResearcherController(rscSysfinalDbContext context, UserManager<ApplicationUser> userManager, ILogger<RSCResearcherController> logger, IWebHostEnvironment hostingEnvironment, CreDbContext creDbContext)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _creDbContext = creDbContext;
        }

        public async Task<IActionResult> Memo()
        {
            var memoranda = await _context.RSC_Memorandums.ToListAsync();

            // Categorize memoranda
            var memorandaList = memoranda.Where(m => m.memorandumName.Contains("Memorandum")).ToList();
            var executiveOrdersList = memoranda.Where(m => !m.memorandumName.Contains("Memorandum")).ToList();

            // Pass data to the view
            var viewModel = new ResearcherMemoViewModel
            {
                Memoranda = memorandaList,
                ExecutiveOrders = executiveOrdersList
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CancelApplication(int requestId)
        {
            var application = await _context.RSC_Requests.FindAsync(requestId); // Replace 'Requests' with your actual DbSet name

            if (application == null)
            {
                return NotFound();
            }

            application.Status = "Cancelled"; // Set status to "Cancelled"
            _context.Update(application);
            await _context.SaveChangesAsync();

            return RedirectToAction("Applications"); // Redirect to the Applications page or wherever you want
        }

        public IActionResult Drafts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID
            var drafts = _context.RSC_Drafts
                .Where(d => d.UserId == userId) // Filter drafts by the current user's ID
                .ToList();

            // Count notifications where NotificationStatus is false for this user
            ViewBag.UnreadNotificationsCount = _context.RSC_Notifications
                .Count(n => n.UserId == userId && !n.NotificationStatus && n.Role == "Researcher" || n.UserId == userId && !n.NotificationStatus && n.Role == "All");

            return View(drafts);
        }

        public async Task<IActionResult> ResubmitDocu(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var request = await _context.RSC_Requests
                .Include(r => r.Requirements)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            // Manually retrieve ApplicationSubCategories for the draft based on criteria
            var applicationSubCategories = await _context.RSC_ApplicationSubCategories
                .Where(sc => sc.ApplicationType.ApplicationTypeName == request.ApplicationType
                           || sc.CategoryName == request.ApplicationType) // Adjusted condition
                .Include(sc => sc.Checklists) // Include Checklists for each ApplicationSubCategory
                .ToListAsync();

            var checklists = applicationSubCategories
                .SelectMany(sc => sc.Checklists)
                .ToList();

            var documentHistories = _context.RSC_DocumentHistories
            .Where(sh => sh.RequestId == id)
            .ToList();


            var model = new ResubmitViewModel
            {
                Request = request,
                Requirements = request.Requirements.ToList(),
                Checklists = checklists,
                DocumentHistories = documentHistories
            };

            // Get the temporary upload path
            var tempUploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "temp", user.Email, request.DtsNo);

            // Fetch files from the temporary directory
            var tempFiles = Directory.Exists(tempUploadPath)
                ? Directory.GetFiles(tempUploadPath)
                    .Select(filePath => new Requirement
                    {
                        RequirementId = 0, // Temporary, you can use a unique identifier if needed
                        FileName = Path.GetFileName(filePath),

                    })
                    .ToList()
                : new List<Requirement>();

            // Set the temporary files to the model
            ViewBag.TempFiles = tempFiles; // Or include in your view model if applicable

            return View(model);

            /*return View(request);*/
        }

        [HttpPost]
        public async Task<IActionResult> ResubmitDocuments(int requestId)
        {
            // Find the request by its ID
            var request = await _context.RSC_Requests.FindAsync(requestId);

            // Check if the request exists
            if (request == null)
            {
                return NotFound();
            }

            // Update the status to "For Evaluation"
            request.Status = "For Evaluation";

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Return a success response
            TempData["SuccessMessage"] = "Application resubmitted successfully and is now under evaluation.";
            return RedirectToAction("Applications"); // Redirect to your desired action or page
        }

        [HttpPost]
        public async Task<IActionResult> ResubmitFiles(int checklistId, int requestId, IFormFile uploadedFile)
        {
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                return Json(new { success = false, message = "Please select a file to upload." });
            }

            // Retrieve checklist and draft to get the necessary data
            var checklist = await _context.RSC_Checklists.FindAsync(checklistId);
            var request = await _context.RSC_Requests.FindAsync(requestId);

            if (checklist == null || request == null)
            {
                return Json(new { success = false, message = "Checklist or Draft not found." });
            }

            // Get the checklist name and limit it to 6 words
            var checklistName = checklist.ChecklistName;
            var words = checklistName.Split(' ');
            var limitedChecklistName = string.Join("-", words.Take(6)); // Take only the first 6 words

            var dtsNo = request.DtsNo;
            var fileName = $"{limitedChecklistName}.._{dtsNo}";

            // Create a new Requirement
            var requirement = new Requirement
            {
                RequestId = requestId,
                ChecklistId = checklistId,
                FileName = fileName, // Use the formatted file name
                FileType = uploadedFile.ContentType,
                UploadDate = DateTime.Now,
                IsResubmitted = true
            };

            using (var memoryStream = new MemoryStream())
            {
                await uploadedFile.CopyToAsync(memoryStream);
                requirement.FileData = memoryStream.ToArray();
            }

            // Save the requirement to the database
            _context.RSC_Requirements.Add(requirement);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "File uploaded successfully." });
        }

        public async Task<IActionResult> ViewDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the logged-in user's ID

            if (userId == null)
            {
                return Unauthorized();  // If not authenticated
            }

            var application = _context.RSC_Requests
                .Include(r => r.Requirements)
                .FirstOrDefault(r => r.RequestId == id);

            if (application == null)
            {
                return NotFound();
            }

            // Retrieve the user's details (first and last name)
            var user = await _userManager.FindByIdAsync(userId);
            // Optionally, you can check if the user exists
            if (user == null)
            {
                return NotFound();  // Handle if user not found
            }

            // Create a formatted name (LastName, FirstName)
            var applicantName = $"{user.LastName}, {user.FirstName}";

            // Pass the applicant name to the view via ViewData
            ViewData["ApplicantName"] = applicantName;
            ViewData["Status"] = application.Status;

            var statusHistories = _context.RSC_StatusHistories
            .Where(sh => sh.RequestId == id)
            .OrderByDescending(sh => sh.StatusDate)  // Order by timestamp if you have a date field
            .ToList();

            // Pass the status histories to the view via ViewData
            ViewData["StatusHistories"] = statusHistories;

            var documentHistories = _context.RSC_DocumentHistories
            .Where(sh => sh.RequestId == id)
            .ToList();

            // Pass the status histories to the view via ViewData
            ViewData["DocumentHistories"] = documentHistories;

            // Count notifications where NotificationStatus is false for this user
            ViewBag.UnreadNotificationsCount = _context.RSC_Notifications
                .Count(n => n.UserId == userId && !n.NotificationStatus && n.Role == "Researcher" || n.UserId == userId && !n.NotificationStatus && n.Role == "All");

            return View(application);
        }

        public IActionResult Notifications()
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch notifications for the current user
            var notifications = _context.RSC_Notifications
                .Where(n => n.UserId == userId && n.Role == "Researcher" || n.UserId == userId && n.Role == "All")
                .OrderByDescending(n => n.NotificationCreationDate)
                .ToList();

            // Count notifications where NotificationStatus is false for this user
            ViewBag.UnreadNotificationsCount = _context.RSC_Notifications
                .Count(n => n.UserId == userId && !n.NotificationStatus && n.Role == "Researcher" || n.UserId == userId && !n.NotificationStatus && n.Role == "All");

            // Pass the notifications to the view
            return View(notifications);
        }

        [HttpPost]
        public IActionResult MarkAllAsRead()
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find all unread notifications for the current user
            var notifications = _context.RSC_Notifications
                .Where(n => n.UserId == userId && n.Role == "Researcher" || n.Role == "All" && !n.NotificationStatus)
                .ToList();

            // Mark each notification as read
            foreach (var notification in notifications)
            {
                notification.NotificationStatus = true;
            }

            // Save changes to the database
            _context.SaveChanges();

            return RedirectToAction("Notifications");
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var applicationTypes = _context.RSC_ApplicationTypes
            .Include(at => at.ApplicationSubCategories) // Use the correct context
            .ToList();

            var viewModel = new DraftViewModel
            {
                Draft = new Draft { UserId = userId },
                Memorandums = await _context.RSC_Memorandums
                    .OrderByDescending(m => m.memorandumUploadDate)
                    .ToListAsync(),
                ApplicationTypes = applicationTypes, // Assign retrieved application types here
                ApplicationSubCategories = applicationTypes.SelectMany(at => at.ApplicationSubCategories).ToList()
            };

            // Count notifications where NotificationStatus is false for this user
            ViewBag.UnreadNotificationsCount = _context.RSC_Notifications
                .Count(n => n.UserId == userId && !n.NotificationStatus && n.Role == "Researcher" || n.UserId == userId && !n.NotificationStatus && n.Role == "All");

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Applications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the logged-in user's ID

            if (userId == null)
            {
                return Unauthorized();  // If not authenticated
            }

            // Retrieve the applications related to the logged-in user
            var applications = _context.RSC_Requests
                  .Include(r => r.DocumentHistories)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();

            // Count notifications where NotificationStatus is false for this user
            ViewBag.UnreadNotificationsCount = _context.RSC_Notifications
                .Count(n => n.UserId == userId && !n.NotificationStatus && n.Role == "Researcher" || n.UserId == userId && !n.NotificationStatus && n.Role == "All");

            return View(applications);  // Pass applications to the view
        }

        // POST: SaveDraft
        [HttpPost]
        public async Task<IActionResult> SaveDraft(DraftViewModel model)
        {
            // Get the logged-in user ID
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();  // User is not logged in
            }

            // Create a new draft object
            var draft = new Draft
            {
                UserId = user.Id,  // Set the logged-in user ID
                DtsNo = model.Draft.DtsNo,
                Name = $"{user.FirstName} {user.LastName}",
                Branch = user.Campus,
                College = user.College,
                Email = user.Email,
                ApplicationType = model.Draft.ApplicationType,
                FieldOfStudy = model.Draft.FieldOfStudy,
                ResearchTitle = model.Draft.ResearchTitle,
                CreatedDate = DateTime.Now
            };

            // Save the draft to the database
            _context.RSC_Drafts.Add(draft);
            await _context.SaveChangesAsync();

            // Redirect to the document upload page, passing the draft ID
            return RedirectToAction("ViewApplication", new { draftId = draft.DraftId });
        }

        [HttpPost]
        public async Task<IActionResult> ReDraft(int requestId, string applicationType)
        {
            // Fetch the application data based on the requestId
            var application = await _context.RSC_Requests
                .Where(r => r.RequestId == requestId)
                .Select(r => new Draft
                {
                    UserId = r.UserId,
                    ApplicationType = applicationType, // Use the selected applicationType from the dropdown
                    ResearchTitle = r.ResearchTitle,
                    FieldOfStudy = r.FieldOfStudy,
                    CreatedDate = DateTime.Now
                })
                .FirstOrDefaultAsync();

            if (application == null)
            {
                return NotFound();
            }

            // Add the draft without requestId and save it to the database
            _context.RSC_Drafts.Add(application);
            await _context.SaveChangesAsync();

            // Redirect to the drafts page
            return Json(new { success = true, redirectUrl = Url.Action("Drafts", "RSCResearcher") });
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsDraft(Draft draft)
        {
            // Validate ModelState
            if (!ModelState.IsValid)
            {
                // Log or return detailed error messages for debugging
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, errors });
            }

            try
            {
                // Get UserId from claims
                draft.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Check if the draft exists
                var existingDraft = await _context.RSC_Drafts.FindAsync(draft.DraftId);
                if (existingDraft != null)
                {
                    // Update existing draft
                    existingDraft.Name = draft.Name;
                    existingDraft.Email = draft.Email;
                    existingDraft.Branch = draft.Branch;
                    existingDraft.ApplicationType = draft.ApplicationType;
                    existingDraft.FieldOfStudy = draft.FieldOfStudy;
                    existingDraft.ResearchTitle = draft.ResearchTitle;
                    existingDraft.DtsNo = draft.DtsNo; // Save edited DTS Number
                    existingDraft.UpdatedDate = DateTime.Now;
                }
                else
                {
                    // Create a new draft
                    draft.CreatedDate = DateTime.Now;
                    _context.RSC_Drafts.Add(draft);
                }

                // Save changes
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return Json(new { success = false, error = ex.Message });
            }
        }


        public async Task<IActionResult> ViewApplication(int draftId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the logged-in user's ID

            // Retrieve the draft including its requirements
            var draft = await _context.RSC_Drafts
                .Include(d => d.Requirements) // Include Requirements
                .FirstOrDefaultAsync(d => d.DraftId == draftId);

            if (draft == null)
            {
                return NotFound();
            }

            List<Checklist> checklists;

            // First, try to retrieve the ApplicationType that directly matches draft.ApplicationType
            var applicationType = await _context.RSC_ApplicationTypes
                .Include(at => at.Checklists) // Include checklists for the ApplicationType
                .FirstOrDefaultAsync(at => at.ApplicationTypeName == draft.ApplicationType);

            if (applicationType != null && applicationType.Checklists.Any())
            {
                // If a direct match is found and checklists are present, fetch from ApplicationType
                checklists = applicationType.Checklists.ToList();
            }
            else
            {
                // If no direct match, retrieve ApplicationSubCategories related to the draft
                var applicationSubCategories = await _context.RSC_ApplicationSubCategories
                    .Where(sc => sc.ApplicationType.ApplicationTypeName == draft.ApplicationType || sc.CategoryName == draft.ApplicationType)
                    .Include(sc => sc.Checklists) // Include Checklists for subcategories
                    .ToListAsync();

                // Extract checklists from the ApplicationSubCategories
                checklists = applicationSubCategories
                    .SelectMany(sc => sc.Checklists)
                    .ToList();
            }

            var model = new ApplicationViewModel
            {
                Draft = draft,
                Requirements = draft.Requirements.ToList(),
                Checklists = checklists // Set the fetched Checklists
            };

            // Count notifications where NotificationStatus is false for this user
            ViewBag.UnreadNotificationsCount = _context.RSC_Notifications
                .Count(n => (n.UserId == userId && !n.NotificationStatus && n.Role == "Researcher") ||
                            (n.UserId == userId && !n.NotificationStatus && n.Role == "All"));

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> UploadFiles(int checklistId, int draftId, IFormFile uploadedFile)
        {
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                return Json(new { success = false, message = "Please select a file to upload." });
            }

            var checklist = await _context.RSC_Checklists.FindAsync(checklistId);
            var draft = await _context.RSC_Drafts.FindAsync(draftId);

            if (checklist == null || draft == null)
            {
                return Json(new { success = false, message = "Checklist or Draft not found." });
            }

            var checklistName = checklist.ChecklistName;
            var limitedChecklistName = string.Join("-", checklistName.Split(' ').Take(6));
            var dtsNo = draft.DtsNo;
            var fileName = $"{limitedChecklistName}.._{dtsNo}";

            // Create a new Requirement
            var requirement = new Requirement
            {
                DraftId = draftId,
                ChecklistId = checklistId,
                FileName = fileName,
                FileType = "application/pdf",
                UploadDate = DateTime.Now,
                IsResubmitted = false
            };

            // Use MemoryStream to read the file data
            using (var memoryStream = new MemoryStream())
            {
                await uploadedFile.CopyToAsync(memoryStream);
                requirement.FileData = memoryStream.ToArray(); // Store the file data
            }

            // Save the requirement to the database
            _context.RSC_Requirements.Add(requirement);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "File uploaded successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFile(int requirementId)
        {
            // Find the requirement record in the database by its ID
            var requirement = await _context.RSC_Requirements.FindAsync(requirementId);

            if (requirement == null)
            {
                return Json(new { success = false, message = "File not found" }); // If file doesn't exist in the database
            }

            // Remove the file record from the database
            _context.RSC_Requirements.Remove(requirement);
            await _context.SaveChangesAsync();

            // Redirect back or return success
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult DeleteTempFile(string fileName, string userEmail, string dtsNumber)
        {
            var uploadsRootPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "temp", userEmail, dtsNumber);
            var filePath = Path.Combine(uploadsRootPath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "File not found." });
        }

        // POST: Delete a draft
        public async Task<IActionResult> DeleteDraft(int draftId)
        {
            var draft = await _context.RSC_Drafts.Include(d => d.Requirements).FirstOrDefaultAsync(d => d.DraftId == draftId);

            if (draft == null)
            {
                return NotFound();
            }

            // Delete associated files from the server
            foreach (var requirement in draft.Requirements)
            {
                /*if (!string.IsNullOrEmpty(requirement.FilePath) && System.IO.File.Exists(requirement.FilePath))
                {
                    System.IO.File.Delete(requirement.FilePath);
                }*/
            }

            // Remove the draft (which will also remove related requirements due to cascading delete)
            _context.RSC_Drafts.Remove(draft);
            await _context.SaveChangesAsync();

            return RedirectToAction("Drafts");
        }

        [HttpPost]
        public IActionResult SubmitAsRequest(int draftId)
        {
            var draft = _context.RSC_Drafts
                .Include(d => d.Requirements)
                .FirstOrDefault(d => d.DraftId == draftId);

            if (draft == null)
            {
                return Json(new { success = false, error = "Draft not found." });
            }

            // Validation for duplicate DTS number
            var duplicateDtsExists = _context.RSC_Requests.Any(r => r.DtsNo == draft.DtsNo);
            if (duplicateDtsExists)
            {
                return Json(new { success = false, error = "A request with the same DTS number already exists." });
            }

            // Validation for duplicate research titles
            if (draft.ApplicationType == "Publication and Citation Incentives")
            {
                var publicationCount = _context.RSC_Requests
                    .Count(r => r.ApplicationType == "Publication and Citation Incentives" && r.ResearchTitle == draft.ResearchTitle);

                if (publicationCount >= 10)
                {
                    return Json(new { success = false, error = "The maximum limit of 10 applications for Publication and Citation Incentives has been reached for this research title." });
                }
            }
            else
            {
                var duplicateTitleExists = _context.RSC_Requests
                    .Any(r => r.ApplicationType == draft.ApplicationType && r.ResearchTitle == draft.ResearchTitle);

                if (duplicateTitleExists)
                {
                    return Json(new { success = false, error = "A request with the same research title already exists for this application type." });
                }
            }

            // Calculate requestSpent based on ApplicationType or ApplicationSubCategory
            decimal requestSpent = 0;
            var subCategory = _context.RSC_ApplicationSubCategories
                .FirstOrDefault(sc => sc.CategoryName == draft.ApplicationType);

            if (subCategory != null)
            {
                requestSpent = subCategory.SubAmount;
            }
            else
            {
                var applicationType = _context.RSC_ApplicationTypes
                    .FirstOrDefault(at => at.ApplicationTypeName == draft.ApplicationType);
                if (applicationType != null)
                {
                    requestSpent = applicationType.Amount;
                }
            }

            // Create a new Request from the Draft
            var request = new Request
            {
                UserId = draft.UserId,
                Name = draft.Name,
                College = draft.College,
                Branch = draft.Branch,
                Email = draft.Email,
                DtsNo = draft.DtsNo,
                ApplicationType = draft.ApplicationType,
                FieldOfStudy = draft.FieldOfStudy,
                Status = "For Evaluation",
                RequestSpent = (double)requestSpent,
                CreatedDate = DateTime.Now,
                ResearchTitle = draft.ResearchTitle,
                SubmittedDate = DateTime.Now
            };

            _context.RSC_Requests.Add(request);
            _context.SaveChanges();

            // Update requirements and remove draft
            foreach (var requirement in draft.Requirements)
            {
                requirement.RequestId = request.RequestId;
                requirement.DraftId = null;
            }

            _context.RSC_Requirements.UpdateRange(draft.Requirements);
            _context.RSC_Drafts.Remove(draft);

            // Notification for the researcher
            var researcherNotification = new Notifications
            {
                UserId = draft.UserId,
                NotificationTitle = "Application Submitted",
                NotificationMessage = "You have successfully submitted your application. You will receive an update when the status of your application changes.",
                NotificationCreationDate = DateTime.Now,
                NotificationStatus = false,
                Role = "Researcher"
            };
            _context.RSC_Notifications.Add(researcherNotification);

            // Notifications for all users in the "Chief" role
            var chiefUsers = _userManager.GetUsersInRoleAsync("Chief").Result;
            foreach (var chiefUser in chiefUsers)
            {
                var chiefNotification = new Notifications
                {
                    UserId = chiefUser.Id,
                    NotificationTitle = "New Application Request",
                    NotificationMessage = $"A new application request has been submitted by {draft.Name} with the DTS No. {draft.DtsNo}.",
                    NotificationCreationDate = DateTime.Now,
                    NotificationStatus = false,
                    Role = "Chief"
                };
                _context.RSC_Notifications.Add(chiefNotification);
            }

            // Add status history
            var statusHistory = new StatusHistory
            {
                RequestId = request.RequestId,
                Status = "Application Submitted",
                StatusDate = DateTime.Now,
                Comments = "The application was submitted successfully."
            };
            _context.RSC_StatusHistories.Add(statusHistory);

            _context.SaveChanges();

            // Send a confirmation email with the user's name
            SendConfirmationEmail(draft.Email, draft.Name);

            return Json(new { success = true });
        }


        private void SendConfirmationEmail(string recipientEmail, string Name)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("PUP Research Support Center", "rsc.rmo3@gmail.com"));
            message.To.Add(new MailboxAddress($"{Name}", recipientEmail));
            message.Subject = "Application Submitted Successfully";
            message.Body = new TextPart("plain")
            {
                Text = $"Dear {Name},\n\nThank you for submitting your application. We have received it. We will keep you updated on any changes to the status of your application. In the meantime, you can track the progress by visiting your application page and selecting 'View Details'\n\nBest regards,\nPUP Research Support Center"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate("rsc.rmo3@gmail.com", "ervg ojdk oeom rfyk");

                client.Send(message);
                client.Disconnect(true);
            }
        }

        public IActionResult ViewMemo(int memorandumId)
        {
            var memo = _context.RSC_Memorandums.FirstOrDefault(r => r.memorandumId == memorandumId);

            if (memo == null)
            {
                return NotFound();
            }

            Response.Headers.Add("Content-Disposition", "inline; filename=\"" + memo.memorandumName + "\"");
            return File(memo.memorandumData, memo.filetype);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadEndorsementLetter(int requestId)
        {
            var document = await _context.RSC_FinalDocuments
                .FirstOrDefaultAsync(d => d.RequestId == requestId);

            if (document == null)
            {
                return NotFound("Endorsement letter not found.");
            }

            var fileName = document.FinalDocuName; // Set the file name as needed
            var fileType = document.FileType; // Set the file type (e.g., "application/pdf")

            return File(document.FileData, fileType, fileName);
        }

        public IActionResult ViewFile(int requirementId)
        {
            var requirement = _context.RSC_Requirements.FirstOrDefault(r => r.RequirementId == requirementId);

            if (requirement == null)
            {
                return NotFound();
            }

            Response.Headers.Add("Content-Disposition", "inline; filename=\"" + requirement.FileName + "\"");
            return File(requirement.FileData, requirement.FileType);
        }

        [HttpPost]
        public async Task<IActionResult> RetrieveEthicsClearance(string urecNumber, int draftId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(urecNumber))
            {
                return Json(new { success = false, message = "Please provide a UREC number." });
            }

            // Retrieve the ethics clearance using the UREC number
            var ethicsClearance = await _creDbContext.CRE_EthicsClearance
                .FirstOrDefaultAsync(e => e.UrecNo == urecNumber && e.EthicsApplication.UserId == userId);

            if (ethicsClearance == null)
            {
                return Json(new { success = false, message = "No Ethics Clearance found for the provided UREC number. Please apply for Ethics Clearance." });
            }

            // Retrieve the draft and application type information
            var draft = await _context.RSC_Drafts
                .FirstOrDefaultAsync(d => d.DraftId == draftId && d.UserId == userId);

            if (draft == null || string.IsNullOrEmpty(draft.ApplicationType))
            {
                return Json(new { success = false, message = "Draft not found or missing application type information." });
            }

            // Prioritize SubCategory over ApplicationType
            var subCategoryId = await _context.RSC_ApplicationSubCategories
                .Where(sc => sc.CategoryName == draft.ApplicationType)
                .Select(sc => (int?)sc.CategoryId)
                .FirstOrDefaultAsync();

            if (subCategoryId != null)
            {
                // Fetch the checklist for the subcategory
                var checklist = await _context.RSC_Checklists
                    .Where(c =>
                        c.ApplicationSubCategoryId == subCategoryId &&
                        c.ChecklistName.ToLower().Contains("copy of ethics clearance".ToLower()))
                    .FirstOrDefaultAsync();

                if (checklist != null)
                {
                    var base64File = Convert.ToBase64String(ethicsClearance.ClearanceFile);

                    return Json(new
                    {
                        success = true,
                        message = "Ethics Clearance found! Would you like to upload it automatically?",
                        clearanceFile = base64File,
                        checklistId = checklist.ChecklistId,
                        draftId = draftId
                    });
                }
            }

            // If no subcategory checklist found, fall back to ApplicationType
            var applicationTypeId = await _context.RSC_ApplicationTypes
                .Where(at => at.ApplicationTypeName == draft.ApplicationType)
                .Select(at => (int?)at.ApplicationTypeId)
                .FirstOrDefaultAsync();

            if (applicationTypeId != null)
            {
                var checklist = await _context.RSC_Checklists
                    .FirstOrDefaultAsync(c =>
                        c.ApplicationTypeId == applicationTypeId &&
                        c.ChecklistName.ToLower().Contains("copy of ethics clearance".ToLower()));

                if (checklist != null)
                {
                    var base64File = Convert.ToBase64String(ethicsClearance.ClearanceFile);

                    return Json(new
                    {
                        success = true,
                        message = "Ethics Clearance found! Would you like to upload it automatically?",
                        clearanceFile = base64File,
                        checklistId = checklist.ChecklistId,
                        draftId = draftId
                    });
                }
            }

            return Json(new { success = false, message = "No checklist for Ethics Clearance found." });
        }





    }
}
