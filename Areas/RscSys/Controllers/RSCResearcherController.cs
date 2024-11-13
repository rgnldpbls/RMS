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

        public RSCResearcherController(rscSysfinalDbContext context, UserManager<ApplicationUser> userManager, ILogger<RSCResearcherController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }


        [HttpPost]
        public async Task<IActionResult> CancelApplication(int requestId)
        {
            var application = await _context.Requests.FindAsync(requestId); // Replace 'Requests' with your actual DbSet name

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
            var drafts = _context.Drafts
                .Where(d => d.UserId == userId) // Filter drafts by the current user's ID
                .ToList();

            // Count notifications where NotificationStatus is false for this user
            ViewBag.UnreadNotificationsCount = _context.Notifications
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

            var request = await _context.Requests
                .Include(r => r.Requirements)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            // Manually retrieve ApplicationSubCategories for the draft based on criteria
            var applicationSubCategories = await _context.ApplicationSubCategories
                .Where(sc => sc.ApplicationType.ApplicationTypeName == request.ApplicationType
                           || sc.CategoryName == request.ApplicationType) // Adjusted condition
                .Include(sc => sc.Checklists) // Include Checklists for each ApplicationSubCategory
                .ToListAsync();

            var checklists = applicationSubCategories
                .SelectMany(sc => sc.Checklists)
                .ToList();

            var documentHistories = _context.DocumentHistories
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
            var request = await _context.Requests.FindAsync(requestId);

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
            var checklist = await _context.Checklists.FindAsync(checklistId);
            var request = await _context.Requests.FindAsync(requestId);

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
            _context.Requirements.Add(requirement);
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

            var application = _context.Requests
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

            var statusHistories = _context.StatusHistories
            .Where(sh => sh.RequestId == id)
            .OrderByDescending(sh => sh.StatusDate)  // Order by timestamp if you have a date field
            .ToList();

            // Pass the status histories to the view via ViewData
            ViewData["StatusHistories"] = statusHistories;

            var documentHistories = _context.DocumentHistories
            .Where(sh => sh.RequestId == id)
            .ToList();

            // Pass the status histories to the view via ViewData
            ViewData["DocumentHistories"] = documentHistories;

            // Count notifications where NotificationStatus is false for this user
            ViewBag.UnreadNotificationsCount = _context.Notifications
                .Count(n => n.UserId == userId && !n.NotificationStatus && n.Role == "Researcher" || n.UserId == userId && !n.NotificationStatus && n.Role == "All");

            return View(application);
        }

        public IActionResult Notifications()
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch notifications for the current user
            var notifications = _context.Notifications
                .Where(n => n.UserId == userId && n.Role == "Researcher" || n.UserId == userId && n.Role == "All")
                .OrderByDescending(n => n.NotificationCreationDate)
                .ToList();

            // Count notifications where NotificationStatus is false for this user
            ViewBag.UnreadNotificationsCount = _context.Notifications
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
            var notifications = _context.Notifications
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

            var applicationTypes = _context.ApplicationTypes
            .Include(at => at.ApplicationSubCategories) // Use the correct context
            .ToList();

            var viewModel = new DraftViewModel
            {
                Draft = new Draft { UserId = userId },
                Memorandums = await _context.Memorandums
                    .OrderByDescending(m => m.memorandumUploadDate)
                    .ToListAsync(),
                ApplicationTypes = applicationTypes, // Assign retrieved application types here
                ApplicationSubCategories = applicationTypes.SelectMany(at => at.ApplicationSubCategories).ToList()
            };

            // Count notifications where NotificationStatus is false for this user
            ViewBag.UnreadNotificationsCount = _context.Notifications
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
            var applications = _context.Requests
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();

            // Count notifications where NotificationStatus is false for this user
            ViewBag.UnreadNotificationsCount = _context.Notifications
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
            _context.Drafts.Add(draft);
            await _context.SaveChangesAsync();

            // Redirect to the document upload page, passing the draft ID
            return RedirectToAction("ViewApplication", new { draftId = draft.DraftId });
        }

        [HttpPost]
        public async Task<IActionResult> ReDraft(int requestId, string applicationType)
        {
            // Fetch the application data based on the requestId
            var application = await _context.Requests
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
            _context.Drafts.Add(application);
            await _context.SaveChangesAsync();

            // Redirect to the drafts page
            return Json(new { success = true, redirectUrl = Url.Action("Drafts", "RSCResearcher") });
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsDraft(Draft draft)
        {
            if (ModelState.IsValid)
            {
                // Get UserId from claims
                draft.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Handle the draft saving logic here...
                var existingDraft = await _context.Drafts.FindAsync(draft.DraftId);
                if (existingDraft != null)
                {
                    existingDraft.ApplicationType = draft.ApplicationType;
                    existingDraft.FieldOfStudy = draft.FieldOfStudy;
                    existingDraft.ResearchTitle = draft.ResearchTitle;
                    existingDraft.DtsNo = draft.DtsNo; // Save edited DTS Number
                    existingDraft.UpdatedDate = DateTime.Now;
                }
                else
                {
                    draft.CreatedDate = DateTime.Now;
                    _context.Drafts.Add(draft);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        public async Task<IActionResult> ViewApplication(int draftId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the logged-in user's ID

            // Retrieve the draft including its requirements
            var draft = await _context.Drafts
                .Include(d => d.Requirements) // Include Requirements
                .FirstOrDefaultAsync(d => d.DraftId == draftId);

            if (draft == null)
            {
                return NotFound();
            }

            List<Checklist> checklists;

            // First, try to retrieve the ApplicationType that directly matches draft.ApplicationType
            var applicationType = await _context.ApplicationTypes
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
                var applicationSubCategories = await _context.ApplicationSubCategories
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
            ViewBag.UnreadNotificationsCount = _context.Notifications
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

            var checklist = await _context.Checklists.FindAsync(checklistId);
            var draft = await _context.Drafts.FindAsync(draftId);

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
                FileType = uploadedFile.ContentType,
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
            _context.Requirements.Add(requirement);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "File uploaded successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFile(int requirementId)
        {
            // Find the requirement record in the database by its ID
            var requirement = await _context.Requirements.FindAsync(requirementId);

            if (requirement == null)
            {
                return Json(new { success = false, message = "File not found" }); // If file doesn't exist in the database
            }

            // Remove the file record from the database
            _context.Requirements.Remove(requirement);
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
            var draft = await _context.Drafts.Include(d => d.Requirements).FirstOrDefaultAsync(d => d.DraftId == draftId);

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
            _context.Drafts.Remove(draft);
            await _context.SaveChangesAsync();

            return RedirectToAction("Drafts");
        }

        [HttpPost]
        public IActionResult SubmitAsRequest(int draftId)
        {
            var draft = _context.Drafts
                         .Include(d => d.Requirements)
                         .FirstOrDefault(d => d.DraftId == draftId);

            if (draft == null)
            {
                return Json(new { success = false, error = "Draft not found." });
            }

            // Calculate requestSpent based on ApplicationType or ApplicationSubCategory
            decimal requestSpent = 0;
            var subCategory = _context.ApplicationSubCategories
                               .FirstOrDefault(sc => sc.CategoryName == draft.ApplicationType);

            if (subCategory != null)
            {
                requestSpent = subCategory.SubAmount;
            }
            else
            {
                var applicationType = _context.ApplicationTypes
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

            _context.Requests.Add(request);
            _context.SaveChanges();

            // Update requirements and remove draft
            foreach (var requirement in draft.Requirements)
            {
                requirement.RequestId = request.RequestId;
                requirement.DraftId = null;
            }

            _context.Requirements.UpdateRange(draft.Requirements);
            _context.Drafts.Remove(draft);

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
            _context.Notifications.Add(researcherNotification);

            // Notifications for all users in the "Chief" role
            var chiefUsers = _userManager.GetUsersInRoleAsync("Chief").Result;
            foreach (var chiefUser in chiefUsers)
            {
                var chiefNotification = new Notifications
                {
                    UserId = chiefUser.Id,
                    NotificationTitle = "New Application Request",
                    NotificationMessage = $"A new application request has been submitted by {draft.Name} withh the DTS No. {draft.DtsNo}.",
                    NotificationCreationDate = DateTime.Now,
                    NotificationStatus = false,
                    Role = "Chief"
                };
                _context.Notifications.Add(chiefNotification);
            }

            // Add status history
            var statusHistory = new StatusHistory
            {
                RequestId = request.RequestId,
                Status = "Application Submitted",
                StatusDate = DateTime.Now,
                Comments = "The application was submitted successfully."
            };
            _context.StatusHistories.Add(statusHistory);

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
            var memo = _context.Memorandums.FirstOrDefault(r => r.memorandumId == memorandumId);

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
            var document = await _context.FinalDocuments
                .FirstOrDefaultAsync(d => d.RequestId == requestId);

            if (document == null)
            {
                return NotFound("Endorsement letter not found.");
            }

            var fileName = document.FinalDocuName; // Set the file name as needed
            var fileType = document.FileType; // Set the file type (e.g., "application/pdf")

            return File(document.FileData, fileType, fileName);
        }

    }
}
