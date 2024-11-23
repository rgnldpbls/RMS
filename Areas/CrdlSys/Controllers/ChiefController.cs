using CrdlSys.Data; 
using CrdlSys.Models;
using CrdlSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MimeKit;
using MimeKit.Utils;
using Newtonsoft.Json;
using ClosedXML.Excel;
using CrdlSys.Services;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using ResearchManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;

namespace CrdlSys.Controllers
{ 
    [Area("CrdlSys")]
    public class ChiefController : Controller
    {
        private readonly CrdlDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ChiefController> _logger;
        private readonly CrdlDbContext _dbContext;
        private readonly ReportService _reportService;
        private readonly SentimentAnalysis _sentimentAnalyzer;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NotificationService _notificationService;

        public ChiefController(CrdlDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<ChiefController> logger, CrdlDbContext dbContext, ReportService reportService, 
            UserManager<ApplicationUser> userManager, NotificationService notificationService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _dbContext = dbContext;
            _reportService = reportService;
            string positiveFilePath = Path.Combine(webHostEnvironment.WebRootPath, "App_Data", "positive-words.txt");
            string negativeFilePath = Path.Combine(webHostEnvironment.WebRootPath, "App_Data", "negative-words.txt");
            _sentimentAnalyzer = new SentimentAnalysis(positiveFilePath, negativeFilePath);
            _reportService = reportService;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult DownloadReport(string reportId)
        {
            var report = _context.GeneratedReport.FirstOrDefault(r => r.ReportId == reportId);

            if (report == null)
            {
                return NotFound("Report not found.");
            }

            var memoryStream = new MemoryStream(report.GenerateReportFile);
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{report.FileName}.xlsx");
        }

        [HttpGet]
        public async Task<JsonResult> GetCalendarEvents()
        {
            var events = await _context.ResearchEvent
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

        // PWEDE MO TO GALAWIN JAIMMEE
        private string GetEventTypeColor(string eventType)
        {
            return eventType.ToLower() switch
            {
                "workshop" => "#4e73df",    // Blue
                "seminar" => "#1cc88a",     // Green
                "publication" => "#f6c23e",  // Yellow
                _ => "#e74a3b"              // Red for Others
            };
        }
        //ITO LANG PWEDE MONG GALAWIN

        [HttpGet]
        public async Task<JsonResult> GetDocumentTypeData()
        {
            var documentCounts = await _context.StakeholderUpload
                .GroupBy(d => d.TypeOfDocument)
                .Select(g => new
                {
                    TypeOfDocument = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.TypeOfDocument, x => x.Count);

            var moaCount = documentCounts.GetValueOrDefault("MOA", 0);
            var mouCount = documentCounts.GetValueOrDefault("MOU", 0);

            return Json(new
            {
                labels = new[] { "MOA", "MOU" },
                data = new[] { moaCount, mouCount }
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetResearchEventTypeData()
        {
            var eventCounts = await _context.ResearchEvent
                .Where(e => !e.IsArchived)
                .GroupBy(e => e.EventType)
                .Select(g => new
                {
                    EventType = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.EventType, x => x.Count);

            var workshopCount = eventCounts.GetValueOrDefault("Workshop", 0);
            var seminarCount = eventCounts.GetValueOrDefault("Seminar", 0);
            var publicationCount = eventCounts.GetValueOrDefault("Publication", 0);
            var othersCount = eventCounts.GetValueOrDefault("Others", 0);

            return Json(new
            {
                labels = new[] { "Workshop", "Seminar", "Publication", "Others" },
                data = new[] { workshopCount, seminarCount, publicationCount, othersCount }
            });
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeSentiment(IFormFile excelFile, string fileName, string researchEventId)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ModelState.AddModelError("", "Please upload a valid Excel file.");
                return View("~/Views/Chief/SentimentAnalysis.cshtml");
            }

            if (string.IsNullOrEmpty(researchEventId))
            {
                ModelState.AddModelError("", "Please select a Research Event.");
                return View("~/Views/Chief/SentimentAnalysis.cshtml");
            }

            byte[] surveyFileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await excelFile.CopyToAsync(memoryStream);
                surveyFileBytes = memoryStream.ToArray();
            }

            string extractedText = "";
            var scores = new List<int>();
            int positiveCount = 0;
            int negativeCount = 0;
            int totalWordCount = 0;

            var positives = new List<string>();
            var negatives = new List<string>();

            using (var stream = new MemoryStream(surveyFileBytes))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var workbook = new XLWorkbook(stream);
                {
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        foreach (var row in worksheet.RowsUsed().Skip(1))
                        {
                            foreach (var cell in row.CellsUsed())
                            {
                                string cellValue = cell.GetString();
                                extractedText += cellValue + " ";

                                if (int.TryParse(cellValue, out int score) && score >= 1 && score <= 5)
                                {
                                    scores.Add(score);
                                }
                                else
                                {
                                    totalWordCount++;
                                    var (degree, posWords, negWords) = _sentimentAnalyzer.AnalyzeSentiment(cellValue);
                                    positives.AddRange(posWords);
                                    negatives.AddRange(negWords);
                                    positiveCount += posWords.Count;
                                    negativeCount += negWords.Count;
                                }
                            }
                        }
                    }
                }
            }

            double averageScore = scores.Count > 0 ? scores.Average() : 0;
            double degreePercentage = totalWordCount > 0 ? ((positiveCount - negativeCount) / (double)totalWordCount) * 100 : 0;
            string tone;

            if (degreePercentage > 10)
            {
                tone = "Positive";
            }
            else if (degreePercentage < -10)
            {
                tone = "Negative";
            }
            else
            {
                tone = "Neutral";
            }

            var generatedReport = await _reportService.GenerateSentimentReport(averageScore, degreePercentage, tone, positives, negatives, fileName);

            byte[] generatedReportBytes;
            using (var memoryStream = new MemoryStream())
            {
                await memoryStream.WriteAsync(((FileContentResult)generatedReport).FileContents);
                generatedReportBytes = memoryStream.ToArray();
            }

            try
            {
                var sentimentAnalysis = new GeneratedSentimentAnalysis
                {
                    FileName = fileName,
                    GenerateReportFile = generatedReportBytes,
                    SurveyFile = surveyFileBytes,
                    ResearchEventId = researchEventId
                };

                _context.GeneratedSentimentAnalysis.Add(sentimentAnalysis);
                await _context.SaveChangesAsync();
                var researchEvent = await _context.ResearchEvent
                .FirstOrDefaultAsync(r => r.ResearchEventId == researchEventId);

                return Json(new
                {
                    success = true,
                    analysis = new
                    {
                        sentimentAnalysisId = sentimentAnalysis.SentimentAnalysisId,
                        fileName = sentimentAnalysis.FileName,
                        researchEvent = new
                        {
                            eventName = researchEvent?.EventName
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult SentimentAnalysis()
        {
            try
            {
                var currentTime = DateTime.Now;
                var researchEvents = _context.ResearchEvent
                    .Where(r => !r.IsArchived && r.EndTime < currentTime)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();

                ViewBag.ResearchEvents = researchEvents;

                var analyses = _context.GeneratedSentimentAnalysis
                    .Include(a => a.ResearchEvent)
                    .OrderByDescending(a => a.GeneratedAt)
                    .ToList();

                var viewModel = new SentimentAnalysisViewModel
                {
                    Analyses = analyses,
                    NewAnalysis = new GeneratedSentimentAnalysis()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in SentimentAnalysis action: {ErrorMessage}", ex.Message);
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAnalysisReport(string sentimentAnalysisId)
        {
            try
            {
                var analysis = await _context.GeneratedSentimentAnalysis
                    .FirstOrDefaultAsync(r => r.SentimentAnalysisId == sentimentAnalysisId);

                if (analysis == null || analysis.GenerateReportFile == null)
                {
                    return NotFound();
                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(analysis.GenerateReportFile, contentType, analysis.FileName + "_Report.xlsx");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAnalysisSurvey(string sentimentAnalysisId)
        {
            try
            {
                var analysis = await _context.GeneratedSentimentAnalysis
                    .FirstOrDefaultAsync(r => r.SentimentAnalysisId == sentimentAnalysisId);

                if (analysis == null || analysis.SurveyFile == null)
                {
                    return NotFound();
                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(analysis.SurveyFile, contentType, analysis.FileName + "_Survey.xlsx");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAnalysis(string sentimentAnalysisId)
        {
            try
            {
                var analysis = await _context.GeneratedSentimentAnalysis
                    .FirstOrDefaultAsync(a => a.SentimentAnalysisId == sentimentAnalysisId);

                if (analysis == null)
                {
                    _logger.LogWarning("Attempt to delete non-existent analysis with ID: {SentimentAnalysisId}", sentimentAnalysisId);
                    return Json(new { success = false, message = "Analysis not found" });
                }

                _context.GeneratedSentimentAnalysis.Remove(analysis);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted analysis with ID: {SentimentAnalysisId}", sentimentAnalysisId);
                return Json(new { success = true, message = "Analysis deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting analysis with ID {SentimentAnalysisId}: {ErrorMessage}", sentimentAnalysisId, ex.Message);
                return Json(new { success = false, message = "An error occurred while deleting the analysis" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleArchiveAnalysis(string sentimentAnalysisId, bool archive)
        {
            try
            {
                var analysis = await _context.GeneratedSentimentAnalysis
                    .FirstOrDefaultAsync(a => a.SentimentAnalysisId == sentimentAnalysisId);

                if (analysis == null)
                {
                    _logger.LogWarning("Attempt to archive non-existent analysis with ID: {SentimentAnalysisId}", sentimentAnalysisId);
                    return Json(new { success = false, message = "Analysis not found" });
                }

                analysis.IsArchived = archive;
                await _context.SaveChangesAsync();

                var actionType = archive ? "archived" : "unarchived";
                _logger.LogInformation("Successfully {ActionType} analysis with ID: {SentimentAnalysisId}", actionType, sentimentAnalysisId);
                return Json(new { success = true, message = $"Analysis {actionType} successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error toggling archive status for analysis {SentimentAnalysisId}: {ErrorMessage}", sentimentAnalysisId, ex.Message);
                return Json(new { success = false, message = "An error occurred while updating the archive status" });
            }
        }

        [Authorize(Roles = "CRDL Chief")]
        [HttpGet]
        public async Task<IActionResult> GenerateReport()
        {
            var events = await _context.ResearchEvent
                .Where(e => !e.IsArchived)
                .ToListAsync();

            var generatedReports = await _context.GeneratedReport.ToListAsync();

            var viewModel = new ReportViewModel
            {
                ResearchEvents = events,
                GeneratedReports = generatedReports
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateReport(string fileName, string reportType, int year, string? researchEventId = null)
        {
            try
            {
                byte[] excelData;

                if (reportType == "List of Contracts")
                {
                    excelData = await _reportService.GenerateStakeholderReportAsync();
                }
                else if (reportType == "List of New Partners")
                {
                    excelData = await _reportService.GenerateNewPartnersExcelAsync(year);
                }
                else if (reportType == "List of Events")
                {
                    excelData = await _reportService.GenerateEventReportExcelAsync(year);
                }
                else if (reportType == "List of Attendees")
                {
                    if (string.IsNullOrEmpty(researchEventId))
                    {
                        return BadRequest("ResearchEventId is required for attendees report.");
                    }
                    excelData = await _reportService.GenerateAttendeesListExcelAsync(researchEventId);
                }
                else if (reportType == "List of Renewal Contracts")
                {
                    excelData = await _reportService.GenerateRenewalHistoryExcelAsync(year);
                }
                else
                {
                    return BadRequest("Invalid report type selected.");
                }

                await _reportService.SaveGeneratedReportAsync(
                    fileName,
                    excelData,
                    (reportType == "List of Events" || reportType == "List of New Partners" || reportType == "Renewal History Report") ? (int?)year : null,
                    reportType,
                    researchEventId
                );

                var filePath = Path.Combine(Path.GetTempPath(), $"{fileName}.xlsx");
                await System.IO.File.WriteAllBytesAsync(filePath, excelData);

                return File(System.IO.File.OpenRead(filePath), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}.xlsx");
            }
            catch (InvalidOperationException ex) when (ex.Message.StartsWith("No events found"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report for reportType: {ReportType}", reportType);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateArchiveStatus(string reportId, bool isArchived)
        {
            try
            {
                var report = await _context.GeneratedReport.FindAsync(reportId);
                if (report == null)
                {
                    return Json(new { success = false });
                }

                report.IsArchived = isArchived;
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReport(string reportId)
        {
            try
            {
                var report = await _context.GeneratedReport.FindAsync(reportId);
                if (report == null)
                {
                    return Json(new { success = false });
                }

                _context.GeneratedReport.Remove(report);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [Authorize(Roles = "CRDL Chief")]
        public IActionResult ChiefHomePage()
        {
            var ongoingPartnersCount = _context.StakeholderUpload
                .Count(s => s.ContractStatus == "Active" && !s.IsArchived);
            ViewBag.OngoingPartnersCount = ongoingPartnersCount;

            var upcomingEventsCount = _context.ResearchEvent
                .Count(e => e.EventDate >= DateTime.Now && !e.IsArchived);
            ViewBag.UpcomingEventsCount = upcomingEventsCount;

            var finishedEventsCount = _context.ResearchEvent
                .Count(e => e.EndTime <= DateTime.Now && !e.IsArchived); 
            ViewBag.FinishedEventsCount = finishedEventsCount;

            var ongoingEventsCount = _context.ResearchEvent
                .Count(e => e.EventDate <= DateTime.Now && e.EndTime >= DateTime.Now && !e.IsArchived);
            ViewBag.OngoingEventsCount = ongoingEventsCount;

            var expiredContractsCount = _context.StakeholderUpload
                .Count(s => s.ContractStatus == "Expired");
            ViewBag.ExpiredContractsCount = expiredContractsCount;

            var pendingContractsCount = _context.StakeholderUpload
                .Count(s => s.Status == "Pending" && !s.IsArchived);
            ViewBag.PendingContractsCount = pendingContractsCount;

            var terminatedContractsCount = _context.StakeholderUpload
                .Count(s => s.ContractStatus == "Terminated");
            ViewBag.TerminatedContractsCount = terminatedContractsCount;

            var extendedContractsCount = _context.RenewalHistory.Count();
            ViewBag.ExtendedContractsCount = extendedContractsCount;
            return View();
        }

        public IActionResult SendDocument()
        {
            return View();
        }

        [Authorize(Roles ="CRDL Chief")]
        public async Task<IActionResult> HomeChief()
        {
            var stakeholderUploads = _context.StakeholderUpload.ToList();
            foreach (var upload in stakeholderUploads)
            {
                _notificationService.CheckAndSendNotification(upload);
                _notificationService.UpdateDocumentExpirationStatus();
            }

            var chiefUploads = _context.ChiefUpload.ToList();
            foreach (var upload in chiefUploads)
            {
                _notificationService.CheckAndSendChiefNotification(upload);
                _notificationService.UpdateDocumentChiefExpirationStatus();
            }

            await _notificationService.CheckAndSendEventReminders();
            await _notificationService.CheckAndSendPendingStakeholderReminders();
            return View();
        }

        [Authorize(Roles = "CRDL Chief")]
        public IActionResult ManageRCBAEvent()
        {
            var allEvents = _context.ResearchEvent.ToList();

            var viewModel = new ManageRCBAEventsPageViewModel
            {
                OpenRegistrationEvents = allEvents
                    .Where(e => e.RegistrationType == "Open Registration" && !e.IsArchived)
                    .Select(e => new ManageRCBAEventsViewModel
                    {
                        ResearchEventId = e.ResearchEventId,
                        EventThumbnail = e.EventThumbnail ?? Array.Empty<byte>(),
                        EventName = e.EventName,
                        EventDescription = e.EventDescription,
                        EventLocation = e.EventLocation,
                        EventType = e.EventType,
                        EventDate = e.EventDate,
                        EndTime = e.EndTime,
                        RegistrationOpen = e.RegistrationOpen,
                        RegistrationDeadline = e.RegistrationDeadline,
                        EventStatus = e.EventStatus,
                        ParticipantsSlot = e.ParticipantsSlot,
                        ParticipantsCount = e.ParticipantsCount,
                        IsArchived = e.IsArchived,
                        UpdatedAt = e.UpdatedAt
                    }).ToList(),

                InvitationalEvents = allEvents
                    .Where(e => e.RegistrationType == "Invitational" && !e.IsArchived)
                    .Select(e => new ManageRCBAEventsViewModel
                    {
                        ResearchEventId = e.ResearchEventId,
                        EventThumbnail = e.EventThumbnail ?? Array.Empty<byte>(),
                        EventName = e.EventName,
                        EventDescription = e.EventDescription,
                        EventLocation = e.EventLocation,
                        EventType = e.EventType,
                        EventDate = e.EventDate,
                        EndTime = e.EndTime,
                        RegistrationOpen = e.RegistrationOpen,
                        RegistrationDeadline = e.RegistrationDeadline,
                        EventStatus = e.EventStatus,
                        ParticipantsSlot = e.ParticipantsSlot,
                        ParticipantsCount = e.ParticipantsCount,
                        IsArchived = e.IsArchived,
                        UpdatedAt = e.UpdatedAt
                    }).ToList(),

                ArchivedEvents = allEvents
                    .Where(e => e.IsArchived)
                    .Select(e => new ManageRCBAEventsViewModel
                    {
                        ResearchEventId = e.ResearchEventId,
                        EventThumbnail = e.EventThumbnail ?? Array.Empty<byte>(),
                        EventName = e.EventName,
                        EventDescription = e.EventDescription,
                        EventLocation = e.EventLocation,
                        EventType = e.EventType,
                        EventDate = e.EventDate,
                        EndTime = e.EndTime,
                        RegistrationOpen = e.RegistrationOpen,
                        RegistrationDeadline = e.RegistrationDeadline,
                        EventStatus = e.EventStatus,
                        ParticipantsSlot = e.ParticipantsSlot,
                        ParticipantsCount = e.ParticipantsCount,
                        IsArchived = e.IsArchived,
                        UpdatedAt = e.UpdatedAt
                    }).ToList(),

                PostponedEvents = allEvents
                    .Where(e => e.EventStatus.Equals("Postponed", StringComparison.OrdinalIgnoreCase))
                    .Select(e => new ManageRCBAEventsViewModel
                    {
                        ResearchEventId = e.ResearchEventId,
                        EventThumbnail = e.EventThumbnail ?? Array.Empty<byte>(),
                        EventName = e.EventName,
                        EventDescription = e.EventDescription,
                        EventLocation = e.EventLocation,
                        EventType = e.EventType,
                        EventDate = e.EventDate,
                        EndTime = e.EndTime,
                        RegistrationOpen = e.RegistrationOpen,
                        RegistrationDeadline = e.RegistrationDeadline,
                        EventStatus = e.EventStatus,
                        ParticipantsSlot = e.ParticipantsSlot,
                        ParticipantsCount = e.ParticipantsCount,
                        IsArchived = e.IsArchived,
                        UpdatedAt = e.UpdatedAt
                    }).ToList(),
            };

            return View(viewModel);
        }

        [Authorize(Roles = "CRDL Chief")]
        [HttpGet]
        public async Task<IActionResult> ViewChiefUploads()
        {
            /*var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["Error"] = "Invalid user ID.";
                return View(new ViewChiefUploadsViewModel());
            }*/

            var uploadedDocuments = await _context.ChiefUpload
                .ToListAsync();

            var viewModel = new ViewChiefUploadsViewModel
            {
                UploadedDocuments = uploadedDocuments 
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocument(ViewChiefUploadsViewModel model)
        {
            if (User.Identity != null && !User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "You need to be logged in to upload documents.";
                return RedirectToAction("Login", "Account"); 
            }

            if (ModelState.IsValid)
            {
                /*var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    TempData["Error"] = "Invalid user ID.";
                    return View("ViewChiefUploads", model);
                }*/

                if (model.ContractStartDate.HasValue && model.ContractEndDate.HasValue)
                {
                    if (model.ContractStartDate > model.ContractEndDate)
                    {
                        TempData["Error"] = "Contract Start Date cannot be later than Contract End Date.";
                        return View("ViewChiefUploads", model);
                    }

                    model.ContractStatus = "Active";
                }

                if (model.ContractStartDate.HasValue && !model.ContractEndDate.HasValue)
                {
                    model.ContractStatus = "Active";
                }

                var chiefUpload = new ChiefUpload
                {
                    DocumentId = ChiefUpload.GenerateDocumentId(),
                    NameOfDocument = model.NameOfDocument,
                    TypeOfDocument = model.TypeOfDocument,
                    DocumentDescription = model.DocumentDescription,
                    StakeholderName = model.StakeholderName,
                    EmailOfStakeholder = model.EmailOfStakeholder,
                    ContractStartDate = model.ContractStartDate.HasValue ? DateOnly.FromDateTime(model.ContractStartDate.Value) : null,
                    ContractEndDate = model.ContractEndDate.HasValue ? DateOnly.FromDateTime(model.ContractEndDate.Value) : null,
                    Status = model.Status,
                    Comment = model.Comment,
                    ContractStatus = model.ContractStatus,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsArchived = false
                };

                if (model.DocumentFile != null && model.DocumentFile.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    {
                        await model.DocumentFile.CopyToAsync(memoryStream);
                        chiefUpload.DocumentFile = memoryStream.ToArray(); 
                    }
                }

                _context.ChiefUpload.Add(chiefUpload);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Document uploaded successfully!";
                return RedirectToAction("ViewChiefUploads");
            }

            return View("ViewChiefUploads", model);
        }

        [HttpPost]
        public async Task<ActionResult> SendEmail(SendDocumentViewModel model)
        {
            try
            {
                if (_webHostEnvironment == null)
                {
                    throw new InvalidOperationException("WebHostEnvironment is not initialized");
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("RMO CRDL", "rmocrdl@gmail.com")); 
                string recipientName = model.EmailOfStakeholder.Split('@')[0]; 
                message.To.Add(new MailboxAddress(recipientName, model.EmailOfStakeholder)); 
                message.Subject = model.Subject; 

                var bodyBuilder = new BodyBuilder();

                string footerImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "Footer.png");
                if (System.IO.File.Exists(footerImagePath))
                {
                    var image = bodyBuilder.LinkedResources.Add(footerImagePath);
                    image.ContentId = MimeUtils.GenerateMessageId();
                    string formattedBody = model.Body.Replace("\n", "<br>");
                    var htmlBody = $@"
                    <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='margin-bottom: 20px;'>
                                {formattedBody} <!-- Email body from the model -->
                            </div>
                            <footer style='margin-top: 20px;'>
                                <img src='cid:{image.ContentId}' alt='Footer Image' style='width: 100%; max-width: 600px; height: auto;' />
                            </footer>
                        </body>
                    </html>";

                    bodyBuilder.HtmlBody = htmlBody;
                    bodyBuilder.TextBody = Regex.Replace(model.Body, "<.*?>", string.Empty); 
                }
                else
                {
                    bodyBuilder.HtmlBody = model.Body; 
                }

                if (model.DocumentFile != null && model.DocumentFile.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    {
                        await model.DocumentFile.CopyToAsync(memoryStream); 
                        bodyBuilder.Attachments.Add(model.DocumentFile.FileName, memoryStream.ToArray(), ContentType.Parse(model.DocumentFile.ContentType));
                    }
                }

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate("rmocrdl@gmail.com", "gkmu momq yxkr dzsq"); 
                    client.Send(message);
                    client.Disconnect(true);
                }

                TempData["AlertMessage"] = "Email Sent Successfully!";
                TempData["AlertType"] = "success";

                /*var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    TempData["AlertMessage"] = "Invalid user ID.";
                    TempData["AlertType"] = "danger";
                    return View("SendDocument", model);
                }*/

                var chiefUpload = new ChiefUpload
                {
                    DocumentId = ChiefUpload.GenerateDocumentId(),
                    NameOfDocument = model.NameOfDocument,
                    TypeOfDocument = model.TypeOfDocument,
                    DocumentDescription = model.DocumentDescription,
                    StakeholderName = model.StakeholderName,
                    EmailOfStakeholder = model.EmailOfStakeholder,
                    Status = "Pending",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsArchived = false
                };

                if (model.DocumentFile != null && model.DocumentFile.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    {
                        await model.DocumentFile.CopyToAsync(memoryStream);
                        chiefUpload.DocumentFile = memoryStream.ToArray();
                    }
                }

                _context.ChiefUpload.Add(chiefUpload);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email: {Message}", ex.Message);

                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner Exception: {InnerMessage}", ex.InnerException.Message);
                }

                TempData["AlertMessage"] = $"Error occurred while sending email: {ex.Message}";
                TempData["AlertType"] = "danger";
            }

            return View("SendDocument", model);
        }

        public IActionResult Download(string id)
        {
            var document = _context.ChiefUpload.Find(id);
            if (document == null)
            {
                return NotFound();
            }

            return File(document.DocumentFile, "application/pdf", document.NameOfDocument + ".pdf");
        }

        public new IActionResult View(string id)
        {
            var document = _context.ChiefUpload.Find(id);
            if (document == null)
            {
                return NotFound();
            }

            return File(document.DocumentFile, "application/pdf");
        }

        [HttpPost]
        public IActionResult Update(string documentId, DateTime? contractStartDate, DateTime? contractEndDate, string status, string comment)
        {
            var document = _context.ChiefUpload.Find(documentId);
            if (document == null)
            {
                TempData["Error"] = "Document not found!";
                return RedirectToAction("ViewChiefUploads");
            }

            if (document.ContractStatus == "Terminated")
            {
                TempData["Error"] = "Updates are not allowed for a terminated contract. Only archival actions are permitted.";
                return RedirectToAction("ViewChiefUploads");
            }

            if (!contractStartDate.HasValue && contractEndDate.HasValue)
            {
                TempData["Error"] = "Contract start date must be provided when setting a contract end date!";
                return RedirectToAction("ViewChiefUploads");
            }

            if (contractStartDate.HasValue && contractEndDate.HasValue && contractEndDate < contractStartDate)
            {
                TempData["Error"] = "Contract end date cannot be earlier than contract start date!";
                return RedirectToAction("ViewChiefUploads");
            }

            if (document.TypeOfDocument == "MOA" &&
                document.ContractEndDate.HasValue &&
                document.ContractEndDate.Value.ToDateTime(new TimeOnly(0, 0)) <= DateTime.Now)
            {
                if (contractEndDate.HasValue &&
                    contractEndDate.Value > document.ContractEndDate.Value.ToDateTime(new TimeOnly(0, 0)))
                {
                    TempData["Error"] = "An expired MOA cannot be renewed. Kindly contact your stakeholder to upload a new MOA instead.";
                    return RedirectToAction("ViewChiefUploads");
                }
            }

            try
            {
                document.Status = status;
                document.Comment = comment;
                document.UpdatedAt = DateTime.Now;
                if ((status == "Pending" || status == "Rejected") && (contractStartDate.HasValue || contractEndDate.HasValue))
                {
                    TempData["Error"] = "You need to nullify the contract start and end dates before setting the status to " + status + "!";
                    return RedirectToAction("ViewChiefUploads");
                }
                else
                {
                    document.ContractStartDate = contractStartDate.HasValue ? DateOnly.FromDateTime(contractStartDate.Value) : (DateOnly?)null;
                    document.ContractEndDate = contractEndDate.HasValue ? DateOnly.FromDateTime(contractEndDate.Value) : (DateOnly?)null;
                }
                if (document.ContractEndDate.HasValue)
                {
                    if (document.ContractEndDate.Value <= DateOnly.FromDateTime(DateTime.Now))
                    {
                        document.ContractStatus = "Expired";
                    }
                    else
                    {
                        document.ContractStatus = "Active";
                    }
                }
                else if (document.ContractStartDate.HasValue && document.Status == "Approved")
                {
                    document.ContractStatus = "Active";
                }
                else
                {
                    document.ContractStatus = null;
                }
                _context.SaveChanges();
                TempData["Success"] = "Document updated successfully!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating document: {ex.Message}");
                TempData["Error"] = "Error updating document. Please try again.";
            }
            return RedirectToAction("ViewChiefUploads");
        }

        [HttpPost]
        public async Task<IActionResult> TerminateContract(string documentId)
        {
            var chiefUpload = await _context.ChiefUpload.FindAsync(documentId);

            if (chiefUpload == null)
            {
                return Json(new { success = false, message = "Document not found." });
            }

            if (chiefUpload.ContractStatus != "Active")
            {
                return Json(new { success = false, message = "Only contracts with an Active status can be terminated." });
            }

            if (chiefUpload.Status == "Pending" || chiefUpload.Status == "Rejected")
            {
                return Json(new { success = false, message = "Cannot terminate a contract with status Pending or Rejected." });
            }

            chiefUpload.ContractStatus = "Terminated";
            chiefUpload.IsArchived = true;

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [Authorize(Roles = "CRDL Chief")]
        public async Task<IActionResult> ViewStakeholderUploads()
        {
            var uploads = await _context.StakeholderUpload
                .Select(upload => new StakeholderUploadViewModel
                {
                    DocumentId = upload.DocumentId,
                    TrackingId = _context.DocumentTracking
                    .Where(dt => dt.DocumentId == upload.DocumentId)
                    .Select(dt => dt.TrackingId)
                    .FirstOrDefault() ?? "no-tracking-id",
                    StakeholderName = upload.StakeholderName,
                    NameOfDocument = upload.NameOfDocument,
                    TypeOfDocument = upload.TypeOfDocument,
                    TypeOfMOA = upload.TypeOfMOA!,
                    DocumentDescription = upload.DocumentDescription,
                    CreatedAt = upload.CreatedAt,
                    UpdatedAt = upload.UpdatedAt,
                    ContractStartDate = upload.ContractStartDate,
                    ContractEndDate = upload.ContractEndDate,
                    Status = upload.Status,
                    Comment = upload.Comment!,
                    IsArchived = upload.IsArchived,
                    ContractStatus = upload.ContractStatus!
                })
                .ToListAsync();

            return View(uploads);
        }
        public IActionResult ViewStakeholderDocument(string id)
        {
            var document = _context.StakeholderUpload.Find(id);
            if (document == null)
            {
                return NotFound();
            }

            return File(document.DocumentFile, "application/pdf");
        }

        public IActionResult DownloadStakeholderDocument(string id)
        {
            var document = _context.StakeholderUpload.Find(id);
            if (document == null)
            {
                return NotFound();
            }

            return File(document.DocumentFile, "application/pdf", document.NameOfDocument + ".pdf");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStakeholderUpload(UpdateStakeholderUploadViewModel model)
        {
            var stakeholderUpload = _context.StakeholderUpload
                .FirstOrDefault(s => s.DocumentId == model.DocumentId);

            if (stakeholderUpload == null)
            {
                TempData["ErrorMessage"] = "Document not found.";
                return RedirectToAction("ViewStakeholderUploads");
            }

            if (stakeholderUpload.ContractStatus == "Terminated")
            {
                TempData["ErrorMessage"] = "Updates are not allowed for a terminated contract. Only archival actions are permitted.";
                return RedirectToAction("ViewStakeholderUploads");
            }

            if (model.ContractStartDate.HasValue && model.ContractEndDate.HasValue &&
                model.ContractEndDate < model.ContractStartDate)
            {
                TempData["ErrorMessage"] = "Contract end date cannot be earlier than contract start date!";
                return RedirectToAction("ViewStakeholderUploads");
            }

            if (!model.ContractStartDate.HasValue && model.ContractEndDate.HasValue)
            {
                TempData["ErrorMessage"] = "Contract start date must be provided when setting a contract end date!";
                return RedirectToAction("ViewStakeholderUploads");
            }

            if (model.Status == "For Revision" && string.IsNullOrWhiteSpace(model.Comment))
            {
                TempData["ErrorMessage"] = "A comment is required when setting the status to 'For Revision'.";
                return RedirectToAction("ViewStakeholderUploads");
            }

            if (stakeholderUpload.TypeOfDocument == "MOA" &&
                stakeholderUpload.ContractEndDate.HasValue &&
                stakeholderUpload.ContractEndDate.Value <= DateOnly.FromDateTime(DateTime.Now))
            {
                if (model.ContractEndDate.HasValue && model.ContractEndDate > stakeholderUpload.ContractEndDate)
                {
                    TempData["ErrorMessage"] = "An expired MOA cannot be renewed. Kindly contact your stakeholder to upload a new MOA instead.";
                    return RedirectToAction("ViewStakeholderUploads");
                }
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if ((model.Status == "Pending" || model.Status == "For Revision") &&
                        (model.ContractStartDate.HasValue || model.ContractEndDate.HasValue))
                    {
                        TempData["ErrorMessage"] = $"You need to nullify the contract start and end dates before setting the status to {model.Status}!";
                        return RedirectToAction("ViewStakeholderUploads");
                    }

                    if (stakeholderUpload.TypeOfDocument == "MOU" &&
                        model.ContractEndDate.HasValue &&
                        stakeholderUpload.ContractEndDate.HasValue &&
                        model.ContractEndDate.Value != stakeholderUpload.ContractEndDate.Value &&
                        stakeholderUpload.ContractStatus == "Expired")
                    {
                        var renewalHistory = new RenewalHistory
                        {
                            Id = RenewalHistory.GenerateRenewalId(),
                            DocumentId = stakeholderUpload.DocumentId,
                            TypeOfDocument = stakeholderUpload.TypeOfDocument,
                            PreviousEndDate = stakeholderUpload.ContractEndDate.Value,
                            NewEndDate = model.ContractEndDate.Value,
                            RenewalDate = DateTime.UtcNow
                        };

                        _context.RenewalHistory.Add(renewalHistory);
                    }

                    string documentName = stakeholderUpload.NameOfDocument;
                    string stakeholderEmail = stakeholderUpload.StakeholdeEmail;

                    stakeholderUpload.ContractStartDate = model.ContractStartDate;
                    stakeholderUpload.ContractEndDate = model.ContractEndDate;

                    string previousStatus = stakeholderUpload.Status;
                    string? contractStatus = stakeholderUpload.ContractStatus;

                    stakeholderUpload.Status = model.Status;
                    stakeholderUpload.Comment = model.Comment;

                    stakeholderUpload.UpdatedAt = DateTime.Now;

                    if (model.ContractStartDate.HasValue && !model.ContractEndDate.HasValue)
                    {
                        stakeholderUpload.ContractStatus = "Active";
                    }
                    else if (model.ContractEndDate.HasValue)
                    {
                        if (model.ContractEndDate.Value <= DateOnly.FromDateTime(DateTime.Now))
                        {
                            stakeholderUpload.ContractStatus = "Expired";
                        }
                        else
                        {
                            stakeholderUpload.ContractStatus = "Active";
                        }
                    }
                    else
                    {
                        stakeholderUpload.ContractStatus = null;
                    }

                    string updatedContractStatus = stakeholderUpload.ContractStatus;

                    string statusToSend = (model.Status == "Approved" || model.Status == "For Revision")
                                          ? model.Status
                                          : "Status Updated";

                    await SendStakeholderUpdateNotification(stakeholderEmail, documentName, statusToSend, updatedContractStatus);

                    stakeholderUpload.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Stakeholder upload updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Please correct the errors in the form.";
                    TempData["ModelStateErrors"] = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating stakeholder upload: {ex.Message}");
                TempData["ErrorMessage"] = "Error updating stakeholder upload. Please try again.";
            }

            return RedirectToAction("ViewStakeholderUploads");
        }

        private static async Task SendStakeholderUpdateNotification(string stakeholderEmail, string documentName, string status, string? contractStatus = null)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("RMO CRDL Notification", "rmocrdlnotification@gmail.com"));
            message.To.Add(new MailboxAddress(stakeholderEmail, stakeholderEmail));
            message.Subject = "Document Status Updated";

            var bodyText = $"The status of your document '{documentName}' has been updated.\n\n" +
                           $"Status: {(string.IsNullOrEmpty(status) ? "Status not provided" : status)}\n";

            if (!string.IsNullOrEmpty(contractStatus))
            {
                bodyText += $"Contract Status: {contractStatus}\n";
            }

            bodyText += "Please check your account for more details.";

            message.Body = new TextPart("plain")
            {
                Text = bodyText
            };

            using var client = new MailKit.Net.Smtp.SmtpClient();
            {
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate("rmocrdlnotification@gmail.com", "uhflpdedbetywhxu");
                await client.SendAsync(message);
                client.Disconnect(true);
            }
        }

        [HttpPost]
        public async Task<IActionResult> TerminateStakeholderContract(string documentId)
        {
            var stakeholderUpload = await _context.StakeholderUpload
                                                      .FirstOrDefaultAsync(s => s.DocumentId == documentId);
            if (stakeholderUpload == null)
            {
                return Json(new { success = false, message = "Document not found." });
            }

            if (stakeholderUpload.ContractStatus != "Active")
            {
                return Json(new { success = false, message = "Only contracts with an Active status can be terminated." });
            }

            if (stakeholderUpload.Status == "Pending" || stakeholderUpload.Status == "For Revision")
            {
                return Json(new { success = false, message = "Cannot terminate a contract with status Pending or For Revision." });
            }

            stakeholderUpload.ContractStatus = "Terminated";
            stakeholderUpload.IsArchived = true;

            await _context.SaveChangesAsync();

            var stakeholderEmail = stakeholderUpload.StakeholdeEmail;
            var documentName = stakeholderUpload.NameOfDocument;

            await SendStakeholderUpdateNotification(stakeholderEmail, documentName, stakeholderUpload.Status, stakeholderUpload.ContractStatus);

            return Json(new { success = true });
        }

        [Authorize(Roles = "CRDL Chief")]
        [HttpGet]
        public IActionResult UpdateTracker(string trackingId)
        {
            var trackingRecord = _context.DocumentTracking
                .Where(t => t.TrackingId == trackingId)
                .Select(t => new DocumentTrackingViewModel
                {
                    TrackingId = t.TrackingId,
                    IsReceivedByRMO = t.IsReceivedByRMO,
                    IsSubmittedToOVPRED = t.IsSubmittedToOVPRED,
                    IsSubmittedToLegalOffice = t.IsSubmittedToLegalOffice,
                    IsReceivedByOVPRED = t.IsReceivedByOVPRED,
                    IsReceivedByRMOAfterOVPRED = t.IsReceivedByRMOAfterOVPRED,
                    IsSubmittedToOfficeOfThePresident = t.IsSubmittedToOfficeOfThePresident,
                    IsReceivedByRMOAfterOfficeOfThePresident = t.IsReceivedByRMOAfterOfficeOfThePresident,
                    UpdatedAt = t.UpdatedAt,
                    ActivityHistory = _context.DocumentTracking
                        .Where(x => x.DocumentId == t.DocumentId)
                        .Select(x => new ActivityHistoryViewModel
                        {
                            IsReceivedByRMO = x.IsReceivedByRMO,
                            IsReceivedByRMOUpdatedAt = x.IsReceivedByRMOUpdatedAt ?? (DateTime?)null,
                            IsSubmittedToOVPRED = x.IsSubmittedToOVPRED,
                            IsSubmittedToOVPREDUpdatedAt = x.IsSubmittedToOVPREDUpdatedAt,
                            IsSubmittedToLegalOffice = x.IsSubmittedToLegalOffice,
                            IsSubmittedToLegalOfficeUpdatedAt = x.IsSubmittedToLegalOfficeUpdatedAt,
                            IsReceivedByOVPRED = x.IsReceivedByOVPRED,
                            IsReceivedByOVPREDUpdatedAt = x.IsReceivedByOVPREDUpdatedAt,
                            IsReceivedByRMOAfterOVPRED = x.IsReceivedByRMOAfterOVPRED,
                            IsReceivedByRMOAfterOVPREDUpdatedAt = x.IsReceivedByRMOAfterOVPREDUpdatedAt,
                            IsSubmittedToOfficeOfThePresident = x.IsSubmittedToOfficeOfThePresident,
                            IsSubmittedToOfficeOfThePresidentUpdatedAt = x.IsSubmittedToOfficeOfThePresidentUpdatedAt,
                            IsReceivedByRMOAfterOfficeOfThePresident = x.IsReceivedByRMOAfterOfficeOfThePresident,
                            IsReceivedByRMOAfterOfficeOfThePresidentUpdatedAt = x.IsReceivedByRMOAfterOfficeOfThePresidentUpdatedAt,
                        })
                        .OrderByDescending(x => x.IsReceivedByRMOUpdatedAt ?? DateTime.MinValue)
                        .ToList()
                })
                .FirstOrDefault();

            if (trackingRecord == null)
            {
                return NotFound();
            }

            return View(trackingRecord);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTracker(string trackingId, string status)
        {
            var trackingRecord = await _context.DocumentTracking.FirstOrDefaultAsync(t => t.TrackingId == trackingId);

            if (trackingRecord == null)
            {
                return NotFound();
            }

            var document = await _context.StakeholderUpload.FirstOrDefaultAsync(d => d.DocumentId == trackingRecord.DocumentId);

            if (document == null)
            {
                TempData["ErrorMessage"] = "Document not found.";
                return RedirectToAction("ViewChiefUploads");
            }

            if (document.TypeOfDocument == "MOA" && document.TypeOfMOA != "Others")
            {
                TempData["ErrorMessage"] = "Tracking is not required for MOA documents unless TypeOfMOA is 'Others'.";
                return RedirectToAction("ViewChiefUploads");
            }

            switch (status)
            {
                case "Received by RMO":
                    trackingRecord.IsReceivedByRMO = true;
                    trackingRecord.IsReceivedByRMOUpdatedAt = DateTime.Now;
                    break;
                case "Submitted to OVPRED":
                    trackingRecord.IsSubmittedToOVPRED = true;
                    trackingRecord.IsSubmittedToOVPREDUpdatedAt = DateTime.Now;
                    break;
                case "Submitted to Legal Office":
                    trackingRecord.IsSubmittedToLegalOffice = true;
                    trackingRecord.IsSubmittedToLegalOfficeUpdatedAt = DateTime.Now;
                    break;
                case "Received by OVPRED":
                    trackingRecord.IsReceivedByOVPRED = true;
                    trackingRecord.IsReceivedByOVPREDUpdatedAt = DateTime.Now;
                    break;
                case "Received by RMO (After OVPRED)":
                    trackingRecord.IsReceivedByRMOAfterOVPRED = true;
                    trackingRecord.IsReceivedByRMOAfterOVPREDUpdatedAt = DateTime.Now;
                    break;
                case "Submitted to Office of the President":
                    trackingRecord.IsSubmittedToOfficeOfThePresident = true;
                    trackingRecord.IsSubmittedToOfficeOfThePresidentUpdatedAt = DateTime.Now;
                    break;
                case "Received by RMO (After Office of the President)":
                    trackingRecord.IsReceivedByRMOAfterOfficeOfThePresident = true;
                    trackingRecord.IsReceivedByRMOAfterOfficeOfThePresidentUpdatedAt = DateTime.Now;
                    break;
                default:
                    return RedirectToAction("ViewChiefUploads");
            }

            trackingRecord.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            /*var stakeholderEmail = await _context.Users
                .Where(u => u.UserId == document.StakeholderId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();*/

            if (document.StakeholdeEmail != null)
            {
                await SendStatusUpdateNotification(document.StakeholdeEmail, document.NameOfDocument, status);
            }
            return RedirectToAction("UpdateTracker", new { trackingId = trackingRecord.TrackingId });
        }

        private static async Task SendStatusUpdateNotification(string stakeholderEmail, string documentName, string status)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("RMO CRDL Notification", "rmocrdlnotification@gmail.com"));
            message.To.Add(new MailboxAddress(stakeholderEmail, stakeholderEmail));
            message.Subject = "Document Status Updated";

            message.Body = new TextPart("plain")
            {
                Text = $"The status of your document '{documentName}' has been updated.\n\n" +
                       $"New Status: {status}\n" +
                       $"Please check your account for more details."
            };

            using var client = new MailKit.Net.Smtp.SmtpClient();
            {
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate("rmocrdlnotification@gmail.com", "uhflpdedbetywhxu");
                await client.SendAsync(message);
                client.Disconnect(true);
            }
        }

        [HttpPost]
        public IActionResult ArchiveDocument(string documentId)
        {
            var document = _context.ChiefUpload.Find(documentId);
            if (document != null)
            {
                document.IsArchived = true;
                document.IsManuallyUnarchived = false;
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult UnarchiveDocument(string documentId)
        {
            var document = _context.ChiefUpload.Find(documentId);
            if (document != null)
            {
                document.IsArchived = false;
                document.IsManuallyUnarchived = true;
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult DeleteDocument(string documentId)
        {
            var document = _context.ChiefUpload.Find(documentId);

            if (document == null)
            {
                return Json(new { success = false, message = "Document not found." });
            }

            _context.ChiefUpload.Remove(document); 
            _context.SaveChanges(); 

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult StakeholderArchiveDocument(string documentId)
        {
            var document = _context.StakeholderUpload.Find(documentId);
            if (document != null)
            {
                document.IsArchived = true;
                document.IsManuallyUnarchived = false;
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult StakeholderUnarchiveDocument(string documentId)
        {
            var document = _context.StakeholderUpload.Find(documentId);
            if (document != null)
            {
                document.IsArchived = false;
                document.IsManuallyUnarchived = true;
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult StakeholderDeleteDocument(string documentId)
        {
            var document = _context.StakeholderUpload.Find(documentId);

            if (document == null)
            {
                return Json(new { success = false, message = "Document not found." });
            }

            _context.StakeholderUpload.Remove(document); 
            _context.SaveChanges(); 

            return Json(new { success = true });
        }

        [Authorize(Roles = "CRDL Chief")]
        [HttpGet]
        public IActionResult CreateRCBAEvent()
        {
            var model = new CreateRCBAEventViewModel
            {
                SelectedResearchers = new(),
                EventDate = DateTime.Now,
                RegistrationOpen = DateTime.Now,
                RegistrationDeadline = DateTime.Now.AddDays(7)
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRCBAEvent(CreateRCBAEventViewModel model)
        {
           
            if (ModelState.IsValid)
            {
                if (model.RegistrationOpen >= model.EventDate)
                {
                    ModelState.AddModelError(nameof(model.RegistrationOpen), "The registration open date must be before the event date.");
                }

                if (model.RegistrationDeadline >= model.EventDate)
                {
                    ModelState.AddModelError(nameof(model.RegistrationDeadline), "The registration deadline must be before the event date.");
                }

                if (model.RegistrationDeadline <= model.RegistrationOpen)
                {
                    ModelState.AddModelError(nameof(model.RegistrationDeadline), "The registration deadline must be before the registration open.");
                }

                if (model.SelectedResearchers != null && model.SelectedResearchers.Count > model.ParticipantSlot)
                {
                    ModelState.AddModelError(nameof(model.ParticipantSlot), "The number of selected participants cannot exceed the participant slots available.");
                }
                else if (model.RegistrationType == "Invitational" && (model.SelectedResearchers == null || model.SelectedResearchers.Count < 1))
                {
                    ModelState.AddModelError(nameof(model.SelectedResearchers), "At least one participant must be selected for invitational events.");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var researchEventId = ResearchEvent.GenerateResearchEventId();

                var researchEvent = new ResearchEvent
                {
                    ResearchEventId = researchEventId,
                    EventName = model.EventName,
                    EventDescription = model.EventDescription,
                    EventThumbnail = model.EventThumbnail != null ? ConvertToBytes(model.EventThumbnail) : null,
                    EventType = model.EventType,
                    EventLocation = model.EventLocation,
                    RegistrationType = model.RegistrationType,
                    EventDate = model.EventDate,
                    EndTime = model.EndTime,
                    RegistrationOpen = model.RegistrationOpen,
                    RegistrationDeadline = model.RegistrationDeadline,
                    ParticipantsSlot = model.ParticipantSlot,
                    EventStatus = "Scheduled"
                };

                _context.ResearchEvent.Add(researchEvent);
                _context.SaveChanges();

                if (model.RegistrationType == "Invitational" && model.SelectedResearchers != null)
                {
                    _logger.LogInformation("SelectedResearchers received from frontend: {SelectedResearchers}",
                        string.Join(", ", model.SelectedResearchers ?? new List<string>()));

                    // Fetch all users matching the selected researchers
                    var selectedUsers = _userManager.Users
                        .Where(u => model.SelectedResearchers.Contains(u.Id))
                        .Select(u => new
                        {
                            u.Id,
                            FullName = $"{u.FirstName} {u.LastName}",
                            u.Email
                        })
                        .ToList();

                    if (!selectedUsers.Any())
                    {
                        _logger.LogWarning("No valid users found for the provided SelectedResearchers list.");
                        return View(model);
                    }

                    foreach (var user in selectedUsers)
                    {
                        bool invitationExists = _context.ResearchEventInvitation.Any(inv =>
                            inv.ResearchEventId == researchEventId && inv.UserId == user.Id);

                        _logger.LogInformation("Invitation already exists for researcher ID {UserId}: {InvitationExists}", user.Id, invitationExists);

                        if (!invitationExists)
                        {
                            var invitation = new ResearchEventInvitation
                            {
                                InvitationId = ResearchEventInvitation.GenerateInvitationId(),
                                ResearchEventId = researchEventId,
                                UserName = user.FullName,
                                UserEmail = user.Email,
                                UserId = user.Id,
                                InvitationStatus = "Pending",
                                InvitedAt = DateTime.Now
                            };

                            _context.ResearchEventInvitation.Add(invitation);
                            _logger.LogInformation("Added invitation for researcher ID {UserId}.", user.Id);

                            try
                            {
                                SendEmailInvitation(user.Email, user.FullName, model.EventName);
                                _logger.LogInformation("Email invitation sent to {Email} for event {EventName}.", user.Email, model.EventName);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Failed to send email to {Email} for event {EventName}.", user.Email, model.EventName);
                            }
                        }
                    }

                    var changes = await _context.SaveChangesAsync();
                    _logger.LogInformation("{Changes} invitations saved successfully.", changes);
                }

                TempData["Success"] = "RCBA Event created successfully!";
                return View(new CreateRCBAEventViewModel());
            }

            return View(model);
        }

        /*var currentUser = await _userManager.GetUserAsync(User);

                                if (currentUser.Id == researcherId)
                                {
                                    SendEmailInvitation(currentUser.Email, $"{currentUser.FirstName} {currentUser.LastName}", model.EventName);
                                }*/

        private void SendEmailInvitation(string recipientEmail, string userName, string eventName)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("RMO CRDL", "rmocrdl@gmail.com"));
            message.To.Add(new MailboxAddress(recipientEmail, recipientEmail));
            message.Subject = $"Invitation to {eventName}";

            var bodyBuilder = new BodyBuilder();

            string footerImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "Footer.png");
            if (System.IO.File.Exists(footerImagePath))
            {
                var image = bodyBuilder.LinkedResources.Add(footerImagePath);
                image.ContentId = MimeUtils.GenerateMessageId();

                var htmlBody = $@"
                <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <div style='margin-bottom: 20px;'>
                            Greetings, {userName}!<br><br>
                            You have been invited to the event: <strong>{eventName}</strong>. Please check your portal for more details.<br><br>
                            Best regards,<br>
                        </div>
                        <footer style='margin-top: 20px;'>
                            <img src='cid:{image.ContentId}' alt='Footer Image' style='width: 100%; max-width: 600px; height: auto;' />
                        </footer>
                    </body>
                </html>";

                bodyBuilder.HtmlBody = htmlBody;
            }
            else
            {
                bodyBuilder.TextBody = $"Greetings, {userName}!\n\nYou have been invited to the event: {eventName}. Please check your portal for more details.\n\nBest regards,\nRMO CRDL";
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            {
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate("rmocrdl@gmail.com", "gkmu momq yxkr dzsq");
                client.Send(message);
                client.Disconnect(true);
            }
        }

        private static byte[] ConvertToBytes(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public IActionResult GetEventDetails(string id)
        {
            Console.WriteLine($"Received Event ID: {id}");
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Event ID cannot be null or empty.");
            }

            try
            {
                var researchEvent = _context.ResearchEvent
                    .Where(e => e.ResearchEventId == id)
                    .Select(e => new ResearchEventViewModel
                    {
                        ResearchEventId = e.ResearchEventId ?? string.Empty,
                        EventName = e.EventName ?? "Unnamed Event",
                        EventDescription = e.EventDescription ?? string.Empty,
                        EventLocation = e.EventLocation ?? string.Empty,
                        EventType = e.EventType ?? string.Empty,
                        RegistrationType = e.RegistrationType ?? string.Empty,
                        EventDate = e.EventDate,
                        EndTime = e.EndTime,
                        RegistrationOpen = e.RegistrationOpen,
                        RegistrationDeadline = e.RegistrationDeadline,
                        EventStatus = e.EventStatus ?? string.Empty,
                        ParticipantsSlot = e.ParticipantsSlot,
                        ParticipantsCount = e.ParticipantsCount,
                        EventThumbnail = e.EventThumbnail,
                        IsArchived = e.IsArchived,
                        UpdatedAt = e.UpdatedAt

                    })
                    .FirstOrDefault();

                if (researchEvent == null)
                {
                    return NotFound($"Event with ID {id} not found.");
                }

                return Json(researchEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching event details: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching event details.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEvent([FromForm] ResearchEventViewModel model, IFormFile eventThumbnail, string userIdsToInvite)
        {
            try
            {
                var existingEvent = _context.ResearchEvent
                    .FirstOrDefault(e => e.ResearchEventId == model.ResearchEventId);

                if (existingEvent == null)
                {
                    return Json(new { success = false, message = "Event not found." });
                }

                if (existingEvent.EventStatus == "Cancelled")
                {
                    return Json(new { success = false, message = "Cannot update an event with status 'Cancelled'. Only deletion is allowed." });
                }

                bool isStatusUpdated = existingEvent.EventStatus != model.EventStatus;

                if (model.RegistrationOpen >= model.EventDate)
                {
                    ModelState.AddModelError("RegistrationOpen", "Registration Open date must be before the Event Date.");
                }
                if (model.RegistrationDeadline >= model.EventDate)
                {
                    ModelState.AddModelError("RegistrationDeadline", "Registration Deadline must be before the Event Date.");
                }

                if (model.RegistrationDeadline <= model.RegistrationOpen)
                {
                    ModelState.AddModelError(nameof(model.RegistrationDeadline), "The registration deadline must be before the registration open.");
                }

                if (model.ParticipantsCount > model.ParticipantsSlot)
                {
                    ModelState.AddModelError("ParticipantsCount", "Participants Count cannot exceed Participant Slots.");
                }

                existingEvent.IsArchived = model.EventStatus == "Cancelled";

                if (eventThumbnail != null && eventThumbnail.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    {
                        eventThumbnail.CopyTo(memoryStream);
                        existingEvent.EventThumbnail = memoryStream.ToArray();
                    }
                }
                else
                {
                    ModelState.Remove("EventThumbnail"); 
                }

                if (ModelState.IsValid)
                {
                    existingEvent.EventName = model.EventName;
                    existingEvent.EventDescription = model.EventDescription;
                    existingEvent.EventLocation = model.EventLocation;
                    existingEvent.EventType = model.EventType;
                    existingEvent.RegistrationType = model.RegistrationType;
                    existingEvent.EventDate = model.EventDate;
                    existingEvent.EndTime = model.EndTime;
                    existingEvent.RegistrationOpen = model.RegistrationOpen;
                    existingEvent.RegistrationDeadline = model.RegistrationDeadline;
                    existingEvent.EventStatus = model.EventStatus;
                    existingEvent.ParticipantsSlot = model.ParticipantsSlot;
                    existingEvent.ParticipantsCount = model.ParticipantsCount;
                    existingEvent.UpdatedAt = DateTime.Now;

                    _context.SaveChanges();

                    Console.WriteLine($"userIdsToInvite: {userIdsToInvite}");

                    List<string> userIdsToInviteList = new();
                    if (!string.IsNullOrEmpty(userIdsToInvite))
                    {
                        userIdsToInviteList = JsonConvert.DeserializeObject<List<string>>(userIdsToInvite) ?? new();
                    }

                    if (model.RegistrationType == "Invitational" && userIdsToInviteList.Count > model.ParticipantsSlot)
                    {
                        return Json(new { success = false, message = "You cannot invite more researchers than the available participant slots." });
                    }

                    int acceptedAndPendingInvitationsCount = GetAcceptedAndPendingInvitationsCount(model.ResearchEventId);

                    if (acceptedAndPendingInvitationsCount + 1 > model.ParticipantsSlot)
                    {
                        ModelState.AddModelError("ParticipantsCount", "You cannot invite more participants than available slots.");
                    }

                    if (model.RegistrationType == "Invitational" && userIdsToInviteList.Count > 0)
                    {
                        foreach (var userId in userIdsToInviteList) 
                        {
                            var user = await _userManager.Users.Where(e => e.Id == userId).FirstAsync();
                            var invitation = new ResearchEventInvitation
                            {
                                InvitationId = ResearchEventInvitation.GenerateInvitationId(),
                                ResearchEventId = existingEvent.ResearchEventId,
                                UserName = $"{user.FirstName} {user.LastName}",
                                UserEmail = user.Email,
                                UserId = user.Id,
                                InvitationStatus = "Pending",
                                InvitedAt = DateTime.Now
                            };
                            _context.ResearchEventInvitation.Add(invitation);
                            Console.WriteLine($"Added invitation for User ID: {userId}");

                            SendEmailInvitation(user.Email, $"{user.FirstName} {user.LastName}", existingEvent.EventName);
                            
                        }

                        _context.SaveChanges(); 
                    }

                    if (isStatusUpdated)
                    {
                        var registeredUsers = _context.ResearchEventRegistration
                            .Where(r => r.ResearchEventId == existingEvent.ResearchEventId)
                            .ToList();

                        foreach (var user in registeredUsers)
                        {
                            var curUser = await _userManager.GetUserAsync(User);
                            if(user.UserId == curUser.Id)
                            {
                                SendEmailStatusUpdate(curUser.Email, $"{curUser.FirstName} {curUser.LastName}", existingEvent.EventName, model.EventStatus);
                            }
                        }
                    }

                    return Json(new { success = true, message = "Event updated successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating event: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return Json(new { success = false, message = "An error occurred while updating the event." });
            }
        }

        /*var user = await _userManager.GetUserAsync(User);
                            if (user.Id == userId)
                            {
                                SendEmailInvitation(user.Email, $"{user.FirstName} {user.LastName}", existingEvent.EventName); 
                            }*/

        private static void SendEmailStatusUpdate(string recipientEmail, string userName, string eventName, string newStatus)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("RMO CRDL Notification", "rmocrdlnotification@gmail.com"));
            message.To.Add(new MailboxAddress(recipientEmail, recipientEmail));
            message.Subject = $"Status Update for {eventName}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
            <html>
                <body>
                    <p>Dear {userName},</p>
                    <p>The status of the event <strong>{eventName}</strong> has been updated to: <strong>{newStatus}</strong>.</p>
                    <p>Please check your portal for further details.</p>
                </body>
            </html>"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            {
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate("rmocrdlnotification@gmail.com", "uhfl pded bety whxu");
                client.Send(message);
                client.Disconnect(true);
            }
        }

        private int GetAcceptedAndPendingInvitationsCount(string researchEventId)
        {
            return _dbContext.ResearchEventInvitation
                .Count(invite => invite.ResearchEventId == researchEventId &&
                                 (invite.InvitationStatus == "Accepted" || invite.InvitationStatus == "Pending"));
        }

        [HttpPost]
        public IActionResult ArchiveEvent(string id)
        {
            try
            {
                var existingEvent = _context.ResearchEvent.FirstOrDefault(e => e.ResearchEventId == id);
                if (existingEvent == null)
                {
                    return NotFound(new { message = "Event not found." });
                }

                existingEvent.IsArchived = true;
                existingEvent.EventStatus = "Cancelled";
                _context.SaveChanges();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error archiving event: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while archiving the event." });
            }
        }

        [HttpPost]
        public IActionResult DeleteEvent(string id)
        {
            try
            {
                var existingEvent = _context.ResearchEvent.FirstOrDefault(e => e.ResearchEventId == id);
                if (existingEvent == null)
                {
                    return NotFound(new { message = "Event not found." });
                }

                _context.ResearchEvent.Remove(existingEvent);
                _context.SaveChanges();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting event: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting the event." });
            }
        }

        [HttpPost]
        public IActionResult UnarchiveEvent(string id)
        {
            try
            {
                var existingEvent = _context.ResearchEvent.FirstOrDefault(e => e.ResearchEventId == id);
                if (existingEvent == null)
                {
                    return NotFound(new { message = "Event not found." });
                }

                existingEvent.IsArchived = false;
                existingEvent.EventStatus = "Scheduled";
                _context.SaveChanges();

                return Ok(new { success = true, message = "Event unarchived successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unarchiving event: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while unarchiving the event." });
            }
        }

        public async Task<IActionResult> GetResearchers(string eventId)
        {
            var invitedResearchers = _context.ResearchEventInvitation
                .Where(invite => invite.ResearchEventId == eventId)
                .Select(invite => invite.UserId)
                .ToList();

            var researchers = await _userManager.Users
                .Where(ur => !invitedResearchers.Contains(ur.Id)) 
                .Select(ur => new
                {
                    ur.FirstName,
                    ur.LastName,
                    userId = ur.Id
                })
                .ToListAsync();

            if (researchers.Count == 0)
            {
                Console.WriteLine("No uninvited researchers found for event with ID: " + eventId);
            }

            return Json(researchers);
        }

        public IActionResult GetInvitedParticipants(string eventId)
        {
            var invitedParticipants = _dbContext.ResearchEventInvitation
                .Where(inv => inv.ResearchEventId == eventId)
                .Select(inv => new
                {
                    Name = inv.UserName,
                    Email = inv.UserEmail,
                    InvitationStatus = inv.InvitationStatus,
                    UserId = inv.UserId,
                    inv.InvitationId
                })
                .ToList();

            return Json(invitedParticipants);
        }

        [HttpGet]
        public IActionResult GetRegisteredParticipants(string eventId)
        {
            try
            {
                if (string.IsNullOrEmpty(eventId))
                {
                    return BadRequest("Event ID is required");
                }

                var registeredParticipants = _dbContext.ResearchEventRegistration
                    .Where(r => r.ResearchEventId == eventId)
                    .Select(r => new
                    {
                        RegistrationId = r.RegistrationId,
                        FullName = r.UserName,
                        Email = r.UserEmail,
                        UserId = r.UserId
                    })
                    .ToList();

                Console.WriteLine($"Found {registeredParticipants.Count} participants for event {eventId}");
                foreach (var participant in registeredParticipants)
                {
                    Console.WriteLine($"Participant: {participant.FullName}");
                }

                return Json(registeredParticipants);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching participants: " + ex.Message);
            }
        }

        [HttpPost]
        public IActionResult DeleteInvitation(string invitationId)
        {
            Console.WriteLine("Received invitation ID for deletion: " + invitationId); 

            if (string.IsNullOrEmpty(invitationId))
            {
                return BadRequest("Invitation ID is required.");
            }

            var invitation = _context.ResearchEventInvitation.Find(invitationId);
            if (invitation == null)
            {
                return NotFound("Invitation not found.");
            }

            _context.ResearchEventInvitation.Remove(invitation);
            _context.SaveChanges();

            return Ok("Invitation deleted successfully.");
        }
    }
}