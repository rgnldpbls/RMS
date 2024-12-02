using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResearchManagementSystem.Areas.CreSys.Data;
using ResearchManagementSystem.Areas.CreSys.Interfaces;
using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Areas.CreSys.ViewModels;
using ResearchManagementSystem.Areas.CreSys.ViewModels.ListViewModels;
using ResearchManagementSystem.Models;
using System.Security.Claims;

namespace ResearchManagementSystem.Areas.CreSys.Controllers
{
    [Area("CreSys")]
    [Authorize]
    public class SecretariatController : Controller
    {
        private readonly CreDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEthicsEmailService _emailService;
        public SecretariatController(CreDbContext context, UserManager<ApplicationUser> userManager, IEthicsEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
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
                PendingApplications = await GetPendingApplicationsAsync(),
                ApprovedApplications = await GetApprovedApplicationsAsync(),
                ReturnedApplications = await GetReturnedApplicationsAsync(),
            };

            return View(viewModel);
        }

        private async Task<IEnumerable<InitialReviewViewModel>> GetPendingApplicationsAsync()
        {
            return await _context.CRE_EthicsApplication
             .Include(e => e.NonFundedResearchInfo)
                 .ThenInclude(nf => nf.CoProponents)
             .Include(e => e.EthicsApplicationLogs)
             .Where(e => e.EthicsApplicationLogs
                 .OrderByDescending(log => log.ChangeDate)
                 .FirstOrDefault().Status == "Pending for Evaluation")
             .Select(e => new InitialReviewViewModel
             {
                 EthicsApplication = e,
                 NonFundedResearchInfo = e.NonFundedResearchInfo,
                 CoProponent = e.NonFundedResearchInfo.CoProponents,
                 EthicsApplicationLog = e.EthicsApplicationLogs
                     .OrderByDescending(log => log.ChangeDate)
                     .Take(1),
             })
             .ToListAsync();
        }


        private async Task<IEnumerable<InitialReviewViewModel>> GetApprovedApplicationsAsync()
        {
            var currentYear = DateTime.UtcNow.Year;
            return await _context.CRE_EthicsApplication
                .Include(e => e.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponents)
                .Include(e => e.EthicsApplicationLogs)
                .Where(e => e.EthicsApplicationLogs
                    .OrderByDescending(log => log.ChangeDate)
                    .FirstOrDefault().Status == "Approved for Evaluation")
                .Select(e => new InitialReviewViewModel
                {
                    EthicsApplication = e, 
                    NonFundedResearchInfo = e.NonFundedResearchInfo,
                    CoProponent = e.NonFundedResearchInfo.CoProponents, 
                    EthicsApplicationLog = e.EthicsApplicationLogs
                        .OrderByDescending(log => log.ChangeDate)
                        .Take(1)
                })
                .ToListAsync();
        }


        private async Task<IEnumerable<InitialReviewViewModel>> GetReturnedApplicationsAsync()
        {
            return await _context.CRE_EthicsApplication
                .Include(e => e.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponents)
                .Include(e => e.EthicsApplicationLogs)
                .Where(e => e.EthicsApplicationLogs
                    .OrderByDescending(log => log.ChangeDate)
                    .FirstOrDefault().Status == "Returned for Revisions")
                .Select(e => new InitialReviewViewModel
                {
                    EthicsApplication = e,
                    NonFundedResearchInfo = e.NonFundedResearchInfo,
                    CoProponent = e.NonFundedResearchInfo.CoProponents,
                    EthicsApplicationLog = e.EthicsApplicationLogs
                        .OrderByDescending(log => log.ChangeDate)
                        .Take(1)
                })
                .ToListAsync();
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
                var application = await _context.CRE_EthicsApplication
                    .Include(e => e.NonFundedResearchInfo)
                        .ThenInclude(nf => nf.CoProponents)
                    .Include(e => e.EthicsEvaluation)
                    .Include(e => e.ReceiptInfo)
                    .Include(e => e.EthicsApplicationLogs)
                    .Include(e => e.EthicsApplicationForms)
                    .Include(e => e.InitialReview)
                    .FirstOrDefaultAsync(e => e.UrecNo == urecNo);

                // If the application is not found, return NotFound
                if (application == null)
                {
                    return NotFound("Application not found.");
                }

                // Convert the list of ethics evaluations (can be empty)
                var ethicsEvaluations = application.EthicsEvaluation.ToList();

                // Retrieve the list of EthicsEvaluator user IDs
                var evaluatorUserIds = ethicsEvaluations
                    .Select(e => e.UserId) // Assuming you're storing UserId for the evaluator
                    .Distinct() // Ensure no duplicates
                    .ToList();

                // Retrieve the actual EthicsEvaluator details from the EthicsEvaluator table
                var ethicsEvaluators = await _context.CRE_EthicsEvaluator
                    .Where(e => evaluatorUserIds.Contains(e.UserID))
                    .ToListAsync();

                var appUser = await _userManager.FindByIdAsync(application.UserId);

                var viewModel = new InitialReviewViewModel
                {
                    EthicsApplication = application,
                    NonFundedResearchInfo = application.NonFundedResearchInfo,
                    CoProponent = application.NonFundedResearchInfo?.CoProponents ?? new List<CoProponent>(),
                    ReceiptInfo = application.ReceiptInfo,
                    User = appUser,
                    EthicsApplicationLog = application.EthicsApplicationLogs.OrderByDescending(log => log.ChangeDate),
                    EthicsApplicationForms = application.EthicsApplicationForms ?? new List<EthicsApplicationForms>(),
                    InitialReview = await _context.CRE_InitialReview.FirstOrDefaultAsync(ir => ir.UrecNo == urecNo),
                    EthicsEvaluation = ethicsEvaluations,
                    EthicsEvaluator = ethicsEvaluators.FirstOrDefault(), // Here, we add the full list of evaluators
                    DeclinedEthicsEvaluation = application.DeclinedEthicsEvaluation != null
                        ? new List<DeclinedEthicsEvaluation> { application.DeclinedEthicsEvaluation }
                        : new List<DeclinedEthicsEvaluation>(),
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Secretariat")]
         
        [HttpPost]
        public async Task<IActionResult> ApproveApplication(string urecNo, string comments)
        {
            if (string.IsNullOrEmpty(urecNo))
            {
                return BadRequest("Invalid UrecNo.");
            }

            // Fetch the userId from the current logged-in user
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the existing initial review
            var initialReview = await _context.CRE_InitialReview
                .FirstOrDefaultAsync(ir => ir.UrecNo == urecNo);

            if (initialReview == null)
            {
                // If no existing review, create a new one
                initialReview = new InitialReview
                {
                    UrecNo = urecNo,
                    Status = "Approved",
                    UserId = userId,
                    Feedback = comments,
                    DateReviewed = DateTime.Now,
                    ReviewType = "Pending"
                };

                // Add the new initial review to the context
                await _context.CRE_InitialReview.AddAsync(initialReview);
            }
            else
            {
                // Update the existing initial review
                initialReview.Status = "Approved";
                initialReview.UserId = userId;
                initialReview.Feedback = comments;
                initialReview.DateReviewed = DateTime.Now;
                initialReview.ReviewType = "Pending";

                // Update the record
                _context.CRE_InitialReview.Update(initialReview);
            }

            // Create a new log entry for the Ethics Application
            var logEntry = new EthicsApplicationLogs
            {
                UrecNo = urecNo,
                Status = "Approved for Evaluation",
                UserId = userId,
                ChangeDate = DateTime.Now,
                Comments = comments
            };

            // Add the new log entry to the context
            await _context.CRE_EthicsApplicationLogs.AddAsync(logEntry);

            // Save changes to both the initial review and the log
            await _context.SaveChangesAsync();

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

            // Prepare email notification
            string recipientName = application.NonFundedResearchInfo.Name;
            string userEmail = user.Email;
            string subject = "Your Ethics Application Forms Has Been Approved";
            string body = $@"
                <p>We are pleased to inform you that the forms of your Ethics Application (UrecNo: <strong>{urecNo}</strong>) has been approved. 
                    All submitted forms have been reviewed and found to be correct.</p>
                <p>It would then be further processed to be evaluated.</p>
                <p>Thank you.</p>";

            // Send email notification
            await _emailService.SendEmailAsync(userEmail, recipientName, subject, body);

            string userRole = await GetUserRole(application.NonFundedResearchInfo.UserId); // Method to fetch the user's role

            // Add notification record
            var notification = new EthicsNotifications
            {
                UrecNo = urecNo,
                UserId = application.NonFundedResearchInfo.UserId,
                NotificationTitle = "Application Approved",
                NotificationMessage = $"Your Ethics Application (UrecNo: {urecNo}) has been approved.",
                NotificationCreationDate = DateTime.Now,
                NotificationStatus = false, // Unread
                Role = userRole, // Role is dynamically assigned based on the user
                PerformedBy = "System"
            };
            await _context.CRE_EthicsNotifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction("InitialReview");
        }

        [Authorize(Roles = "Secretariat")]
         
        [HttpPost]
        public async Task<IActionResult> ReturnApplication(string urecNo, string comments)
        {
            if (string.IsNullOrEmpty(urecNo))
            {
                return BadRequest("Invalid UrecNo.");
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var initialReview = await _context.CRE_InitialReview
                .FirstOrDefaultAsync(ir => ir.UrecNo == urecNo);

            if (initialReview == null)
            {
                initialReview = new InitialReview
                {
                    UrecNo = urecNo,
                    Status = "Returned",
                    Feedback = comments,
                    UserId = userId,
                    DateReviewed = DateTime.Now,
                    ReviewType = "Pending"
                };

                await _context.CRE_InitialReview.AddAsync(initialReview);
            }
            else
            {
                initialReview.Status = "Returned";
                initialReview.Feedback = comments;
                initialReview.UserId = userId;
                initialReview.DateReviewed = DateTime.Now;

                _context.CRE_InitialReview.Update(initialReview);
            }

            var logEntry = new EthicsApplicationLogs
            {
                UrecNo = urecNo,
                Status = "Returned for Revisions", 
                UserId = userId,
                ChangeDate = DateTime.Now,
                Comments = comments
            };

            await _context.CRE_EthicsApplicationLogs.AddAsync(logEntry);

            await _context.SaveChangesAsync();

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
            string subject = "Your Ethics Application Forms Has Been Returned";
            string body = $@"
                <p>We regret to inform you that the forms of your Ethics Application (UrecNo: <strong>{urecNo}</strong>) have been returned. 
                Please review the feedback below and make the necessary revisions:</p>
                <p><strong>Feedback:</strong> {comments}</p>
                <p>Once you have made the required changes, kindly resubmit the forms.</p>
                <p>Thank you.</p>";

            await _emailService.SendEmailAsync(userEmail, recipientName, subject, body);

            string userRole = await GetUserRole(application.NonFundedResearchInfo.UserId); 

            var notification = new EthicsNotifications
            {
                UrecNo = urecNo,
                UserId = application.NonFundedResearchInfo.UserId,
                NotificationTitle = "Application Returned",
                NotificationMessage = $"Your Ethics Application (UrecNo: {urecNo}) has been returned for revisions.",
                NotificationCreationDate = DateTime.Now,
                NotificationStatus = false,
                Role = userRole,
                PerformedBy = "System"
            };

            await _context.CRE_EthicsNotifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction("InitialReview");
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
        
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
