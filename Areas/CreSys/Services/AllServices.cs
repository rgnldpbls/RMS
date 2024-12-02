using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ResearchManagementSystem.Areas.CreSys.Data;
using ResearchManagementSystem.Areas.CreSys.Interfaces;
using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Areas.CreSys.ViewModels;
using ResearchManagementSystem.Data;
using ResearchManagementSystem.Models;

namespace ResearchManagementSystem.Areas.CreSys.Services
{
    public class AllServices : IAllServices
    {
        private readonly CreDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AllServices(CreDbContext context, ApplicationDbContext appdbcontext, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> SubmitTerminalReportAsync(string urecNo, IFormFile terminalReportFile, DateTime researchStartDate, DateTime researchEndDate)
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
                    UrecNo = urecNo,
                    TerminalReport = fileBytes,  // Store the byte array in the TerminalReport field
                    ResearchStartDate = researchStartDate,
                    ResearchEndDate = researchEndDate,
                    SubmissionDate = DateTime.Now

                };

                // Save to the database
                _context.CRE_CompletionReports.Add(completionReport);
                await _context.SaveChangesAsync();

                return true; // Return true if the operation was successful
            }

            return false; // Return false if the file is invalid
        }
        public async Task<CompletionReport> GetCompletionReportByUrecNoAsync(string urecNo)
        {
            return await _context.CRE_CompletionReports
                .Include(cr => cr.EthicsApplication)
                    .ThenInclude(ea => ea.NonFundedResearchInfo)
                        .ThenInclude(nfri => nfri.CoProponents)
                .Include(cr => cr.EthicsApplication)
                    .ThenInclude(ea => ea.EthicsApplicationLogs) // Include EthicsApplicationLog
                .Include(cr => cr.EthicsApplication)
                    .ThenInclude(ea => ea.CompletionCertificate) // Include CompletionCertificate
                .FirstOrDefaultAsync(cr => cr.UrecNo == urecNo);
        }
        public async Task<byte[]> GetTerminalReportAsync(string urecNo)
        {
            var report = await _context.CRE_CompletionReports
                .Where(cr => cr.UrecNo == urecNo)
                .Select(cr => cr.TerminalReport) // Assuming TerminalReportFile is a byte[] field
                .FirstOrDefaultAsync();

            return report;
        }

        public async Task<IEnumerable<CompletionReportViewModel>> GetCompletionReportsAsync()
        {
            var ethicsApplications = await _context.CRE_EthicsApplication
                .Include(ea => ea.NonFundedResearchInfo)
                    .ThenInclude(nfri => nfri.CoProponents)
                .Include(ea => ea.CompletionReport)
                .Include(ea => ea.CompletionCertificate)
                .Include(ea => ea.EthicsApplicationLogs)
                .Where(ea => ea.NonFundedResearchInfo != null && ea.CompletionReport != null) 
                .ToListAsync();

            // Map the result to the ViewModel
            var viewModel = ethicsApplications.Select(ea => new CompletionReportViewModel
            {
                NonFundedResearchInfo = ea.NonFundedResearchInfo,
                CoProponent = ea.NonFundedResearchInfo?.CoProponents ?? Enumerable.Empty<CoProponent>(), // Safely handle CoProponents
                EthicsApplication = ea,
                CompletionReport = ea.CompletionReport,
                CompletionCertificate = ea.CompletionCertificate,
                EthicsApplicationLog = ea.EthicsApplicationLogs ?? Enumerable.Empty<EthicsApplicationLogs>() // Fallback to empty logs
            }).ToList();

            return viewModel;
        }
        public async Task<EthicsClearance> GetClearanceByUrecNoAsync(string urecNo)
        {
            return await _context.CRE_EthicsClearance
            .FirstOrDefaultAsync(clearance => clearance.UrecNo == urecNo);
        }
        public async Task<List<ResearchReportModel>> GetFilteredResearchData(ReportGenerationViewModel model)
        {
            var query = _context.CRE_EthicsApplication
                          .Include(r => r.NonFundedResearchInfo)
                              .ThenInclude(nf => nf.CoProponents)
                          .Include(r => r.EthicsApplicationLogs)
                          .Include(r => r.EthicsClearance)
                          .Include(r => r.InitialReview)
                          .AsQueryable();

            // Filter by Campus (if applicable)
            if (!string.IsNullOrEmpty(model.SelectedCampus))
            {
                if (model.SelectedCampus == "Whole University")
                {
                    query = query.Where(r => r.NonFundedResearchInfo.University.Contains("Polytechnic University of the Philippines"));
                }
                else
                {
                    query = query.Where(r => r.NonFundedResearchInfo.Campus == model.SelectedCampus);
                }
            }
            // If no campus is selected, filter by College (if applicable)
            else if (!string.IsNullOrEmpty(model.SelectedCollege) && model.SelectedCollege != "All Colleges")
            {
                query = query.Where(r => r.NonFundedResearchInfo.College == model.SelectedCollege);
            }

            // Filter by Field of Study (if applicable)
            if (!string.IsNullOrEmpty(model.SelectedFieldOfStudy) && model.SelectedFieldOfStudy != "All Field of Study")
            {
                query = query.Where(r => r.FieldOfStudy == model.SelectedFieldOfStudy);
            }

            // Filter by Date Range (if applicable)
            if (model.StartDate.HasValue)
            {
                query = query.Where(r => r.SubmissionDate >= model.StartDate.Value && r.EthicsClearance != null);
            }

            if (model.EndDate.HasValue)
            {
                query = query.Where(r => r.SubmissionDate <= model.EndDate.Value && r.EthicsClearance != null);
            }
            // Validate the date range: ensure StartDate is not after EndDate
            if (model.StartDate.HasValue && model.EndDate.HasValue && model.StartDate > model.EndDate)
            {
                throw new ArgumentException("Start date cannot be after end date.");
            }

            if (!string.IsNullOrEmpty(model.ReaserchType) && model.ReaserchType != "All Researcher")
            {
                if (model.ReaserchType == "External Researcher")
                {
                    // Filter out research from Polytechnic University of the Philippines
                    query = query.Where(r => r.NonFundedResearchInfo.University != "Polytechnic University of the Philippines");
                }
                else if (model.ReaserchType == "Internal Researcher")
                {
                 
                    if (!string.IsNullOrEmpty(model.InternalResearcherType) && model.InternalResearcherType != "All Internal Researcher")
                    {
                        var userIds = new List<string>();

                        if (model.InternalResearcherType == "Student")
                        {
                            var studentUsers = await _userManager.GetUsersInRoleAsync("Student");
                            userIds = studentUsers.Select(u => u.Id).ToList();
                        }
                        else if (model.InternalResearcherType == "Faculty")
                        {
                            var facultyUsers = await _userManager.GetUsersInRoleAsync("Faculty");
                            userIds = facultyUsers.Select(u => u.Id).ToList();
                        }


                        // Filter research applications by UserId
                        query = query.Where(r => r.UserId != null && userIds.Contains(r.UserId));

                    }
                }
            }
            else
            {
                // If "All Researcher" is selected, no filtering by University or UserId
                // No changes to the query for this case
            }

            // Map the filtered data to the report model
            var researchData = query.Select(r => new ResearchReportModel
            {
                UrecNo = r.UrecNo,
                TitleOfResearch = r.NonFundedResearchInfo.Title,
                ProponentsAuthors = string.IsNullOrEmpty(r.NonFundedResearchInfo.Name)
            ? string.Join(", ", r.NonFundedResearchInfo.CoProponents.Select(cp => cp.CoProponentName))
            : r.NonFundedResearchInfo.Name + (r.NonFundedResearchInfo.CoProponents.Any()
                ? ", " + string.Join(", ", r.NonFundedResearchInfo.CoProponents.Select(cp => cp.CoProponentName))
                : ""),
                DateReceived = r.EthicsApplicationLogs
                    .Where(log => log.Status == "Pending for Evaluation")
                    .OrderBy(log => log.ChangeDate)
                    .Select(log => log.ChangeDate)
                    .FirstOrDefault(),
                DateReviewedForCompleteness = r.InitialReview != null && r.InitialReview.Status == "Approved"
                    ? r.InitialReview.DateReviewed
                    : null,
                DateReceivedFromEvaluation = r.EthicsApplicationLogs
                .Where(log => log.Status == "Evaluated" || log.Status == "Application Evaluated")
                .Select(log => log.ChangeDate)
                .FirstOrDefault(),
                            DateNoticeToProponents = r.EthicsApplicationLogs
                .Where(log => log.Status == "Evaluated" || log.Status == "Application Evaluated")
                .Select(log => log.ChangeDate)
                .FirstOrDefault(),
                DateApprovedByUREB = r.EthicsApplicationLogs
            .Where(log => log.Status == "Clearance Issued")
            .Select(log => log.ChangeDate).FirstOrDefault(),
                DateIssuedCertificate = r.EthicsApplicationLogs
            .Where(ec => ec.Status == "Clearance Issued")
            .Select(ec => ec.ChangeDate).FirstOrDefault()
            }).ToList();

            return researchData;
        }

        public byte[] GenerateExcelFile(List<ResearchReportModel> researchData, DateTime? startDate, DateTime? endDate, out string fileName)
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                // Create a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Research Report");

                // Title at the top
                string title = $"EAM-({startDate?.ToString("MMM dd")}-{endDate?.ToString("MMM dd")})";
                worksheet.Cells[1, 1].Value = title;

                // Merge cells for the title
                worksheet.Cells[1, 1, 1, 10].Merge = true;

                // Title Row Formatting
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Size = 14;
                worksheet.Cells[1, 1].Style.Font.Name = "Times New Roman";
                worksheet.Cells[1, 1].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[1, 1].Style.WrapText = true;

                // Set Headers
                worksheet.Cells[2, 1].Value = "UREC Code";
                worksheet.Cells[2, 2].Value = "Title of Research";
                worksheet.Cells[2, 3].Value = "Proponents/Authors";
                worksheet.Cells[2, 4].Value = "Date Received";
                worksheet.Cells[2, 5].Value = "Date Reviewed for Complete Documentary Requirements";
                worksheet.Cells[2, 6].Value = "Date Received from the UREB/CBREB Evaluation Results";
                worksheet.Cells[2, 7].Value = "Date Notice to the Proponents on the Results of the UREB Evaluation";
                worksheet.Cells[2, 8].Value = "Date Approved by the UREB/CBREB";
                worksheet.Cells[2, 9].Value = "Date Issued Certificate";
                worksheet.Cells[2, 10].Value = "Remarks";

                // Header Row Formatting
                using (var headerRange = worksheet.Cells[2, 1, 2, 10])
                {
                    headerRange.Style.Font.Name = "Times New Roman";
                    headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    headerRange.Style.WrapText = true; // Wrap header text
                }

                // Populate the worksheet with the data
                var sortedData = researchData.OrderBy(r => r.UrecNo).ToList();

                int row = 3;  // Start from row 3 (after the title and header)
                foreach (var data in sortedData)
                {
                    worksheet.Cells[row, 1].Value = CleanText(data.UrecNo);
                    worksheet.Cells[row, 2].Value = CleanText(data.TitleOfResearch);
                    worksheet.Cells[row, 3].Value = CleanText(data.ProponentsAuthors);
                    worksheet.Cells[row, 4].Value = data.DateReceived?.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 5].Value = data.DateReviewedForCompleteness?.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 6].Value = data.DateReceivedFromEvaluation?.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 7].Value = data.DateNoticeToProponents?.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 8].Value = data.DateApprovedByUREB?.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 9].Value = data.DateIssuedCertificate?.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 10].Value = ""; // Remarks column left blank

                    // Data Row Formatting
                    using (var rowRange = worksheet.Cells[row, 1, row, 10])
                    {
                        rowRange.Style.Font.Name = "Arial";
                        rowRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        rowRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        rowRange.Style.WrapText = true; // Wrap text while keeping whole words intact
                    }

                    row++;
                }

                // Adjust column widths based on content
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Add borders to all cells
                using (var allCells = worksheet.Cells[2, 1, row - 1, 10])
                {
                    allCells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    allCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    allCells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    allCells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }

                // Construct dynamic file name
                fileName = title;
                return package.GetAsByteArray();
            }
        }

        // Helper method to clean text
        private string CleanText(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            // Remove extra spaces and replace multiple spaces with a single space
            return System.Text.RegularExpressions.Regex.Replace(input.Trim(), @"\s{2,}", " ");
        }
        public async Task<List<EthicsApplication>> GetApplicationsByFieldOfStudyAsync(string userId)
        {
            // Get the chairperson's faculty using the user ID
            var chairperson = await _context.CRE_Chairperson
                .FirstOrDefaultAsync(c => c.UserId == userId); // Correct the matching condition

            if (chairperson == null)
                return new List<EthicsApplication>(); // Return an empty list if chairperson not found

            // Retrieve applications matching the chairperson's field of study and specific review types
            var applications = await _context.CRE_EthicsApplication
                .Include(e => e.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponents)
                .Include(e => e.InitialReview)
                .Include(e => e.EthicsEvaluation) // Include the EthicsEvaluation records
                .Where(e => e.FieldOfStudy == chairperson.FieldOfStudy // Ensure correct casing for FieldOfStudy
                            && (e.InitialReview.ReviewType == "Full Review"
                            || e.InitialReview.ReviewType == "Expedited"))
                .ToListAsync();

            // After retrieving the applications, filter out the evaluations where the evaluator's UserId matches
            foreach (var application in applications)
            {
                // Loop through each evaluation in the application
                foreach (var evaluation in application.EthicsEvaluation)
                {
                    if (evaluation != null)
                    {
                        // Retrieve the EthicsEvaluator by matching UserId and the EvaluationId
                        var evaluator = await _context.CRE_EthicsEvaluator
                            .FirstOrDefaultAsync(ev => ev.UserID == userId && ev.UserID == evaluation.UserId);

                        if (evaluator != null)
                        {
                            evaluation.EthicsEvaluator = evaluator; // Assign the evaluator explicitly
                        }
                    }
                }
            }

            return applications;
        }
        public async Task<IEnumerable<EthicsApplication>> GetUnderEvaluationApplicationsAsync(IEnumerable<EthicsApplication> ethicsApplications)
        {
            return ethicsApplications.Where(a =>
                a.InitialReview != null &&
                (
                    // Expedited review type with at least one "Evaluated" status but exclude if exactly 2 are "Evaluated"
                    (a.InitialReview.ReviewType == "Expedited" &&
                     a.EthicsEvaluation.Count(e => e.EvaluationStatus == "Evaluated") < 2) ||

                    // Existing condition for Expedited review with specific counts and recommendations pending
                    (a.InitialReview.ReviewType == "Expedited" &&
                     (a.EthicsEvaluation.Count == 2 || a.EthicsEvaluation.Count == 3) &&
                     a.EthicsEvaluation.All(e => e.ProtocolRecommendation == "Pending" && e.ConsentRecommendation == "Pending")) ||

                    // Full Review condition
                    (a.InitialReview.ReviewType == "Full Review" &&
                     (
                         // Include if fewer than 3 evaluations exist
                         a.EthicsEvaluation.Count < 3 ||

                         // Include if not all 3 evaluations are "Evaluated"
                         !a.EthicsEvaluation.All(e => e.EvaluationStatus == "Evaluated")
                     )) ||

                    // Condition to include evaluations with null EndDate or status "Assigned"
                    a.EthicsEvaluation.Any(e => e.EndDate == null || e.EvaluationStatus == "Assigned")
                )
            );
        }


        public async Task<Dictionary<string, List<EthicsEvaluator>>> GetEvaluatorNamesAsync(IEnumerable<EthicsApplication> ethicsApplications)
        {
            var evaluatorNames = new Dictionary<string, List<EthicsEvaluator>>();

            foreach (var application in ethicsApplications)
            {
                // Retrieve all evaluations for the current application with the same UrecNo
                var evaluations = await _context.CRE_EthicsEvaluation
                    .Where(e => e.UrecNo == application.UrecNo && e.EvaluationStatus != "Declined")  // Exclude declined evaluations
                    .ToListAsync();

                // List to store evaluators for the current application
                var evaluators = new List<EthicsEvaluator>();

                foreach (var evaluation in evaluations)
                {
                    // Retrieve evaluator for each evaluation by UserId
                    var evaluator = await _context.CRE_EthicsEvaluator
                        .FirstOrDefaultAsync(e => e.UserID == evaluation.UserId);  // Match evaluator by UserId

                    if (evaluator != null)
                    {
                        evaluators.Add(evaluator);  // Add evaluator to the list
                    }
                }

                // Store the evaluators in the dictionary, ensuring each UrecNo has its own list of evaluators
                evaluatorNames[application.UrecNo] = evaluators;
            }

            return evaluatorNames;
        }
        public async Task<IEnumerable<EthicsApplication>> GetUnassignedApplicationsAsync(IEnumerable<EthicsApplication> ethicsApplications)
        {
            return ethicsApplications.Where(a =>
                a.InitialReview != null &&
                (a.InitialReview.ReviewType == "Expedited" || a.InitialReview.ReviewType == "Full Review") &&
                (
                    // New condition: If evaluation count is less than required for each type, consider it unassigned
                    (a.InitialReview.ReviewType == "Expedited" && a.EthicsEvaluation.Count < 2) ||
                    (a.InitialReview.ReviewType == "Full Review" && a.EthicsEvaluation.Count < 3) ||

                   // If only one evaluation exists and its status is "Evaluated," consider the application as unassigned
                   (a.EthicsEvaluation.Count(e => e.EvaluationStatus == "Evaluated") < 1) ||
                    (
                        // If review type is "Expedited" or "Full Review" but no "Evaluated" entries, apply the other filters
                        (
                            !a.EthicsEvaluation.Any(e => e.EvaluationStatus == "Evaluated") &&
                            (
                                // Expedited review conditions
                                (a.InitialReview.ReviewType == "Expedited" &&
                                 a.EthicsEvaluation.Count(e => e.EvaluationStatus == "Pending" || e.EvaluationStatus == "Assigned") <= 3 &&
                                 !a.EthicsEvaluation.All(e => e.EndDate != null)) ||

                                // Full review conditions
                                (a.InitialReview.ReviewType == "Full Review" &&
                                 a.EthicsEvaluation.Count(e => e.EvaluationStatus == "Pending" || e.EvaluationStatus == "Assigned") < 3 &&
                                 !a.EthicsEvaluation.All(e => e.EndDate != null))
                            )
                        )
                    )
                )
            );
        }
        public async Task<IEnumerable<EthicsApplication>> GetEvaluationResultApplicationsAsync(IEnumerable<EthicsApplication> ethicsApplications)
        {
            return ethicsApplications.Where(a =>
                a.InitialReview != null &&
                ((a.InitialReview.ReviewType == "Expedited" &&
                  a.EthicsEvaluation.Count >= 2 && a.EthicsEvaluation.Count <= 3 &&
                  a.EthicsEvaluation.All(e => e.EndDate != null)) || // For Expedited: 2 to 3 evaluations with end dates
                 (a.InitialReview.ReviewType == "Full Review" &&
                  a.EthicsEvaluation.Count == 3 &&
                  a.EthicsEvaluation.All(e => e.EndDate != null)))); // For Full Review: exactly 3 evaluations with end dates
        }
        public async Task<List<NonFundedResearchInfo>> GetNonFundedResearchInfosAsync(List<string> urecNos)
        {
            // Fetch NonFundedResearchInfo entries based on the provided urecNo list
            var nonFundedResearchInfos = await _context.CRE_NonFundedResearchInfo
                .Where(info => urecNos.Contains(info.UrecNo)) // Assuming UrecNo is the property in NonFundedResearchInfo
                .ToListAsync();

            return nonFundedResearchInfos;
        }
        public async Task<List<ChiefEvaluationViewModel>> GetExemptApplicationsAsync()
        {
            return await _context.CRE_EthicsApplication
                .Include(a => a.NonFundedResearchInfo)
                    .ThenInclude(n => n.CoProponents)
                .Where(a => a.InitialReview.ReviewType == "Exempt" && !a.EthicsEvaluation.Any())
                .Select(a => new ChiefEvaluationViewModel
                {
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    EthicsApplication = a,
                    InitialReview = a.InitialReview,
                    ReceiptInfo = a.ReceiptInfo,
                    EthicsApplicationForms = a.EthicsApplicationForms,
                    EthicsApplicationLog = a.EthicsApplicationLogs
                }).ToListAsync();
        }
        public async Task<List<EvaluatedExemptApplication>> GetEvaluatedExemptApplicationsAsync()
        {
            var applications = await _context.CRE_EthicsApplication
                .Where(a => a.InitialReview.ReviewType == "Exempt" && a.EthicsEvaluation.Any())
                .Select(a => new EvaluatedExemptApplication
                {
                    EthicsApplication = a,
                    NonFundedResearchInfo = a.NonFundedResearchInfo ?? new NonFundedResearchInfo(),  // Ensure it's not null
                    EthicsEvaluation = a.EthicsEvaluation.FirstOrDefault() ?? new EthicsEvaluation(),  // Handle null for FirstOrDefault
                    InitialReview = a.InitialReview,
                    EthicsApplicationLog = a.EthicsApplicationLogs ?? new List<EthicsApplicationLogs>(),  // Ensure collection is not null
                })
                .ToListAsync();

            return applications;
        }
        public async Task<List<EvaluatedExpeditedApplication>> GetEvaluatedExpeditedApplicationsAsync()
        {
            return await _context.CRE_EthicsApplication
                .Where(a => a.InitialReview.ReviewType == "Expedited" && a.EthicsEvaluation.Any())
                .Select(a => new EvaluatedExpeditedApplication
                {
                    EthicsApplication = a,
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    EthicsEvaluation = a.EthicsEvaluation.ToList(),
                    InitialReview = a.InitialReview,
                    EthicsEvaluator = a.EthicsEvaluation
                        .Select(e => e.UserId) // Now we're accessing the UserId directly, or we can query EthicsEvaluator for additional details.
                        .Distinct()
                        .ToList(),
                    EthicsApplicationLog = a.EthicsApplicationLogs
                })
                .ToListAsync();
        }
        public async Task<List<EvaluatedFullReviewApplication>> GetEvaluatedFullReviewApplicationsAsync()
        {
            return await _context.CRE_EthicsApplication
              .Where(a => a.InitialReview.ReviewType == "Full Review" && a.EthicsEvaluation.Any())
              .Select(a => new EvaluatedFullReviewApplication
              {
                  EthicsApplication = a,
                  NonFundedResearchInfo = a.NonFundedResearchInfo,
                  EthicsEvaluation = a.EthicsEvaluation.ToList(),
                  InitialReview = a.InitialReview,
                  EthicsEvaluator = a.EthicsEvaluation
                      .Select(e => e.UserId) // Now we're accessing the UserId directly, or we can query EthicsEvaluator for additional details.
                      .Distinct()
                      .ToList(),
                  EthicsApplicationLog = a.EthicsApplicationLogs
              })
              .ToListAsync();
        }
        public async Task<List<PendingIssuanceViewModel>> GetPendingApplicationsForIssuanceAsync()
        {
            var pendingIssuanceList = new List<PendingIssuanceViewModel>();

            // Step 1: Retrieve the applications first
            var pendingApplications = await _context.CRE_EthicsApplication
                .Where(a => a.EthicsApplicationLogs != null && a.EthicsApplicationLogs.Any()  // Ensure logs exist
                            && a.EthicsEvaluation != null && a.EthicsEvaluation.Any()) // Ensure evaluations exist
                .ToListAsync();

            // Step 2: Process each application one by one
            foreach (var application in pendingApplications)
            {
                try
                {
                    // Step 3: Load related data (NonFundedResearchInfo, Forms, Logs, etc.)
                    var nonFundedResearchInfo = await _context.CRE_NonFundedResearchInfo
                        .FirstOrDefaultAsync(n => n.UrecNo == application.UrecNo);
                    var applicationForms = await _context.CRE_EthicsApplicationForms
                        .Where(f => f.UrecNo == application.UrecNo).ToListAsync();
                    var applicationLogs = await _context.CRE_EthicsApplicationLogs
                        .Where(log => log.UrecNo == application.UrecNo).ToListAsync();
                    var ethicsEvaluations = await _context.CRE_EthicsEvaluation
                        .Where(e => e.UrecNo == application.UrecNo).ToListAsync();
                    var ethicsClearance = await _context.CRE_EthicsClearance
                        .FirstOrDefaultAsync(c => c.UrecNo == application.UrecNo);

                    // Step 4: Check for evaluations completion
                    var allEvaluationsCompleted = ethicsEvaluations?.All(e => e.EvaluationStatus == "Evaluated") ?? false;

                    // Step 5: Skip applications where evaluations are not completed
                    if (!allEvaluationsCompleted)
                    {
                        continue;
                    }

                    // Step 6: Check if Form15 is uploaded
                    bool hasForm15Uploaded = applicationForms.Any(f => f.EthicsFormId == "FORM15");

                    // Step 7: Check if there are minor or major revisions in the application logs
                    bool hasMinorOrMajorRevisions = applicationLogs
                        .Any(log => log.Status == "Minor Revisions" || log.Status == "Major Revisions");

                    // Step 8: Create the PendingIssuanceViewModel for this application
                    var pendingIssuance = new PendingIssuanceViewModel
                    {
                        EthicsApplication = application,
                        EthicsApplicationLog = applicationLogs ?? new List<EthicsApplicationLogs>(), // Ensure it's not null
                        NonFundedResearchInfo = nonFundedResearchInfo ?? new NonFundedResearchInfo(), // Ensure it's not null
                        EthicsEvaluation = ethicsEvaluations ?? new List<EthicsEvaluation>(), // Ensure it's not null
                        EthicsClearance = ethicsClearance, // Can be null, handled properly
                        HasClearanceIssued = ethicsClearance != null,
                        AllEvaluationsCompleted = allEvaluationsCompleted,
                        HasForm15Uploaded = hasForm15Uploaded,
                        HasMinorOrMajorRevisions = hasMinorOrMajorRevisions
                    };

                    // Step 9: Add the processed application to the list
                    pendingIssuanceList.Add(pendingIssuance);
                }
                catch (Exception ex)
                {
                    // Log the error with details about which application caused it
                    // You can use a logging library (like Serilog, NLog, etc.) or simple logging for debugging purposes
                    Console.WriteLine($"Error processing application ID {application.UrecNo}: {ex.Message}");
                }
            }

            return pendingIssuanceList;
        }
        public async Task<AssignEvaluatorsViewModel> GetApplicationDetailsForEvaluationAsync(string urecNo)
        {
            var application = await _context.CRE_EthicsApplication
                 .Include(e => e.InitialReview)
                 .Include(e => e.NonFundedResearchInfo)
                     .ThenInclude(nf => nf.CoProponents)
                 .Include(e => e.EthicsApplicationLogs)
                 .Include(e => e.ReceiptInfo)
                 .Include(e => e.EthicsApplicationForms)
                 .FirstOrDefaultAsync(e => e.UrecNo == urecNo);

            if (application == null)
            {
                throw new Exception("Application not found.");
            }

           

            var viewModel = new AssignEvaluatorsViewModel
            {
                EthicsApplication = application,
                NonFundedResearchInfo = application.NonFundedResearchInfo,
                CoProponent = application.NonFundedResearchInfo?.CoProponents,
                EthicsApplicationForms = application.EthicsApplicationForms,
                ReceiptInfo = application.ReceiptInfo
            };

            return viewModel;
        }
        public async Task<List<EthicsEvaluator>> GetPendingEvaluatorsAsync(string urecNo)
        {
            return await _context.CRE_EthicsEvaluator
                .Where(e => e.EthicsEvaluation.Any(a =>
                    a.EthicsApplication.UrecNo == urecNo && a.EvaluationStatus == "Assigned"))
                .ToListAsync();
        }
        public async Task<List<EthicsEvaluator>> GetAcceptedEvaluatorsAsync(string urecNo)
        {
            return await _context.CRE_EthicsEvaluator
                .Where(e => e.EthicsEvaluation.Any(a =>
                    a.EthicsApplication.UrecNo == urecNo && a.EvaluationStatus == "Accepted"))
                .ToListAsync();
        }
        public async Task<List<EthicsEvaluator>> GetDeclinedEvaluatorsAsync(string urecNo)
        {
            return await _context.CRE_EthicsEvaluator
           .Where(e => e.EthicsEvaluation.Any(a =>
               a.EthicsApplication.UrecNo == urecNo && a.EvaluationStatus == "Declined"))
           .ToListAsync();
        }
        public async Task<List<EthicsEvaluator>> GetEvaluatedEvaluatorsAsync(string urecNo)
        {
            return await _context.CRE_EthicsEvaluation
                .Where(e => e.UrecNo == urecNo && e.EvaluationStatus == "Evaluated")
                .Select(e => e.EthicsEvaluator)
                .Distinct()
                .ToListAsync();
        }
        public async Task<List<EthicsEvaluator>> GetAllEvaluatorsAsync()
        {
            return await _context.CRE_EthicsEvaluator
                .Include(e => e.EthicsEvaluatorExpertises)
                    .ThenInclude(exp => exp.Expertise)
                .OrderBy(e => e.Pending) // Sort by the number of pending evaluations (ascending)
                .ToListAsync();
        }
        public async Task<IEnumerable<EthicsEvaluator>> GetRecommendedEvaluatorsAsync(
             IEnumerable<EthicsEvaluator> allEvaluators,
             string requiredFieldOfStudy,
             string applicantUserId,
             UserManager<ApplicationUser> userManager)
        {

            // Fetch the applicant's User record using UserManager
            var applicantUser = await userManager.FindByIdAsync(applicantUserId);
            if (applicantUser == null)
            {
                throw new ArgumentException("Invalid applicant UserId");
            }

                    return allEvaluators
               .Where(e => e.EthicsEvaluatorExpertises
                   .Any(exp => exp.Expertise != null && exp.Expertise.ExpertiseName == requiredFieldOfStudy)) // Filter by required expertise
               .Where(e => e.UserID != applicantUserId) // Exclude applicant by UserId
               .OrderBy(e => e.Pending) // Sort by least pending evaluations
               .Take(3); // Take top 3 recommended evaluators
        }

        public async Task AssignEvaluatorAsync(string urecNo, int evaluatorId, string fullName)
        {
            var ethicsEvaluation = new EthicsEvaluation
            {
                UrecNo = urecNo,
                EthicsEvaluatorId = evaluatorId,
                Name = fullName,  // Set the evaluator's full name here
                ProtocolRecommendation = "Pending",
                ProtocolRemarks = string.Empty,

                ConsentRecommendation = "Pending",
                ConsentRemarks = string.Empty,

                EvaluationStatus = "Assigned"
            };

            await _context.CRE_EthicsEvaluation.AddAsync(ethicsEvaluation);
            await _context.SaveChangesAsync();
        }

        public async Task<EvaluationDetailsViewModel> GetEvaluationDetailsWithUrecNoAsync(string urecNo, int evaluationId)
        {
            // Fetch the application based on urecNo and evaluationId
            var application = await _context.CRE_EthicsApplication
                .Include(a => a.NonFundedResearchInfo)
                    .ThenInclude(a => a.CoProponents)
                .Include(a => a.EthicsApplicationLogs) // Include EthicsApplicationLog
                .Include(a => a.EthicsApplicationForms)
                .Include(a => a.InitialReview)
                .Include(a => a.EthicsEvaluation)
                .FirstOrDefaultAsync(a => a.UrecNo == urecNo);

            if (application == null)
            {
                throw new Exception("Application not found");
            }

            var evaluation = await _context.CRE_EthicsEvaluation
                .FirstOrDefaultAsync(e => e.EvaluationId == evaluationId);

            if (evaluation == null)
            {
                throw new Exception("Evaluation not found");
            }

            // Create and populate the ViewModel
            var model = new EvaluationDetailsViewModel
            {
                EthicsApplication = application,
                NonFundedResearchInfo = application.NonFundedResearchInfo,
                CoProponent = application.NonFundedResearchInfo.CoProponents.ToList(),
                EthicsApplicationForms = application.EthicsApplicationForms,
                InitialReview = application.InitialReview,
                EthicsEvaluation = new List<EthicsEvaluation> { evaluation },
                ReceiptInfo = application.ReceiptInfo // Assuming you have a ReceiptInfo property
            };

            return model;
        }
        public async Task<IEnumerable<AssignedEvaluationViewModel>> GetAssignedEvaluationsAsync(int evaluatorId)
        {
            return await _context.CRE_EthicsEvaluation
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.InitialReview)
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.NonFundedResearchInfo)
                .Include(e => e.EthicsEvaluator)
                .Where(e => e.EthicsEvaluatorId == evaluatorId
                            && e.EvaluationStatus == "Assigned"
                            && e.EvaluationStatus != "Accepted") // Exclude "Accepted" evaluations
                .Select(e => new AssignedEvaluationViewModel
                {
                    EthicsApplication = e.EthicsApplication,
                    EthicsEvaluation = e,
                    EthicsEvaluator = e.EthicsEvaluator, // This should be a single instance
                    NonFundedResearchInfo = e.EthicsApplication.NonFundedResearchInfo,
                    InitialReview = e.EthicsApplication.InitialReview
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<AssignedEvaluationViewModel>> GetAcceptedEvaluationsAsync(int evaluatorId)
        {
            return await _context.CRE_EthicsEvaluation
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.InitialReview)
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.NonFundedResearchInfo)
                .Include(e => e.EthicsEvaluator)
                .Where(e => e.EthicsEvaluatorId == evaluatorId && e.EvaluationStatus == "Accepted")
                .Select(e => new AssignedEvaluationViewModel
                {
                    EthicsApplication = e.EthicsApplication,
                    EthicsEvaluation = e,
                    EthicsEvaluator = e.EthicsEvaluator,
                    NonFundedResearchInfo = e.EthicsApplication.NonFundedResearchInfo,
                    InitialReview = e.EthicsApplication.InitialReview
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<AssignedEvaluationViewModel>> GetCompletedEvaluationsAsync(int evaluatorId)
        {
            return await _context.CRE_EthicsEvaluation
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.InitialReview)
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.NonFundedResearchInfo)
                .Include(e => e.EthicsEvaluator)
                .Where(e => e.EthicsEvaluatorId == evaluatorId && e.EvaluationStatus == "Evaluated")
                .Select(e => new AssignedEvaluationViewModel
                {
                    EthicsApplication = e.EthicsApplication,
                    EthicsEvaluation = e,
                    EthicsEvaluator = e.EthicsEvaluator,
                    NonFundedResearchInfo = e.EthicsApplication.NonFundedResearchInfo,
                    InitialReview = e.EthicsApplication.InitialReview
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<AssignedEvaluationViewModel>> GetDeclinedEvaluationsAsync(int evaluatorId)
        {
            return await _context.CRE_DeclinedEthicsEvaluation
                    .Include(e => e.EthicsEvaluator) // Include evaluator in EthicsEvaluation
                .Include(de => de.EthicsApplication)
                    .ThenInclude(a => a.InitialReview)
                .Include(de => de.EthicsApplication)
                    .ThenInclude(a => a.NonFundedResearchInfo)
                .Include(de => de.EthicsApplication)
                    .ThenInclude(a => a.ReceiptInfo)
                .Include(de => de.EthicsApplication)
                    .ThenInclude(a => a.EthicsApplicationLogs)
                .Where(de => de.EthicsEvaluator.EthicsEvaluatorId == evaluatorId)
                .Select(de => new AssignedEvaluationViewModel
                {
                    EthicsApplication = de.EthicsApplication,
                    EthicsEvaluator = de.EthicsEvaluator, // Assign evaluator information
                    NonFundedResearchInfo = de.EthicsApplication.NonFundedResearchInfo,
                    InitialReview = de.EthicsApplication.InitialReview,
                    EthicsApplicationLogs = de.EthicsApplication.EthicsApplicationLogs,
                    ReceiptInfo = de.EthicsApplication.ReceiptInfo,
                    DeclinedEthicsEvaluation = de // Set the declined evaluation details
                })
                .ToListAsync();
        }
        public async Task<EthicsEvaluation> GetEvaluationByUrecNoAndEvaluatorIdAsync(string urecNo, int ethicsEvaluatorId)
        {
            return await _context.CRE_EthicsEvaluation
                .FirstOrDefaultAsync(e => e.UrecNo == urecNo && e.EthicsEvaluatorId == ethicsEvaluatorId);
        }
        public async Task UpdateApplicationStatusAsync(int evaluationId, string urecNo, string status)
        {
            // Retrieve the application using the provided urecNo
            var application = await _context.CRE_EthicsApplication
                .Include(a => a.EthicsEvaluation) // Include the related evaluations
                .FirstOrDefaultAsync(a => a.UrecNo == urecNo);

            if (application == null)
            {
                throw new InvalidOperationException("The specified application was not found.");
            }

            // Find the specific evaluation to update
            var evaluationToUpdate = application.EthicsEvaluation
                .FirstOrDefault(e => e.EvaluationId == evaluationId); // Adjust the property name as per your model

            if (evaluationToUpdate == null)
            {
                throw new InvalidOperationException("The specified evaluation was not found.");
            }

            // Update the status of the specific evaluation
            evaluationToUpdate.EvaluationStatus = status;

            // Save changes only once at the end
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEvaluationStatusAsync(int evaluationId, string status, string? reasonForDecline, string userId)
        {
            var evaluation = await _context.CRE_EthicsEvaluation
                .Include(e => e.EthicsApplication)
                .Include(e => e.EthicsEvaluator) // Ensure this includes the evaluator
                .FirstOrDefaultAsync(e => e.EvaluationId == evaluationId);

            if (evaluation != null)
            {
                bool hasChanges = false; // Flag to track if any changes occurred

                if (status == "Declined")
                {
                    // Ensure the evaluator is not null
                    var evaluator = evaluation.EthicsEvaluator;

                    if (evaluator == null)
                    {
                        // Handle the case where there is no evaluator assigned
                        throw new InvalidOperationException("The evaluator for this evaluation was not found.");
                    }

                    // Create the declined evaluation record
                    var declinedEvaluation = new DeclinedEthicsEvaluation
                    {
                        EvaluationId = evaluationId,
                        ReasonForDeclining = reasonForDecline,
                        UrecNo = evaluation.EthicsApplication?.UrecNo,
                        UserId = userId,
                        Name = evaluation.Name,
                        DeclineDate = DateOnly.FromDateTime(DateTime.UtcNow),

                        // Assign the EthicsEvaluatorId properly
                        EthicsEvaluatorId = evaluator.EthicsEvaluatorId // Use the evaluator's ID
                    };

                    // Add the declined evaluation
                    _context.CRE_DeclinedEthicsEvaluation.Add(declinedEvaluation);

                    // Now remove the original evaluation (this won't violate the FK because the declined record already holds the references)
                    _context.CRE_EthicsEvaluation.Remove(evaluation);

                    hasChanges = true;
                }
                else
                {
                    // If the status is not "Declined", just update the evaluation status
                    evaluation.EvaluationStatus = status;
                    evaluation.StartDate = DateTime.UtcNow;

                    _context.CRE_EthicsEvaluation.Update(evaluation);

                    hasChanges = true;
                }

                // Save all changes in one go if any changes were made
                if (hasChanges)
                {
                    await _context.SaveChangesAsync();
                }
            }
        }


        public async Task<InitialReviewViewModel> GetApplicationDetailsAsync(string urecNo)
        {
            var application = await _context.CRE_EthicsApplication
                .Include(e => e.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponents)
                .Include(e => e.EthicsEvaluation)
                    .ThenInclude(e => e.EthicsEvaluator)
                        .ThenInclude(ee => ee.DeclinedEthicsEvaluation)
                .Include(e => e.ReceiptInfo)
                .Include(e => e.EthicsApplicationLogs)
                .Include(e => e.EthicsApplicationForms)
                .Include(e => e.InitialReview) // Include InitialReview directly
                .FirstOrDefaultAsync(e => e.UrecNo == urecNo);

            if (application == null)
            {
                throw new Exception("Application not found.");
            }

            var ethicsEvaluations = application.EthicsEvaluation.ToList(); // Convert to a list if needed

            var viewModel = new InitialReviewViewModel
            {
                EthicsApplication = application,
                NonFundedResearchInfo = application.NonFundedResearchInfo,
                CoProponent = application.NonFundedResearchInfo?.CoProponents ?? new List<CoProponent>(),
                ReceiptInfo = application.ReceiptInfo,
                EthicsApplicationLog = application.EthicsApplicationLogs.OrderByDescending(log => log.ChangeDate),
                EthicsApplicationForms = application.EthicsApplicationForms ?? new List<EthicsApplicationForms>(),
                InitialReview = await _context.CRE_InitialReview.FirstOrDefaultAsync(ir => ir.UrecNo == urecNo),
                EthicsEvaluation = ethicsEvaluations,
                EthicsEvaluator = ethicsEvaluations.Select(e => e.EthicsEvaluator).FirstOrDefault(),
                DeclinedEthicsEvaluation = application.DeclinedEthicsEvaluation == null
            ? new List<DeclinedEthicsEvaluation>()
            : new List<DeclinedEthicsEvaluation> { application.DeclinedEthicsEvaluation }
            };

            return viewModel;
        }
        public async Task<bool> AreAllEvaluationsEvaluatedAsync(string urecNo)
        {
            var evaluations = await _context.CRE_EthicsEvaluation
                .Where(e => e.EthicsApplication.UrecNo == urecNo)
                .ToListAsync();

            return evaluations.All(e => e.EvaluationStatus == "Evaluated");
        }
        public async Task<EvaluationDetailsViewModel> GetEvaluationAndEvaluatorDetailsAsync(string urecNo, int evaluationId)
        {
            var evaluation = await _context.CRE_EthicsEvaluation
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.NonFundedResearchInfo) 
                        .ThenInclude(nf => nf.CoProponents) 
                .Include(e => e.EthicsApplication.EthicsApplicationLogs) 
                .Include(e => e.EthicsApplication.InitialReview) 
                .Include(e => e.EthicsEvaluator)
                .FirstOrDefaultAsync(e => e.EthicsApplication.UrecNo == urecNo && e.EvaluationId == evaluationId);

            if (evaluation == null)
                return null;

            // Retrieve evaluator user information, if available
            var evaluatorUser = evaluation.Name;

            // Create and return the view model
            return new EvaluationDetailsViewModel
            {
                EthicsApplication = evaluation.EthicsApplication,
                NonFundedResearchInfo = evaluation.EthicsApplication?.NonFundedResearchInfo,
                EthicsApplicationLog = evaluation.EthicsApplication?.EthicsApplicationLogs ?? new List<EthicsApplicationLogs>(),
                EthicsEvaluation = new List<EthicsEvaluation> { evaluation },
                InitialReview = evaluation.EthicsApplication?.InitialReview,
                EthicsEvaluator = evaluation.EthicsEvaluator ?? new EthicsEvaluator()
            };
        }
        public async Task<List<EthicsEvaluator>> GetAssignedEvaluatorsAsync(string urecNo)
        {
            var assignedEvaluators = await _context.CRE_EthicsEvaluation
                .Where(e => e.EvaluationStatus == "Assigned" && e.UrecNo == urecNo)
                .Select(e => e.EthicsEvaluator) // Select only the related evaluator
                .ToListAsync();

            return assignedEvaluators;
        }
        public async Task AddLogAsync(EthicsApplicationLogs log)
        {
            _context.CRE_EthicsApplicationLogs.Add(log); // Add the log to the DbSet
            await _context.SaveChangesAsync();  // Commit the changes to the database
        }
        public async Task<bool> SaveEthicsFormAsync(EthicsApplicationForms form)
        {
            try
            {
                _context.CRE_EthicsApplicationForms.Add(form);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                // Handle or log the exception as needed
                return false;
            }
        }
        


    }
}
