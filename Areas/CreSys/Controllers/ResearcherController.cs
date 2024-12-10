using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemcSys.Data;
using ResearchManagementSystem.Areas.CreSys.Data;
using ResearchManagementSystem.Areas.CreSys.Interfaces;
using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Areas.CreSys.ViewModels;
using ResearchManagementSystem.Models;
using System.Security.Claims;

namespace ResearchManagementSystem.Areas.CreSys.Controllers
{
    [Area("CreSys")]
    [Authorize]
    public class ResearcherController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CreDbContext _context;
        private readonly IEthicsEmailService _emailService;
        private readonly IAllServices _allServices;
        private readonly RemcDBContext _remcDbContext;

        public ResearcherController(UserManager<ApplicationUser> userManager, CreDbContext context, IEthicsEmailService emailService, IAllServices allServices, RemcDBContext remcDbContext)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
            _allServices = allServices;
            _remcDbContext = remcDbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        
        [HttpGet]
        public async Task<IActionResult> Applications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "User is not logged in.");
                return View(); // Return view with error handling
            }
            // Fetch the ApplicationUser record using UserManager
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(); // Handle user not found scenario
            }
            // Concatenate FirstName, MiddleName, LastName
            var fullName = $"{user.FirstName} {user.MiddleName} {user.LastName}".Trim();
            // Fetch data directly from the DbContext
            var ethicsApplications = await _context.CRE_EthicsApplication
                .Where(e => e.UserId == userId)
                .ToListAsync();

            var nonFundedResearchInfos = await _context.CRE_NonFundedResearchInfo
                .Where(n => n.UserId == userId)
                .ToListAsync();

            var ethicsApplicationIds = ethicsApplications
                .Select(e => e.UrecNo)
                .ToList();

            var ethicsApplicationLogs = await _context.CRE_EthicsApplicationLogs
                .Where(l => ethicsApplicationIds.Contains(l.UrecNo))
                .GroupBy(l => l.UrecNo)
                .Select(g => g.OrderByDescending(l => l.ChangeDate).FirstOrDefault()) // Get latest log for each application
                .ToListAsync();

            // Use ViewBag to pass data to the view
            ViewBag.FullName = fullName;
            ViewBag.EthicsApplications = ethicsApplications;
            ViewBag.NonFundedResearchInfos = nonFundedResearchInfos;
            ViewBag.EthicsApplicationLogs = ethicsApplicationLogs;

            // Return the view
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> DownloadForms()
        {
            var forms = await _context.CRE_EthicsForms.ToListAsync();
            return View(forms);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadForm(string id)
        {
            var form = await _context.CRE_EthicsForms.FirstOrDefaultAsync(f => f.EthicsFormId == id);

            if (form == null || form.EthicsFormFile == null)
            {
                return NotFound();
            }

            return File(form.EthicsFormFile,
                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                        form.FormName + ".docx");
        }

        [HttpGet]
        public async Task<IActionResult> ApplyEthics()
        {
            // Fetch the current user
            var user = await _userManager.GetUserAsync(User);

            // Determine if the user is external based on their roles
            var roles = await _userManager.GetRolesAsync(user);
            bool isExternalUser = roles.Any(role => role.Contains("external", StringComparison.OrdinalIgnoreCase));

            // Construct the user's full name, including middle initial if available
            var middleInitial = !string.IsNullOrWhiteSpace(user.MiddleName) ? $"{user.MiddleName[0]}." : string.Empty;
            string fullName = $"{user.FirstName} {middleInitial} {user.LastName}".Trim();

            // Initialize the ViewModel with necessary data
            var model = new ApplyEthicsViewModel
            {
                User = user,
                FullName = fullName,
                EthicsApplication = new EthicsApplication(),
                NonFundedResearchInfo = new NonFundedResearchInfo(),
                CoProponent = new List<CoProponent>(),
                IsExternalResearcher = isExternalUser,
                ReceiptInfo = isExternalUser ? new ReceiptInfo() : null,
                EthicsApplicationLogs = new List<EthicsApplicationLogs>(),
                
            };

            // Return the populated ViewModel to the View
            return View(model);
        }

        [HttpGet]
        public IActionResult CheckDtsNumber(string dtsNo)
        {
            if (string.IsNullOrEmpty(dtsNo))
            {
                return Json(new { isValid = false, message = "DTS Number is required." });
            }

            // Check if the DTS number exists in your database (adjust the condition according to your data structure)
            var existingDts = _context.CRE_EthicsApplication
                .FirstOrDefault(e => e.DtsNo == dtsNo);

            if (existingDts != null)
            {
                // If the DTS number exists, it's invalid
                return Json(new { isValid = false, message = "DTS Number already exists." });
            }

            // If no matching DTS number is found, it's valid
            return Json(new { isValid = true, message = "" });
        }
        
        private async Task<string> GenerateUrecNoAsync()
        {
            string baseFormat = "UREC-{0}-{1}"; // "UREC-YYYY-XXXX"
            string year = DateTime.Now.Year.ToString();

            // Fetch existing UREC Nos for the current year
            var existingUrecNos = await _context.CRE_EthicsApplication
                .Where(u => u.UrecNo.StartsWith($"UREC-{year}-"))
                .Select(u => u.UrecNo)
                .ToListAsync();

            // Determine the next sequence number
            int newSequenceNumber = 1; // Default to 1
            if (existingUrecNos.Any())
            {
                // Extract the last four digits from existing UREC Nos
                var lastNumbers = existingUrecNos.Select(u =>
                    int.Parse(u.Substring(u.Length - 4))).ToList();

                // Get the maximum number and increment it
                newSequenceNumber = lastNumbers.Max() + 1;
            }

            // Format the new UREC No.
            string newUrecNo = string.Format(baseFormat, year, newSequenceNumber.ToString("D4"));

            return newUrecNo;
        }
        private async Task<string> GenerateNonFundedResearchIdAsync()
        {
            string year = DateTime.Now.Year.ToString();

            string prefix = $"NFID-{year}-";
            var currentYearIds = await _context.CRE_NonFundedResearchInfo
                .Where(r => r.NonFundedResearchId.StartsWith(prefix))
                .ToListAsync();
            var sequenceNumbers = currentYearIds
                .Where(r => r.NonFundedResearchId.Length == prefix.Length + 4 && 
                            int.TryParse(r.NonFundedResearchId.Substring(prefix.Length, 4), out _)) 
                .Select(r => int.Parse(r.NonFundedResearchId.Substring(prefix.Length, 4)))
                .ToList();

            int nextSequence = (sequenceNumbers.Any() ? sequenceNumbers.Max() : 0) + 1;

            string id = $"{prefix}{nextSequence:D4}";

            return id;
        }

        [HttpGet]
        public async Task<IActionResult> ViewFile(string formId, string urecNo)
        {
            var form = await _context.CRE_EthicsApplicationForms
                .FirstOrDefaultAsync(f => f.EthicsFormId == formId && f.UrecNo == urecNo);

            if (form == null || form.File == null)
            {
                return NotFound();
            }

            return File(form.File, "application/pdf");
        }

        [HttpGet]
        public async Task<IActionResult> ViewClearanceFile(string urecNo)
        {
            if (string.IsNullOrEmpty(urecNo))
            {
                return NotFound("UREC No. is missing.");
            }
            
            var clearance = await _context.CRE_EthicsClearance
                .FirstOrDefaultAsync(c => c.UrecNo == urecNo);

            if (clearance == null || clearance.ClearanceFile == null)
            {
                return NotFound("Clearance document not found.");
            }
            var stream = new MemoryStream(clearance.ClearanceFile);
            return File(stream, "application/pdf");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string formId, string urecNo)
        {
            var form = await _context.CRE_EthicsApplicationForms
                .FirstOrDefaultAsync(f => f.EthicsFormId == formId && f.UrecNo == urecNo);

            if (form == null || form.File == null)
            {
                return NotFound();
            }

            var filename = $"{formId}_{urecNo}.pdf";
            return File(form.File, "application/pdf", filename);
        }

        public async Task<IActionResult> ViewCertificateFile(string urecNo)
        {
            var certificate = await _context.CRE_CompletionCertificates
                .FirstOrDefaultAsync(c => c.UrecNo == urecNo);

            if (certificate == null || certificate.CertificateFile == null) 
            {
                return NotFound("Certificate not found."); 
            }

            return File(certificate.CertificateFile, "application/pdf"); 
        }
        [HttpGet]
        public async Task<IActionResult> DownloadClearanceFile(string urecNo)
        {
            // Fetch the clearance by urecNo
            var ethicsClearance = await _allServices.GetClearanceByUrecNoAsync(urecNo);

            // Check if clearance or file data exists
            if (ethicsClearance == null || ethicsClearance.ClearanceFile == null)
            {
                return NotFound("Clearance document not found.");
            }

            // Prepare the file download
            var fileContent = ethicsClearance.ClearanceFile;
            var fileName = "ClearanceDocument.pdf"; // Use a dynamic name if needed
            var contentType = "application/pdf"; // Adjust content type based on file type

            return File(fileContent, contentType, fileName);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyEthics(ApplyEthicsViewModel model)
        {
            bool titleExists = await _context.CRE_NonFundedResearchInfo
                                    .AnyAsync(r => r.Title == model.NonFundedResearchInfo.Title);

            if (titleExists)
            {
                ModelState.AddModelError("NonFundedResearchInfo.Title", "This research title already exists.");
                return View(model);
            }
            // Retrieve the logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Invalid user ID.");
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("", "User does not exist.");
                return View(model);
            }

            model.User = user;

            var ethicsApplication = model.EthicsApplication;
            ethicsApplication.UrecNo = await GenerateUrecNoAsync(); 
            ethicsApplication.UserId = userId;
            ethicsApplication.Name = model.FullName;
            ethicsApplication.SubmissionDate = DateTime.Now;

            // Prepare the NonFundedResearchInfo details
            var nonFundedResearchInfo = model.NonFundedResearchInfo;
            nonFundedResearchInfo.NonFundedResearchId = await GenerateNonFundedResearchIdAsync(); // Method to generate ID
            nonFundedResearchInfo.UserId = userId;
            nonFundedResearchInfo.Name = model.FullName;
            nonFundedResearchInfo.UrecNo = ethicsApplication.UrecNo;
            nonFundedResearchInfo.DateSubmitted = DateTime.Now;

            var ethicsApplyLog = new EthicsApplicationLogs
            {
                UrecNo = ethicsApplication.UrecNo,
                UserId = userId,
                Status = "Submit Documentary Requirements",
                ChangeDate = DateTime.Now
            };

            // Check if the title already exists
            var existingResearch = await _context.CRE_NonFundedResearchInfo
                .FirstOrDefaultAsync(n => n.Title == model.NonFundedResearchInfo.Title);
            if (existingResearch != null)
            {
                ModelState.AddModelError("NonFundedResearchInfo.title", "This title has already been used for another ethics application.");
                return View(model);
            }

            if (model.IsExternalResearcher && model.receiptFile != null)
            {
                // Handle file upload
                using (var memoryStream = new MemoryStream())
                {
                    await model.receiptFile.CopyToAsync(memoryStream);
                    model.ReceiptInfo = new ReceiptInfo
                    {
                        ScanReceipt = memoryStream.ToArray(),
                        UrecNo = ethicsApplication.UrecNo
                    };
                    _context.CRE_ReceiptInfo.Add(model.ReceiptInfo);
                }
            }
            else if (model.IsExternalResearcher)
            {
                ModelState.AddModelError("receiptFile", "Please upload a scanned receipt.");
                return View(model);
            }

            _context.CRE_EthicsApplication.Add(ethicsApplication);
            _context.CRE_NonFundedResearchInfo.Add(nonFundedResearchInfo);
            _context.CRE_EthicsApplicationLogs.Add(ethicsApplyLog);

            if (model.CoProponent != null && model.CoProponent.Any())
            {
                foreach (var coProponent in model.CoProponent)
                {
                    if (coProponent != null)
                    {
                        coProponent.NonFundedResearchId = nonFundedResearchInfo.NonFundedResearchId;
                        _context.CRE_CoProponents.Add(coProponent);
                    }
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your application has been submitted successfully.";
            if (!model.IsExternalResearcher)
            {
                return RedirectToAction("Applications");
            }

            return RedirectToAction("Applications");
        }

        /*Integration from REMC*/
        public async Task<IActionResult> ApplyFundedEthics(string id)
        {
            if (id == null)
            {
                return NotFound("No Funded Research Application Id found!");
            }
            var fr = await _remcDbContext.REMC_FundedResearchApplication.FirstOrDefaultAsync(f => f.fra_Id == id);
            if (fr == null)
            {
                return NotFound("No Funded Research Application found!");
            }

            bool titleExists = await _context.CRE_NonFundedResearchInfo.AnyAsync(r => r.Title == fr.research_Title);
            if (titleExists)
            {
                return BadRequest("This research title already exists.");
            }

            var ethicsApplication = new EthicsApplication
            {
                UrecNo = await GenerateUrecNoAsync(),
                UserId = fr.UserId,
                Name = fr.applicant_Name,
                SubmissionDate = DateTime.Now,
                FieldOfStudy = fr.field_of_Study
            };

            var nfri = new NonFundedResearchInfo
            {
                NonFundedResearchId = await GenerateNonFundedResearchIdAsync(),
                UserId = fr.UserId,
                Name = fr.applicant_Name,
                UrecNo = ethicsApplication.UrecNo,
                DateSubmitted = DateTime.Now,
                Title = fr.research_Title,
                College = fr.college == null ? null : fr.college,
                Campus = fr.branch == null ? null : fr.branch,
                University = "Polytechnic University of the Philippines"
            };

            var ethicsApplyLog = new EthicsApplicationLogs
            {
                UrecNo = ethicsApplication.UrecNo,
                UserId = fr.UserId,
                Name = fr.applicant_Name,
                Status = "Applied",
                ChangeDate = DateTime.Now,
            };

            _context.CRE_EthicsApplication.Add(ethicsApplication);
            _context.CRE_NonFundedResearchInfo.Add(nfri);
            _context.CRE_EthicsApplicationLogs.Add(ethicsApplyLog);

            if (fr.team_Members != null && fr.team_Members.Any())
            {
                foreach (var member in fr.team_Members)
                {
                    var coProponent = new CoProponent
                    {
                        NonFundedResearchId = nfri.NonFundedResearchId,
                        CoProponentName = member
                    };

                    _context.CRE_CoProponents.Add(coProponent);
                }
            }

            var fre = await _remcDbContext.REMC_FundedResearchEthics.FirstAsync(fre => fre.fra_Id == id);
            fre.urecNo = ethicsApplication.UrecNo;

            await _context.SaveChangesAsync();
            await _remcDbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your application has been submitted successfully.";
            return RedirectToAction("Applications");
        }


        public async Task<IActionResult> ConfirmCancelApplication(string applicationId)
        {
            var application = await _context.CRE_EthicsApplication
                .Include(a => a.NonFundedResearchInfo)
                .Include(a => a.EthicsApplicationLogs)
                .Include(a => a.EthicsApplicationForms)
            .FirstOrDefaultAsync(a => a.UrecNo == applicationId);

            if (application == null)
            {
                TempData["AlertMessage"] = "Application not found.";
                TempData["AlertType"] = "danger";
                return RedirectToAction("Applications");
            }

            var latestLog = application.EthicsApplicationLogs?.LastOrDefault();
            if (latestLog == null)
            {
                TempData["AlertMessage"] = "No logs found for this application.";
                TempData["AlertType"] = "danger";
                return RedirectToAction("Applications");
            }

            if (latestLog?.Status == "Applied")
            {
                if (!application.EthicsApplicationForms.Any())  // Ensure no forms are uploaded
                {
                    // Get the UrecNo associated with the application
                    var urecNo = application.UrecNo;

                    // Delete the associated records in NonFundedResearchInfo and ReceiptInfo
                    var nonFundedRecords = _context.CRE_NonFundedResearchInfo.Where(n => n.UrecNo == urecNo);
                    var receiptInfoRecords = _context.CRE_ReceiptInfo.Where(r => r.UrecNo == urecNo);


                    // Get current user's email for notification
                    var currentUser = await _userManager.GetUserAsync(User);
                    string currentUserEmail = currentUser?.Email;

                    string recipientName = application.Name;
                    string subject = "Your Ethics Application Has Been Cancelled";
                    string body = $@"
                <p>Your Ethics Application with the UrecNo <strong>{applicationId}</strong> and title <strong>{application.NonFundedResearchInfo.Title}</strong> has been successfully cancelled.</p>
    
                <p>Details of the cancellation:</p>
                <ul>
                    <li><strong>UrecNo:</strong> {applicationId}</li>
                    <li><strong>Title:</strong> {application.NonFundedResearchInfo.Title}</li>
                    <li><strong>Status:</strong> Cancelled</li>
                    <li><strong>Date of Cancellation:</strong> {DateTime.Now.ToString("MMMM dd, yyyy")}</li>
                    <li><strong>Action Taken:</strong> All associated records, including forms, logs, and related application data, have been permanently deleted from the system.</li>
                </ul>
    
                <p>This email serves as a confirmation that the application and its related records have been permanently removed from our system.</p>";

                    // Send email to the user confirming cancellation
                    await _emailService.SendEmailAsync(currentUserEmail, recipientName, subject, body);

                    // Remove related records
                    _context.CRE_NonFundedResearchInfo.RemoveRange(nonFundedRecords);
                    _context.CRE_ReceiptInfo.RemoveRange(receiptInfoRecords);

                    // Remove related logs (EthicsApplicationLog)
                    var relatedLogs = _context.CRE_EthicsApplicationLogs.Where(l => l.UrecNo == urecNo);
                    _context.CRE_EthicsApplicationLogs.RemoveRange(relatedLogs);

                    _context.CRE_EthicsApplication.Update(application); // Update the application status instead of deleting it

                    // Save changes
                    await _context.SaveChangesAsync();
                    TempData["AlertMessage"] = "Application and associated records have been cancelled and deleted successfully.";
                    TempData["AlertType"] = "success";
                }
                else
                {
                    TempData["AlertMessage"] = "Cannot cancel. Forms have already been uploaded.";
                    TempData["AlertType"] = "danger";
                }
            }
            else
            {
                TempData["AlertMessage"] = "Cannot cancel. The application is not in 'Applied' status.";
                TempData["AlertType"] = "danger";
            }

            return RedirectToAction("Applications");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDtsNo(string dtsNo, string urecNo)
        {
            // Ensure dtsNo and urecNo are received correctly
            if (string.IsNullOrEmpty(dtsNo) || string.IsNullOrEmpty(urecNo))
            {
                return BadRequest(new { success = false, message = "DTS No. and UREC No. are required." });
            }

            // Check if the DTS No already exists
            var existingDts = await _context.CRE_EthicsApplication
                .Where(e => e.DtsNo == dtsNo && e.UrecNo != urecNo)
                .AnyAsync();

            if (existingDts)
            {
                return Json(new { success = false, message = "The DTS No. is already in use. Please choose a different one." });
            }

            var application = await _context.CRE_EthicsApplication.FindAsync(urecNo);
            if (application != null)
            {
                application.DtsNo = dtsNo;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "DTS No. updated successfully." });
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> UploadForms(string urecNo)
        {
            if (string.IsNullOrEmpty(urecNo))
            {
                ModelState.AddModelError("", "UrecNo is missing.");
                return View("Error");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Invalid user ID.");
                return View("Error");
            }

            var ethicsApplication = await _context.CRE_EthicsApplication
                .FirstOrDefaultAsync(e => e.UrecNo == urecNo);

            var nonFundedResearchInfo = await _context.CRE_NonFundedResearchInfo
                .FirstOrDefaultAsync(n => n.UrecNo == urecNo);

            var receiptInfo = await _context.CRE_ReceiptInfo
                .FirstOrDefaultAsync(r => r.UrecNo == urecNo);

            var ethicsApplicationLogs = await _context.CRE_EthicsApplicationLogs
                .Where(l => l.UrecNo == urecNo)
                .ToListAsync();

            var latestComment = await _context.CRE_EthicsApplicationLogs
                .Where(log => log.EthicsApplication.UrecNo == urecNo)
                .OrderByDescending(log => log.ChangeDate)
                .Select(log => log.Comments) 
                .FirstOrDefaultAsync();

            var comment = latestComment ?? "No comments available";

            var ethicsApplicationForms = await _context.CRE_EthicsApplicationForms
                .Where(f => f.UrecNo == urecNo)
                .ToListAsync();

            var initialReview = await _context.CRE_InitialReview
                .FirstOrDefaultAsync(i => i.UrecNo == urecNo);

            var user = await _userManager.FindByIdAsync(userId);
            var clearance = await _context.CRE_EthicsClearance
                .FirstOrDefaultAsync(c => c.UrecNo == urecNo);

            var completionCertificate = await _context.CRE_CompletionCertificates
                .FirstOrDefaultAsync(c => c.UrecNo == urecNo);

            var completionReport = await _context.CRE_CompletionReports
                .FirstOrDefaultAsync(c => c.UrecNo == urecNo);

            var ethicsForms = await _context.CRE_EthicsForms.ToListAsync(); 

            // Ensure all necessary data exists
            if (ethicsApplication == null || nonFundedResearchInfo == null || user == null)
            {
                ModelState.AddModelError("", "Application or user data is missing.");
                return View("Error");
            }

            var coProponents = await _context.CRE_CoProponents
                .Where(c => c.NonFundedResearchId == nonFundedResearchInfo.NonFundedResearchId)
                .ToListAsync();

            // Populate the ViewModel
            var model = new UploadFormsViewModel
            {
                User = user,
                EthicsApplication = ethicsApplication,
                NonFundedResearchInfo = nonFundedResearchInfo,
                ReceiptInfo = receiptInfo,
                EthicsApplicationForms = ethicsApplicationForms,
                EthicsApplicationLog = ethicsApplicationLogs,
                LatestComment = latestComment, // Add the latest comment to the ViewModel
                CoProponent = coProponents.ToList(),
                EthicsClearance = clearance,
                InitialReview = initialReview, // Ensure it's a List
                CompletionCertificate = completionCertificate, // Add CompletionCertificate data
                CompletionReport = completionReport, // Add CompletionReport data
                EthicsForms = ethicsForms // Add the EthicsForms data to the ViewModel
            };

            return View(model);
        }

        public async Task<IActionResult> ViewReceiptAsync(string urecNo)
        {
            var receiptInfo = await _context.CRE_ReceiptInfo
                .FirstOrDefaultAsync(r => r.UrecNo == urecNo);

            if (receiptInfo == null || receiptInfo.ScanReceipt == null)
            {
                return NotFound(); 
            }
            return File(receiptInfo.ScanReceipt, "application/pdf");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadForms(UploadFormsViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Invalid user ID.");
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var existingForms = await _context.CRE_EthicsApplicationForms
                .Where(f => f.UrecNo == model.EthicsApplication.UrecNo)
                .ToListAsync();

            if (existingForms.Any())
            {
                model.FORM9 = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.EthicsFormId == "FORM9")?.File, "FORM9.pdf");
                model.FORM10 = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.EthicsFormId == "FORM10")?.File, "FORM10.pdf");
                model.FORM10_1 = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.EthicsFormId == "FORM10_1")?.File, "FORM10_1.pdf");
                model.FORM11 = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.EthicsFormId == "FORM11")?.File, "FORM11.pdf");
                model.FORM12 = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.EthicsFormId == "FORM12")?.File, "FORM12.pdf");
                model.CAA = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.EthicsFormId == "CAA")?.File, "CAA.pdf");
                model.RCV = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.EthicsFormId == "RCV")?.File, "RCV.pdf");
                model.CV = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.EthicsFormId == "CV")?.File, "CV.pdf");
                model.LI = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.EthicsFormId == "LI")?.File, "LI.pdf");
                model.QST = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.EthicsFormId == "QST")?.File, "QST.pdf");
                model.CR = FileHelper.ConvertToFormFile(existingForms.FirstOrDefault(f => f.EthicsFormId == "CR")?.File, "CR.pdf"); 
            }


            var fileProperties = new List<(string PropertyName, IFormFile NewFile, IFormFile EditFile)>
            {
                (nameof(model.FORM9), model.FORM9, model.editFORM9),
                (nameof(model.FORM10), model.FORM10, model.editFORM10),
                (nameof(model.FORM10_1), model.FORM10_1, model.editFORM10_1),
                (nameof(model.FORM11), model.FORM11, model.editFORM11),
                (nameof(model.FORM12), model.FORM12, model.editFORM12),
                (nameof(model.CAA), model.CAA, model.editCAA),
                (nameof(model.RCV), model.RCV, model.editRCV),
                (nameof(model.CV), model.CV, model.editCV),
                (nameof(model.LI), model.LI, model.editLI),
                (nameof(model.QST), model.QST, model.editQST),
                (nameof(model.CR), model.CR, model.editCR)
            };

            foreach (var (propertyName, newFile, editFile) in fileProperties)
            {
                var fileToUpload = editFile != null && editFile.Length > 0 ? editFile : newFile;

                if (fileToUpload != null && fileToUpload.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await fileToUpload.CopyToAsync(memoryStream);

                        // Check if the form already exists in the database
                        var existingForm = await _context.CRE_EthicsApplicationForms
                            .FirstOrDefaultAsync(f => f.EthicsFormId == propertyName && f.UrecNo == model.EthicsApplication.UrecNo);

                        if (existingForm != null)
                        {
                            // Update the existing form with the new or edited file
                            existingForm.File = memoryStream.ToArray();
                            existingForm.DateUploaded = DateTime.UtcNow;
                            existingForm.FileName = $"{propertyName}_{model.EthicsApplication.UrecNo}.pdf";
                            _context.CRE_EthicsApplicationForms.Update(existingForm);
                        }
                        else
                        {
                            // Create a new instance of the EthicsApplicationForms object
                            var ethicsApplicationForm = new EthicsApplicationForms
                            {
                                UrecNo = model.EthicsApplication.UrecNo,
                                EthicsFormId = propertyName,
                                DateUploaded =DateTime.UtcNow,
                                File = memoryStream.ToArray(),
                                FileName = $"{propertyName}_{model.EthicsApplication.UrecNo}.pdf"
                            };

                            // Save the new form to the database
                            await _context.CRE_EthicsApplicationForms.AddAsync(ethicsApplicationForm);
                        }
                        await _context.SaveChangesAsync();  // Save changes to the database
                    }
                }
            }

            // Create a log entry for the forms upload
            var uploadFormLog = new EthicsApplicationLogs
            {
                UrecNo = model.EthicsApplication.UrecNo,
                UserId = userId,
                Status = "Pending for Evaluation",
                ChangeDate = DateTime.Now
            };
            await _context.CRE_EthicsApplicationLogs.AddAsync(uploadFormLog);

            var initialReview = await _context.CRE_InitialReview
                .FirstOrDefaultAsync(ir => ir.UrecNo == model.EthicsApplication.UrecNo);

            if (initialReview != null)
            {
                initialReview.Status = "Pending";
                _context.CRE_InitialReview.Update(initialReview);
            }
            await _context.SaveChangesAsync();

            var application = await _context.CRE_EthicsApplication
                .Include(app => app.NonFundedResearchInfo)
                .FirstOrDefaultAsync(app => app.UrecNo == model.EthicsApplication.UrecNo);

          
            var currentUser = await _userManager.GetUserAsync(User);

            string recipientName = application.NonFundedResearchInfo.Name;
            string currentUserEmail = currentUser?.Email;
            string subject = "Your Ethics Application is Being Processed for Initial Review";
            string body = $@"
                <p>Thank you for submitting your forms. Your Ethics Application (UrecNo: <strong>{model.EthicsApplication.UrecNo}</strong>) is now being processed.</p>
                <p>Your application will undergo an initial review to check for any errors in the forms.</p>
                <p>We will notify you once the initial review is complete or if any corrections are needed.</p>";

            // Send email to the user notifying them that the application is being processed
            await _emailService.SendEmailAsync(currentUserEmail, recipientName, subject, body);


            // Log that the user has uploaded their forms
            TempData["SuccessMessage"] = "Forms uploaded successfully!";

            // Redirect after processing
            return RedirectToAction("UploadForms", new { urecNo = model.EthicsApplication.UrecNo });
        }


        [HttpGet]
        public async Task<IActionResult> TrackApplication(string urecNo)
        {
            // Fetch the application, related NonFundedResearchInfo, and logs in one query using Include
            var applicationWithDetails = await _context.CRE_EthicsApplication
                .Where(app => app.UrecNo == urecNo)
                .Include(app => app.InitialReview)
                .Include(app => app.NonFundedResearchInfo)
                    .ThenInclude(info => info.CoProponents)
                .Include(app => app.EthicsApplicationLogs)  // Fetch related logs
                .FirstOrDefaultAsync();

            // If no application is found or no logs exist, show message and redirect
            if (applicationWithDetails == null || !applicationWithDetails.EthicsApplicationLogs.Any())
            {
                TempData["ErrorMessage"] = "No logs available for the provided UREC No.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            // Create the ViewModel
            var viewModel = new TrackApplicationViewModel
            {
                Application = applicationWithDetails,
                InitialReview = applicationWithDetails.InitialReview,
                NonFundedResearchInfo = applicationWithDetails.NonFundedResearchInfo,
                CoProponents = applicationWithDetails.NonFundedResearchInfo?.CoProponents?.ToList() ?? new List<CoProponent>(),
                Logs = applicationWithDetails.EthicsApplicationLogs.ToList()
            };

            // Pass the ViewModel to the view
            return View(viewModel);
        }


        public async Task<IActionResult> ViewNotifications()
        {
            // Get the logged-in user's ID
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not logged in.");
            }

            // Get the current role from the session
            string currentRole = HttpContext.Session.GetString("CurrentRole");

            if (string.IsNullOrEmpty(currentRole))
            {
                return Unauthorized("No role selected.");
            }

            // Fetch notifications based on the current role
            var notifications = await _context.CRE_EthicsNotifications
                .Where(n => n.Role == currentRole) // Filter notifications by the current role
                .OrderByDescending(n => n.NotificationCreationDate) // Order by latest first
                .ToListAsync();

            // Mark notifications as read (set NotificationStatus to true)
            foreach (var notification in notifications)
            {
                if (!notification.NotificationStatus)
                {
                    notification.NotificationStatus = true; // Mark as read
                    _context.CRE_EthicsNotifications.Update(notification);
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Map to ViewModel for easier display in the view
            var notificationViewModels = notifications.Select(n => new NotificationViewModel
            {
                NotificationId = n.NotificationId,
                NotificationTitle = n.NotificationTitle,
                NotificationMessage = n.NotificationMessage,
                NotificationCreationDate = n.NotificationCreationDate,
                NotificationStatus = n.NotificationStatus
            }).ToList();

            return View(notificationViewModels); // Pass notifications to the view
        }

        [HttpGet]
        public async Task<IActionResult> ApplyForCompletionCertificate(string urecNo)
        {
            // Retrieve the required entities from the database based on urecNo
            var ethicsApplication = await _context.CRE_EthicsApplication
                .FirstOrDefaultAsync(ea => ea.UrecNo == urecNo);

            if (ethicsApplication == null)
            {
                return NotFound("The specified application data was not found.");
            }

            var ethicsClearance = await _context.CRE_EthicsClearance
                .FirstOrDefaultAsync(ec => ec.UrecNo == urecNo);

            var nonFundedResearchInfo = await _context.CRE_NonFundedResearchInfo
                .FirstOrDefaultAsync(nfri => nfri.UrecNo == urecNo);

            if (nonFundedResearchInfo == null)
            {
                return NotFound("The associated non-funded research information was not found.");
            }

            var coProponents = await _context.CRE_CoProponents
                .Where(cp => cp.NonFundedResearchId == nonFundedResearchInfo.NonFundedResearchId)
                .ToListAsync();

            var ethicsApplicationLog = await _context.CRE_EthicsApplicationLogs
                .Where(log => log.UrecNo == urecNo)
                .ToListAsync();

            // Construct the view model
            var viewModel = new ApplyForCompletionCertificateViewModel
            {
                EthicsApplication = ethicsApplication,
                EthicsClearance = ethicsClearance,
                NonFundedResearchInfo = nonFundedResearchInfo,
                CoProponents = coProponents,
                EthicsApplicationLog = ethicsApplicationLog
            };

            // Return the view with the populated model
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyForCompletionCertificate(string urecNo, IFormFile form18, DateOnly researchStartDate, DateOnly researchEndDate)
        {
            DateTime startDateTime = researchStartDate.ToDateTime(TimeOnly.MinValue); // Start of the day
            DateTime endDateTime = researchEndDate.ToDateTime(TimeOnly.MinValue); // Start of the day
            // Manually validate the DateOnly fields
            if (researchStartDate > researchEndDate)
            {
                TempData["ErrorMessage"] = "Research end date cannot be before the research start date.";
                return RedirectToAction("UploadForms", "Researcher", new { urecNo = urecNo });
            }

            // Manually validate the form18 (check if file is provided)
            if (form18 == null || form18.Length == 0)
            {
                TempData["ErrorMessage"] = "Form 18 is required.";
                return RedirectToAction("UploadForms", "Researcher", new { urecNo = urecNo });
            }

            // Check if the researchEndDate is valid (it should be in the past or present)
            if (researchEndDate > DateOnly.FromDateTime(DateTime.Now))
            {
                TempData["ErrorMessage"] = "Research end date cannot be in the future.";
                return RedirectToAction("UploadForms", "Researcher", new { urecNo = urecNo });
            }

            // If all validations pass, proceed with the service call
            bool success = await _allServices.SubmitTerminalReportAsync(
                urecNo, // UREC number
                form18, // The uploaded file (Form 18)
                startDateTime, // Research start date
                endDateTime // Research end date
            );

            if (success)
            {
                // Add the log using _context
                var log = new EthicsApplicationLogs
                {
                    UrecNo = urecNo,
                    Status = "Terminal Report Submitted",
                    ChangeDate = DateTime.UtcNow,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier), // Get the logged-in user's ID
                    Comments = "Terminal report submitted successfully."
                };

                _context.CRE_EthicsApplicationLogs.Add(log); // Add the log to the DbSet
                await _context.SaveChangesAsync(); // Save changes to the database

                // Set the success message in TempData
                TempData["SuccessMessage"] = "Terminal report submitted successfully.";

                // Redirect to the target action with urecNo parameter
                return RedirectToAction("UploadForms", "Researcher", new { urecNo = urecNo });
            }
            else
            {
                // If the service call fails
                TempData["ErrorMessage"] = "There was an issue submitting the terminal report.";
                return RedirectToAction("UploadForms", "Researcher", new { urecNo = urecNo });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadForm15(IFormFile FORM15, string urecNo)
        {
            if (string.IsNullOrEmpty(urecNo))
            {
                TempData["ErrorMessage"] = "UrecNo is required.";
                return RedirectToAction("UploadForms");
            }

            if (FORM15 == null || FORM15.Length == 0)
            {
                ModelState.AddModelError("FORM15", "Please upload a valid PDF file.");
                return RedirectToAction("UploadForms");
            }

            if (FORM15.ContentType != "application/pdf")
            {
                ModelState.AddModelError("FORM15", "Only PDF files are allowed.");
                return RedirectToAction("UploadForms");
            }

            byte[] fileData;
            string fileName;

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await FORM15.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                    fileName = FORM15.FileName;
                }

                var form15 = new EthicsApplicationForms
                {
                    UrecNo = urecNo,
                    EthicsFormId = "FORM15",
                    File = fileData,
                    DateUploaded = DateTime.Now,
                    FileName = fileName
                };

                var success = await _allServices.SaveEthicsFormAsync(form15);

                if (success)
                {
                    var logEntry = new EthicsApplicationLogs
                    {
                        UrecNo = urecNo,
                        Status = "Amendment form Uploaded",
                        Comments = "Form 15 uploaded for revisions.",
                        ChangeDate = DateTime.Now,
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    };

                    await _allServices.AddLogAsync(logEntry);

                    TempData["SuccessMessage"] = "Form 15 uploaded and status updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while uploading Form 15.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Applications");
        }
    }
}
