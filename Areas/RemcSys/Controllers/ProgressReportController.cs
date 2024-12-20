﻿using Humanizer.Bytes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemcSys.Data;
using RemcSys.Models;
using ResearchManagementSystem.Models;

namespace RemcSys.Controllers
{
    [Area("RemcSys")]
    public class ProgressReportController : Controller
    {
        private readonly RemcDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ActionLoggerService _actionLogger;

        public ProgressReportController(RemcDBContext context, UserManager<ApplicationUser> userManager, ActionLoggerService actionLogger)
        {
            _context = context;
            _userManager = userManager;
            _actionLogger = actionLogger;
        }

        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> ProgressTracker()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var fr = await _context.REMC_FundedResearches
                .Where(f => f.UserId == user.Id && f.isArchive == false)
                .FirstOrDefaultAsync();
            if (fr == null)
            {
                return NotFound();
            }
            ViewBag.Id = fr.fr_Id;
            ViewBag.Research = fr.research_Title;
            ViewBag.Lead = fr.team_Leader;
            ViewBag.Members = fr.team_Members;
            ViewBag.Field = fr.field_of_Study;
            ViewBag.Status = fr.status;
            ViewBag.Extend1 = fr.isExtension1;
            ViewBag.Extend2 = fr.isExtension2;

            int numReports = 4;
            int interval = fr.project_Duration / numReports;
            List<DateTime> deadlines = new List<DateTime>();
            for (int i = 1; i <= numReports; i++)
            {
                var deadline = fr.start_Date.AddMonths(i * interval);
                deadlines.Add(deadline);
            }

            if (deadlines.Count >= 1) ViewBag.Report1 = deadlines[0];
            if (deadlines.Count >= 2) ViewBag.Report2 = deadlines[1];
            if (deadlines.Count >= 3) ViewBag.Report3 = deadlines[2];
            if (deadlines.Count >= 4) ViewBag.Report4 = deadlines[3];

            ViewBag.Report5 = fr.end_Date.AddMonths(interval);
            ViewBag.Report6 = fr.end_Date.AddMonths(interval * 2);

            var existingRep = await _context.REMC_ProgressReports
                .Where(pr => pr.fr_Id == fr.fr_Id)
                .ToListAsync();

            int reportNum = existingRep.Count;
            ViewBag.Count = reportNum;


            var logs = await _context.REMC_ActionLogs
                    .Where(f => f.Name == fr.team_Leader && f.isTeamLeader == true && f.FraId == fr.fra_Id)
                    .OrderByDescending(log => log.Timestamp)
                    .ToListAsync();
            
            return View(logs);
        }

        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> UploadProgReport(string id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var fr = await _context.REMC_FundedResearches.FindAsync(id);
            if(fr == null)
            {
                return NotFound();
            }

            ViewBag.Id = fr.fr_Id;
            ViewBag.Research = fr.research_Title;
            ViewBag.Lead = fr.team_Leader;
            ViewBag.Members = fr.team_Members;

            var docu = await _context.REMC_GeneratedForms
                .Where(d => d.fra_Id == fr.fra_Id && d.FileName.Contains("Progress-Report"))
                .ToListAsync();
            return View(docu);
        }

        public async Task<IActionResult> Download(string id)
        {
            var document = await _context.REMC_GeneratedForms.FindAsync(id);

            if (document == null)
            {
                return NotFound();
            }
            return File(document.FileContent, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", document.FileName);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitProgressReport(IFormFile file, string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var fr = await _context.REMC_FundedResearches.FindAsync(id);
            if(fr == null)
            {
                return NotFound();
            }

            if (file == null || file.Length == 0)
            {
                return NotFound("There is no uploaded file. Please upload the file and try to submit again.");
            }

            var existingReports = await _context.REMC_ProgressReports
                .Where(pr => pr.fr_Id == id)
                .ToListAsync();

            int reportNum = existingReports.Count + 1;
            string docuType = $"Progress Report No.{reportNum}";

            byte[] pdfData;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                pdfData = ms.ToArray();
                var progReport = new ProgressReport
                {
                    pr_Id = Guid.NewGuid().ToString(),
                    file_Name = file.FileName,
                    file_Type = Path.GetExtension(file.FileName),
                    data = pdfData,
                    file_Status = "Pending",
                    document_Type = "Progress Report",
                    file_Feedback = null,
                    file_Uploaded = DateTime.Now,
                    fr_Id = fr.fr_Id
                };
                _context.REMC_ProgressReports.Add(progReport);
                fr.status = $"Submitted {docuType}";
                fr.reminded_ThreeDaysBefore = false;
                fr.reminded_OneDayBefore = false;
                fr.reminded_Today = false;
                fr.reminded_OneDayOverdue = false;
                fr.reminded_ThreeDaysOverdue = false;
                fr.reminded_SevenDaysOverdue = false;
            }
            await _actionLogger.LogActionAsync(fr.team_Leader, fr.fr_Type, 
                fr.research_Title + $" already uploaded the {docuType}.", true, true, false, fr.fra_Id);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProgressReportStatus", new { id = id });
        }

        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> ProgressReportStatus(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var fr = await _context.REMC_FundedResearches.FindAsync(id);
            if(fr == null)
            {
                return NotFound();
            }

            ViewBag.Id = fr.fr_Id;
            ViewBag.Research = fr.research_Title;
            ViewBag.Lead = fr.team_Leader;
            ViewBag.Members = fr.team_Members;
            ViewBag.Field = fr.field_of_Study;

            var progReport = await _context.REMC_ProgressReports
                    .Where(pr => pr.fr_Id == id)
                    .OrderBy(pr => pr.file_Uploaded)
                    .ToListAsync();

            return View(progReport);
        }

        public async Task<IActionResult> PreviewFile(string id)
        {
            var progReport = await _context.REMC_ProgressReports.FindAsync(id);
            if (progReport == null)
            {
                return NotFound();
            }

            if (progReport.file_Type == ".pdf")
            {
                return File(progReport.data, "application/pdf");
            }
            else
            {
                var contentType = GetContentType(progReport.file_Type);
                return File(progReport.data, contentType, progReport.file_Name);
            }
        }

        private string GetContentType(string fileType)
        {
            switch (fileType)
            {
                case ".xls":
                    return "application/vnd.ms-excel";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                default:
                    return "application/octet-stream";
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFile(string id, IFormFile newFile)
        {
            var progReport = await _context.REMC_ProgressReports.FindAsync(id);
            if (progReport == null)
            {
                return NotFound();
            }

            if (newFile != null && newFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await newFile.CopyToAsync(memoryStream);
                    progReport.data = memoryStream.ToArray();
                    progReport.file_Name = newFile.FileName;
                    progReport.file_Type = Path.GetExtension(newFile.FileName);
                    progReport.file_Uploaded = DateTime.Now;
                    progReport.file_Status = "Pending";
                    progReport.file_Feedback = null;
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ProgressReportStatus", new { id = progReport.fr_Id });
        }

        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> UploadTerminalReport(string id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var fr = await _context.REMC_FundedResearches.FindAsync(id);
            if(fr == null)
            {
                return NotFound();
            }

            ViewBag.Id = fr.fr_Id;
            ViewBag.Research = fr.research_Title;
            ViewBag.Lead = fr.team_Leader;
            ViewBag.Members = fr.team_Members;

            var docu = await _context.REMC_GeneratedForms
                .Where(d => d.fra_Id == fr.fra_Id && d.FileName.Contains("Terminal-Report"))
                .ToListAsync();

            return View(docu);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitTerminalReport(IFormFile file, string id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var fr = await _context.REMC_FundedResearches.FindAsync(id);
            if(fr == null)
            {
                return NotFound();
            }

            if(file == null || file.Length == 0)
            {
                return NotFound("There is no upload file. Please upload the file and try to submit again.");
            }

            byte[] pdfData;
            using(var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                pdfData = ms.ToArray();
                var progressReport = new ProgressReport
                {
                    pr_Id = Guid.NewGuid().ToString(),
                    file_Name = file.FileName,
                    file_Type = Path.GetExtension(file.FileName),
                    data = pdfData,
                    file_Status = "Pending",
                    document_Type = "Terminal Report",
                    file_Feedback = null,
                    file_Uploaded = DateTime.Now,
                    fr_Id = fr.fr_Id
                };
                _context.REMC_ProgressReports.Add(progressReport);
                fr.status = $"Submitted Terminal Report";
            }

            await _actionLogger.LogActionAsync(fr.team_Leader, fr.fr_Type,
                fr.research_Title + " already uploaded the Terminal Report.", true, true, false, fr.fra_Id);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProgressReportStatus", new { id = id });
        }

        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> UploadLiquidationReport(string id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var fr = await _context.REMC_FundedResearches.FindAsync(id);
            if(fr == null)
            {
                return NotFound();
            }

            ViewBag.Id = fr.fr_Id;
            ViewBag.Research = fr.research_Title;
            ViewBag.Lead = fr.team_Leader;
            ViewBag.Members = fr.team_Members;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitLiquidationReport(IFormFile file, string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fr = await _context.REMC_FundedResearches.FindAsync(id);
            if (fr == null)
            {
                return NotFound();
            }

            if (file == null || file.Length == 0)
            {
                return NotFound("There is no upload file. Please upload the file and try to submit again.");
            }

            byte[] pdfData;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                pdfData = ms.ToArray();
                var progressReport = new ProgressReport
                {
                    pr_Id = Guid.NewGuid().ToString(),
                    file_Name = file.FileName,
                    file_Type = Path.GetExtension(file.FileName),
                    data = pdfData,
                    file_Status = "Pending",
                    document_Type = "Liquidation Report",
                    file_Feedback = null,
                    file_Uploaded = DateTime.Now,
                    fr_Id = fr.fr_Id
                };
                _context.REMC_ProgressReports.Add(progressReport);
                fr.status = $"Submitted Liquidation Report";
            }

            await _actionLogger.LogActionAsync(fr.team_Leader, fr.fr_Type,
                fr.research_Title + " already uploaded the Liquidation Report.", true, true, false, fr.fra_Id);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProgressReportStatus", new { id = id });
        }

        public async Task<IActionResult> DownloadCC(string id)
        {
            var file = await _context.REMC_ProgressReports.FirstOrDefaultAsync(f => f.fr_Id == id && f.document_Type == "Certificate of Completion");
            if (file == null)
            {
                return NotFound();
            }
            return File(file.data, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", file.file_Name);
        }

        [HttpPost]
        public async Task<IActionResult> NewApplication(string frId)
        {
            var fr = await _context.REMC_FundedResearches.FindAsync(frId);
            if(fr == null)
            {
                return NotFound();
            }

            fr.isArchive = true;
            var evaluator = await _context.REMC_Evaluator.FirstOrDefaultAsync(e => e.UserId == fr.UserId);
            if(evaluator != null)
            {
                if(evaluator.field_of_Interest == null)
                {
                    evaluator.field_of_Interest = new List<string>();
                }

                if(!string.IsNullOrEmpty(fr.field_of_Study) && !evaluator.field_of_Interest.Contains(fr.field_of_Study))
                {
                    evaluator.field_of_Interest.Add(fr.field_of_Study);
                    _context.REMC_Evaluator.Update(evaluator);
                }
            }

            var genForms = await _context.REMC_GeneratedForms.Where(f => f.fra_Id == fr.fra_Id).ToListAsync();
            foreach(var form in genForms)
            {
                _context.REMC_GeneratedForms.Remove(form);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Forms", "Home");
        }
    }
}
