using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using CRE.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CRE.Services
{
    public class EthicsApplicationServices : IEthicsApplicationServices
    {
        private readonly CreDbContext _context;
        public EthicsApplicationServices(CreDbContext context)
        {
            _context = context;
        }

        public async Task ApplyForEthicsAsync(EthicsApplication application)
        {
            _context.EthicsApplication.Add(application);
            await _context.SaveChangesAsync();
        }

        public async Task CancelApplicationAsync(string urecNo)
        {
            var application = await GetApplicationByUrecNoAsync(urecNo);
            if (application != null)
            {
                _context.EthicsApplication.Remove(application);
                await _context.SaveChangesAsync();
            }
        }

        public async Task EditApplicationAsync(EthicsApplication application)
        {
            _context.EthicsApplication.Update(application);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GenerateUrecNoAsync()
        {
            string baseFormat = "UREC-{0}-{1}"; // "UREC-YYYY-XXXX"
            string year = DateTime.Now.Year.ToString();

            // Fetch existing UREC Nos for the current year
            var existingUrecNos = await _context.EthicsApplication
                .Where(u => u.urecNo.StartsWith($"UREC-{year}-"))
                .Select(u => u.urecNo)
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



        public async Task<bool> IsUrecNoExistsAsync(string urecNo)
        {
            // Check if urecNo already exists for the current day
            return await _context.EthicsApplication
                .AnyAsync(a => a.urecNo == urecNo); // Assuming CreatedDate is a DateTime property in your model
        }

        public async Task<IEnumerable<EthicsApplication>> GetAllApplicationsAsync()
        {
            return await _context.EthicsApplication.ToListAsync();
        }


        public async Task<EthicsApplication> GetApplicationByUrecNoAsync(string urecNo)
        {
            return await _context.EthicsApplication.FirstOrDefaultAsync(a => a.urecNo == urecNo);
        }

        public async Task<IEnumerable<EthicsApplication>> GetApplicationsByUserAsync(string userId)
        {
            return await _context.EthicsApplication
                .Where(a => a.userId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<EthicsApplication>> GetApplicationsSortedByFieldOfStudyAsync()
        {
            return await _context.EthicsApplication
                .OrderBy(a => a.fieldOfStudy)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<EthicsApplication> GetApplicationByDtsNoAsync(string dtsNo)
        {
            return await _context.EthicsApplication
                                 .FirstOrDefaultAsync(e => e.dtsNo == dtsNo);
        }
        public async Task<List<ApplicationViewModel>> GetApplicationsByInitialReviewTypeAsync(string reviewType)
        {
            return await _context.EthicsApplication
                .Include(a => a.InitialReview)
                .Include(a => a.NonFundedResearchInfo)
                .Include(a => a.EthicsApplicationLog)
                .Include(a => a.NonFundedResearchInfo.CoProponent) // Include all CoProponents
                .Where(a => a.InitialReview.ReviewType == reviewType)
                .Select(a => new ApplicationViewModel
                {
                    EthicsApplication = a,
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    CoProponent = a.NonFundedResearchInfo.CoProponent.ToList(), // Get all CoProponents
                    EthicsApplicationLog = a.EthicsApplicationLog
                        .OrderByDescending(log => log.changeDate)
                        .ToList(), // Get all logs ordered by change date
                    InitialReview = a.InitialReview
                }).ToListAsync();
        }



        public async Task<List<ApplicationViewModel>> GetApplicationsBySubmitReviewTypeAsync(string reviewType)
        {
            return await _context.EthicsApplication
                .Include(a => a.InitialReview)
                .Include(a => a.NonFundedResearchInfo)
                .Include(a => a.EthicsApplicationLog)
                .Include(a => a.NonFundedResearchInfo.CoProponent) // Include all CoProponents
                .Where(a => a.InitialReview.ReviewType == reviewType)
                .Select(a => new ApplicationViewModel
                {
                    EthicsApplication = a,
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    CoProponent = a.NonFundedResearchInfo.CoProponent.ToList(), // Get all CoProponents
                    EthicsApplicationLog = a.EthicsApplicationLog
                        .OrderByDescending(log => log.changeDate)
                        .ToList(), // Get all logs ordered by change date
                    InitialReview = a.InitialReview
                }).ToListAsync();
        }

        public async Task<List<ApplicationViewModel>> GetAllApplicationViewModelsAsync()
        {
            return await _context.EthicsApplication
                .Include(a => a.InitialReview)
                .Include(a => a.NonFundedResearchInfo) // Include necessary navigation properties
                .Include(a => a.EthicsApplicationLog)
                .Include(a => a.NonFundedResearchInfo.CoProponent) // Include all CoProponents
                .Select(a => new ApplicationViewModel
                {
                    EthicsApplication = a,
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    CoProponent = a.NonFundedResearchInfo.CoProponent.ToList(), // Get all CoProponents
                    EthicsApplicationLog = a.EthicsApplicationLog
                        .OrderByDescending(log => log.changeDate)
                        .ToList(),
                    InitialReview = a.InitialReview
                }).ToListAsync();
        }
        public async Task UpdateApplicationStatusAsync(int evaluationId, string urecNo, string status)
        {
            // Retrieve the application using the provided urecNo
            var application = await _context.EthicsApplication
                .Include(a => a.EthicsEvaluation) // Include the related evaluations
                .FirstOrDefaultAsync(a => a.urecNo == urecNo);

            if (application != null)
            {
                // Find the specific evaluation to update
                var evaluationToUpdate = application.EthicsEvaluation
                    .FirstOrDefault(e => e.evaluationId == evaluationId); // Adjust the property name as per your model

                if (evaluationToUpdate != null)
                {
                    // Update the status of the specific evaluation
                    evaluationToUpdate.evaluationStatus = status; // Update status here
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new InvalidOperationException("The specified evaluation was not found.");
                }
            }
            else
            {
                throw new InvalidOperationException("The specified application was not found.");
            }
        }

        public async Task<EvaluationDetailsViewModel> GetEvaluationDetailsAsync(string urecNo)
        {
            // Fetch the application and include related data
            var application = await _context.EthicsApplication
                .Include(app => app.EthicsEvaluation)
                    .ThenInclude(e => e.EthicsEvaluator)
                .Include(app => app.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponent)
                .Include(app => app.InitialReview)
                .Include(app => app.ReceiptInfo)
                .Include(app => app.EthicsApplicationForms)
                .Include(app => app.EthicsApplicationLog)
                .Include(app => app.EthicsClearance)
                .FirstOrDefaultAsync(app => app.urecNo == urecNo);

            if (application == null)
            {
                return null; // Handle application not found
            }

            // Create and return the view model
            var viewModel = new EvaluationDetailsViewModel
            {
                NonFundedResearchInfo = application.NonFundedResearchInfo,
                EthicsApplication = application,
                InitialReview = application.InitialReview,
                ReceiptInfo = application.ReceiptInfo,
                EthicsApplicationForms = application.EthicsApplicationForms,
                EthicsApplicationLog = application.EthicsApplicationLog,
                EthicsEvaluation = application.EthicsEvaluation,
                CurrentEvaluation = application.EthicsEvaluation.FirstOrDefault(),
                CoProponent = application.NonFundedResearchInfo.CoProponent,
                HasEthicsClearance = application.EthicsClearance != null,
                EthicsClearance = application.EthicsClearance
            };

            return viewModel;
        }
    }
}
