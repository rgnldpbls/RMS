using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using CRE.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResearchManagementSystem.Models;
using static System.Net.Mime.MediaTypeNames;

namespace CRE.Services
{
    public class InitialReviewServices : IInitialReviewServices
    {
        private readonly CreDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public InitialReviewServices(CreDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IEnumerable<InitialReviewViewModel>> GetEthicsApplicationsForInitialReviewAsync()
        {
            // Step 1: Get all the latest logs with "Forms Uploaded"
            var latestLogs = await _context.EthicsApplicationLog
                .Where(log => log.status == "Pending for Evaluation")
                .GroupBy(log => log.urecNo) // Group by UrecNo to get the latest log per application
                .Select(g => g.OrderByDescending(log => log.changeDate).FirstOrDefault()) // Get the latest log in each group
                .ToListAsync();

            // Step 2: Retrieve all EthicsApplications and their forms for the found logs
            var ethicsApplications = new List<InitialReviewViewModel>();

            foreach (var log in latestLogs)
            {
                var ethicsApplication = await _context.EthicsApplication
                    .FirstOrDefaultAsync(e => e.urecNo == log.urecNo);
                // Ensure you also retrieve NonFundedResearchInfo if it's linked
                var nonFundedResearchInfo = await _context.NonFundedResearchInfo
                    .FirstOrDefaultAsync(nf => nf.urecNo == log.urecNo); // Use appropriate link

                var ethicsApplicationForms = await _context.EthicsApplicationForms
                    .Where(form => form.urecNo == log.urecNo)
                    .ToListAsync();

                ethicsApplications.Add(new InitialReviewViewModel
                {
                    EthicsApplication = ethicsApplication,
                    EthicsApplicationForms = ethicsApplicationForms,
                    NonFundedResearchInfo = nonFundedResearchInfo,
                    EthicsApplicationLog = new List<EthicsApplicationLog> { log } // Add the log if needed
                });
            }

            return ethicsApplications;
        }
        public async Task<EthicsEvaluation> GetEthicsEvaluationAsync(string urecNo)
        {
            // Retrieve the single evaluation associated with the specified exempt application
            return await _context.EthicsEvaluation
                .Where(eval => eval.EthicsApplication.urecNo == urecNo)
                .FirstOrDefaultAsync();
        }


        public async Task UpdateInitialReviewAsync(InitialReview initialReview)
        {
            _context.InitialReview.Update(initialReview);
            await _context.SaveChangesAsync();
        }
        // New method for getting application details
        public async Task<InitialReviewViewModel> GetApplicationDetailsAsync(string urecNo)
        {
            var application = await _context.EthicsApplication
                .Include(e => e.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponent)
                .Include(e => e.EthicsEvaluation)
                    .ThenInclude(e => e.EthicsEvaluator)
                .Include(e => e.EthicsEvaluationDeclined)
                .Include(e => e.ReceiptInfo)
                .Include(e => e.EthicsApplicationLog)
                .Include(e => e.EthicsApplicationForms)
                .Include(e => e.InitialReview) // Include InitialReview directly
                .FirstOrDefaultAsync(e => e.urecNo == urecNo);

            if (application == null)
            {
                throw new Exception("Application not found.");
            }

            var appUser = await _userManager.FindByIdAsync(application.userId);

            var ethicsEvaluations = application.EthicsEvaluation.ToList(); // Convert to a list if needed

            var viewModel = new InitialReviewViewModel
            {
                EthicsApplication = application,
                NonFundedResearchInfo = application.NonFundedResearchInfo,
                CoProponent = application.NonFundedResearchInfo?.CoProponent ?? new List<CoProponent>(),
                ReceiptInfo = application.ReceiptInfo,
                EthicsApplicationLog = application.EthicsApplicationLog.OrderByDescending(log => log.changeDate),
                EthicsApplicationForms = application.EthicsApplicationForms ?? new List<EthicsApplicationForms>(),
                InitialReview = await _context.InitialReview.FirstOrDefaultAsync(ir => ir.urecNo == urecNo),
                EthicsEvaluation = ethicsEvaluations,
                EthicsEvaluator = ethicsEvaluations.Select(e => e.EthicsEvaluator).FirstOrDefault(),
                EthicsEvaluationDeclined = application.EthicsEvaluationDeclined // Assign declined evaluations
            };

            return viewModel;
        }

        public async Task<IEnumerable<ChiefEvaluationViewModel>> GetExemptApplicationsAsync()
        {
            return await _context.EthicsApplication
                .Where(app => app.InitialReview.ReviewType == "Exempt")
                .Select(app => new ChiefEvaluationViewModel
                {
                    NonFundedResearchInfo = app.NonFundedResearchInfo,
                    ReceiptInfo = app.ReceiptInfo,
                    EthicsApplication = app,
                    InitialReview = app.InitialReview,
                    EthicsApplicationForms = app.EthicsApplicationForms,
                    EthicsApplicationLog = app.EthicsApplicationLog,
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<CoProponent>> GetCoProponentsByNonFundedResearchIdAsync(string nonFundedResearchId)
        {
            return await _context.CoProponent
                .Where(cp => cp.nonFundedResearchId == nonFundedResearchId)
                .ToListAsync();
        }

        public async Task ApproveApplicationAsync(string urecNo, string comments, string userId)
        {
            // Find the existing initial review
            var initialReview = await _context.InitialReview
                .FirstOrDefaultAsync(ir => ir.urecNo == urecNo);

            if (initialReview == null)
            {
                // If no existing review, create a new one
                initialReview = new InitialReview
                {
                    urecNo = urecNo,
                    status = "Approved",
                    userId = userId,
                    feedback = comments,
                    dateReviewed = DateOnly.FromDateTime(DateTime.Now),
                    ReviewType = "Pending"
                };

                // Add the new initial review to the context
                await _context.InitialReview.AddAsync(initialReview);
            }
            else
            {
                // Update the existing initial review
                initialReview.status = "Approved";
                initialReview.userId = userId;
                initialReview.feedback = comments;
                initialReview.dateReviewed = DateOnly.FromDateTime(DateTime.Now);
                initialReview.ReviewType = "Pending";

                // Update the record
                _context.InitialReview.Update(initialReview);
            }

            // Create a new log entry for the Ethics Application
            var logEntry = new EthicsApplicationLog
            {
                urecNo = urecNo,
                status = "Approved for Evaluation",
                userId = userId,
                changeDate = DateTime.Now,
                comments = comments
            };

            // Add the new log entry to the context
            await _context.EthicsApplicationLog.AddAsync(logEntry);

            // Save changes to both the initial review and the log
            await _context.SaveChangesAsync();
        }

        public async Task ReturnApplicationAsync(string urecNo, string comments, string userId)
        {
            // Find the existing initial review
            var initialReview = await _context.InitialReview
                .FirstOrDefaultAsync(ir => ir.urecNo == urecNo);

            if (initialReview == null)
            {
                // If no existing review, create a new one
                initialReview = new InitialReview
                {
                    urecNo = urecNo,
                    status = "Returned",
                    feedback = comments,
                    userId = userId,
                    dateReviewed = DateOnly.FromDateTime(DateTime.Now),
                    ReviewType = "Pending"
                };

                // Add the new initial review to the context
                await _context.InitialReview.AddAsync(initialReview);
            }
            else
            {
                // Update the existing initial review
                initialReview.status = "Returned";
                initialReview.feedback = comments;
                initialReview.userId = userId; 
                initialReview.dateReviewed = DateOnly.FromDateTime(DateTime.Now);

                // Update the record
                _context.InitialReview.Update(initialReview);
            }

            // Create a new log entry for the Ethics Application
            var logEntry = new EthicsApplicationLog
            {
                urecNo = urecNo,
                status = "Returned for Revisions", // Update the status to reflect the return
                userId = userId,
                changeDate = DateTime.Now,
                comments = comments
            };

            // Add the new log entry to the context
            await _context.EthicsApplicationLog.AddAsync(logEntry);

            // Save changes to both the initial review and the log
            await _context.SaveChangesAsync();
        }
        // Fetch pending applications for initial review
        public async Task<IEnumerable<InitialReviewViewModel>> GetPendingApplicationsAsync()
        {
            return await _context.EthicsApplication
                .Include(e => e.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponent)
                .Include(e => e.EthicsApplicationLog)
                .Where(e => e.EthicsApplicationLog
                    .OrderByDescending(log => log.changeDate)  // Get logs in descending order by changeDate
                    .FirstOrDefault().status == "Pending for Evaluation") // Check if the latest log's status is "Pending"
                .Select(e => new InitialReviewViewModel
                {
                    EthicsApplication = e,
                    NonFundedResearchInfo = e.NonFundedResearchInfo,
                    // Fetch only the latest log with status "Pending for Evaluation"
                    EthicsApplicationLog = e.EthicsApplicationLog
                        .OrderByDescending(log => log.changeDate)
                        .Take(1) // Take only the latest log entry
                })
                .ToListAsync();
        }

        // Fetch approved applications for initial review
        public async Task<IEnumerable<InitialReviewViewModel>> GetApprovedApplicationsAsync()
        {
            var currentYear = DateTime.UtcNow.Year; // Get the current year

            return await _context.EthicsApplication
                .Include(e => e.NonFundedResearchInfo)
                   .ThenInclude(nf => nf.CoProponent)
                .Include(e => e.EthicsApplicationLog)
                .Where(e => e.EthicsApplicationLog
                    .Where(log => log.status == "Approved for Evaluation" && log.changeDate.Year == currentYear) // Filter logs to only include those approved this year
                    .OrderByDescending(log => log.changeDate)  // Get logs in descending order by changeDate
                    .Any() // Check if there are any logs meeting the criteria
                )
                .Select(e => new InitialReviewViewModel
                {
                    EthicsApplication = e,
                    NonFundedResearchInfo = e.NonFundedResearchInfo,
                    // Fetch only the latest log with status "Approved for Evaluation"
                    EthicsApplicationLog = e.EthicsApplicationLog
                        .Where(log => log.status == "Approved for Evaluation" && log.changeDate.Year == currentYear) // Ensure to only take logs from this year
                        .OrderByDescending(log => log.changeDate)
                        .Take(1) // Take only the latest log entry
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<InitialReviewViewModel>> GetReturnedApplicationsAsync()
        {
            // Fetch applications that have been returned
            return await _context.EthicsApplication
                .Include(e => e.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponent)
                .Include(e => e.EthicsApplicationLog)
                .Where(e => e.EthicsApplicationLog
                    .OrderByDescending(log => log.changeDate) // Order logs by changeDate
                    .FirstOrDefault().status == "Returned for Revisions") // Check if the latest log's status is "Returned"
                .Select(e => new InitialReviewViewModel
                {
                    EthicsApplication = e,
                    NonFundedResearchInfo = e.NonFundedResearchInfo,
                    // Fetch only the latest log with status "Returned"
                    EthicsApplicationLog = e.EthicsApplicationLog
                        .OrderByDescending(log => log.changeDate)
                        .Take(1) // Take only the latest log entry
                })
                .ToListAsync();
        }

        public async Task<InitialReview?> GetInitialReviewByUrecNoAsync(string urecNo)
        {
            var application = await _context.InitialReview
                .Include(ir => ir.EthicsApplication)
                .FirstOrDefaultAsync(ir => ir.EthicsApplication.urecNo == urecNo);

            // Return null if the application is not found instead of throwing an exception
            return application; // This will return null if no match is found
        }


        public async Task<IEnumerable<EthicsApplication>> GetApprovedEthicsApplicationsAsync()
        {
            // Fetch the EthicsApplications from InitialReviews with status "Approved"
            return await _context.InitialReview
                .Where(a => a.status == "Approved")
                .Select(a => a.EthicsApplication) // Assuming InitialReview has a navigation property to EthicsApplication
                .ToListAsync();
        }

        public async Task<EvaluationDetailsViewModel> GetApplicationDetailsAsync(string urecNo, int evaluationId)
        {
            // Fetch the evaluation details along with related entities
            var evaluation = await _context.EthicsEvaluation
                .Include(ev => ev.EthicsEvaluator)
                    .ThenInclude(e => e.Faculty)
                .Include(ev => ev.EthicsApplication)
                    .ThenInclude(ea => ea.NonFundedResearchInfo)
                        .ThenInclude(nf => nf.CoProponent) // Include co-proponents
                .Include(ev => ev.EthicsApplication)
                    .ThenInclude(ea => ea.ReceiptInfo)
                .Include(ev => ev.EthicsApplication)
                    .ThenInclude(ea => ea.EthicsApplicationForms)
                .Include(ev => ev.EthicsApplication)
                    .ThenInclude(ea => ea.EthicsApplicationLog)
                .FirstOrDefaultAsync(ev => ev.evaluationId == evaluationId);

            if (evaluation == null)
            {
                throw new Exception("Evaluation not found.");
            }

            // Retrieve the associated application for further details
            var application = await _context.EthicsApplication
                .Include(a => a.InitialReview)
                .Include(a => a.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponent) // Assuming you want to get co-proponents too
                .Include(a => a.ReceiptInfo)
                .Include(a => a.EthicsApplicationForms)
                .Include(a => a.EthicsApplicationLog)
                .Include(a => a.EthicsEvaluation)
                .FirstOrDefaultAsync(a => a.urecNo == urecNo); // Use urecNo to fetch the application

            if (application == null)
            {
                throw new Exception("Application not found.");
            }

            // Retrieve the associated user (applicant)
            var appUser = await _userManager.FindByIdAsync(application.userId);

            // Retrieve the initial review (if necessary)
            var initialReview = await _context.InitialReview
                .FirstOrDefaultAsync(ir => ir.urecNo == urecNo);

            // Create and populate the ViewModel
            var viewModel = new EvaluationDetailsViewModel
            {
                CurrentEvaluation = evaluation,
                NonFundedResearchInfo = application.NonFundedResearchInfo,
                CoProponent = application.NonFundedResearchInfo?.CoProponent ?? new List<CoProponent>(),
                ReceiptInfo = application.ReceiptInfo,

                // Ensure that EthicsEvaluator is checked for null
                EthicsEvaluator = evaluation.EthicsEvaluator ?? new EthicsEvaluator(),
                EthicsApplication = application,
                InitialReview = initialReview,
                EthicsApplicationForms = application.EthicsApplicationForms ?? new List<EthicsApplicationForms>(),
                EthicsApplicationLog = application.EthicsApplicationLog ?? new List<EthicsApplicationLog>(),
            };


            return viewModel;
        }

    }

}
