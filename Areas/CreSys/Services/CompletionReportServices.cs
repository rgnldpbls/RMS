using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using CRE.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CRE.Services
{
    public class CompletionReportServices : ICompletionReportServices
    {
        private readonly CreDbContext _context;
        public CompletionReportServices(CreDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SubmitTerminalReportAsync(string urecNo, IFormFile terminalReportFile, DateOnly researchStartDate)
        {
            // Ensure the file is valid
            if (terminalReportFile != null && terminalReportFile.Length > 0)
            {
                // Convert the uploaded file into a byte array
                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await terminalReportFile.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                // Create a new completion report entry
                var completionReport = new CompletionReport
                {
                    urecNo = urecNo,
                    terminalReport = fileBytes,  // Store the byte array in the TerminalReport field
                    researchStartDate = researchStartDate,
                    submissionDate = DateOnly.FromDateTime(DateTime.Now)
                };

                // Save to the database
                _context.CompletionReport.Add(completionReport);
                await _context.SaveChangesAsync();

                return true; // Return true if the operation was successful
            }

            return false; // Return false if the file is invalid
        }

     
            public async Task<ApplyForCompletionCertificateViewModel> GetApplyForCompletionCertificateViewModelAsync(string urecNo)
            {
                // Retrieve the required entities from the database based on urecNo
                var ethicsApplication = await _context.EthicsApplication
                    .FirstOrDefaultAsync(ea => ea.urecNo == urecNo);

                if (ethicsApplication == null)
                {
                    return null; // Return null or handle the case where the application is not found
                }

                var ethicsClearance = await _context.EthicsClearance
                    .FirstOrDefaultAsync(ec => ec.urecNo == urecNo);

                var nonFundedResearchInfo = await _context.NonFundedResearchInfo
                    .FirstOrDefaultAsync(nfri => nfri.urecNo == urecNo);

                var coProponents = await _context.CoProponent
                    .Where(cp => cp.nonFundedResearchId == nonFundedResearchInfo.nonFundedResearchId)
                    .ToListAsync();

                var ethicsApplicationLog = await _context.EthicsApplicationLog
                    .Where(log => log.urecNo == urecNo)
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

                return viewModel;
            }

        public async Task<CompletionReport?> GetCompletionReportByUrecNoAsync(string urecNo)
        {
            return await _context.CompletionReport
                .Include(cr => cr.EthicsApplication)
                .Include(cr => cr.EthicsApplication)
                    .ThenInclude(ea => ea.NonFundedResearchInfo) // Include NonFundedResearchInfo
                        .ThenInclude(nfri => nfri.CoProponent) // Include CoProponents
                .Include(cr => cr.EthicsApplication)
                    .ThenInclude(ea => ea.EthicsApplicationLog) // Include EthicsApplicationLog
                .Include(cr => cr.EthicsApplication)
                    .ThenInclude(ea => ea.CompletionCertificate) // Include CompletionCertificate
                .FirstOrDefaultAsync(cr => cr.urecNo == urecNo);
        }


        public async Task<IEnumerable<CompletionReportViewModel>> GetCompletionReportsAsync()
        {
            // Query the database to retrieve the relevant data
            var completionReports = await _context.CompletionReport
                .Include(cr => cr.EthicsApplication)
                    .ThenInclude(e => e.CompletionCertificate) // Load related CompletionCertificate data
                .Include(cr => cr.EthicsApplication)
                    .ThenInclude(e => e.EthicsApplicationLog)
                .Include(cr => cr.EthicsApplication)
                .ThenInclude(e => e.NonFundedResearchInfo)
                    .ThenInclude(cr => cr.CoProponent) // Load related CoProponent data
                .ToListAsync();

            // Map the result to the ViewModel
            var viewModel = completionReports.Select(report => new CompletionReportViewModel
            {
                // Assuming each User has only one NonFundedResearchInfo, take the first
                NonFundedResearchInfo = report.EthicsApplication.NonFundedResearchInfo,
                CoProponent = report.EthicsApplication.NonFundedResearchInfo.CoProponent,
                EthicsApplication = report.EthicsApplication,
                CompletionReport = report,
                CompletionCertificate = report.EthicsApplication.CompletionCertificate,
                EthicsApplicationLog = report.EthicsApplication.EthicsApplicationLog
            }).ToList();

            return viewModel;
        }

        public async Task<byte[]> GetTerminalReportAsync(string urecNo)
        {
            var report = await _context.CompletionReport
                .Where(cr => cr.urecNo == urecNo)
                .Select(cr => cr.terminalReport) // Assuming TerminalReportFile is a byte[] field
                .FirstOrDefaultAsync();

            return report;
        }
        public async Task SaveCompletionReportAsync(CompletionReport report)
        {
            _context.CompletionReport.Update(report);
            await _context.SaveChangesAsync();
        }

    }
}
