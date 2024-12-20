﻿using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemcSys.Data;
using RemcSys.Models;
using ResearchManagementSystem.Models;
using Xceed.Words.NET;

namespace RemcSys.Controllers
{
    [Area("RemcSys")]
    public class EvaluatorController : Controller
    {
        private readonly RemcDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ActionLoggerService _actionLogger;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public EvaluatorController(RemcDBContext context, UserManager<ApplicationUser> userManager,
            ActionLoggerService actionLogger, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _actionLogger = actionLogger;
            _hostingEnvironment = hostingEnvironment;
        }

        [Authorize(Roles ="REMC Evaluator")]
        public async Task<IActionResult> EvaluatorDashboard() // Dashboard of the Evaluator
        {
            if (_context.REMC_Settings.First().isMaintenance)
            {
                return RedirectToAction("UnderMaintenance", "Home");
            }

            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound("User not found!");
            }
            var evaluator = await _context.REMC_Evaluator.FirstOrDefaultAsync(f => f.UserId == user.Id);
            if(evaluator == null)
            {
                return NotFound("Evaluator not found!");
            }

            if(evaluator.field_of_Interest == null)
            {
                return RedirectToAction("EvaluatorSpecialization", "Evaluator");
            }

            ViewBag.Pending = await _context.REMC_Evaluations.Where(e => e.evaluation_Status == "Pending" && e.evaluator_Id == evaluator.evaluator_Id).CountAsync();
            ViewBag.Missed = await _context.REMC_Evaluations.Where(e => e.evaluation_Status == "Missed" && e.evaluator_Id == evaluator.evaluator_Id).CountAsync();
            ViewBag.Evaluated = await _context.REMC_Evaluations.Where(e => (e.evaluation_Status == "Approved" || e.evaluation_Status == "Rejected")
                && e.evaluator_Id == evaluator.evaluator_Id).CountAsync();

            var pendingEvals = await _context.REMC_Evaluations
                .Include(e => e.fundedResearchApplication)
                .Where(e => e.evaluation_Status == "Pending" && e.evaluator_Id == evaluator.evaluator_Id)
                .OrderBy(e => e.evaluation_Deadline)
                .ToListAsync();
            
            return View(pendingEvals);
        }

        [Authorize(Roles = "REMC Evaluator")]
        public async Task<IActionResult> EvaluatorNotif() // Notification of the Evaluator
        {
            if (_context.REMC_Settings.First().isMaintenance)
            {
                return RedirectToAction("UnderMaintenance", "Home");
            }

            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound("User not found!");
            }
            var logs = await _context.REMC_ActionLogs
                .Where(f => f.Name == $"{user.FirstName} {user.MiddleName} {user.LastName}" && f.isEvaluator == true)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
            return View(logs);
        }

        public async Task CheckMissedEvaluations() // Check if there's a Missed Evaluations
        {
            var today = DateTime.Today;

            var missedEvaluations = await _context.REMC_Evaluations
                .Where(e => e.evaluation_Status == "Pending" && e.evaluation_Deadline <= today)
                .ToListAsync();

            foreach (var evaluation in missedEvaluations)
            {
                evaluation.evaluation_Status = "Missed";
            }

            await _context.SaveChangesAsync();
        }

        [Authorize(Roles = "REMC Evaluator")]
        public async Task<IActionResult> EvaluatorPending() // List of Pending Evaluation
        {
            await CheckMissedEvaluations();

            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound("User not found!");
            }
            var evaluator = _context.REMC_Evaluator.Where(e => e.UserId == user.Id).FirstOrDefault();

            if(evaluator != null)
            {
                var pendingEvaluations = await _context.REMC_Evaluations
                    .Where(e => e.evaluator_Id == evaluator.evaluator_Id && e.evaluation_Status == "Pending")
                    .Join(_context.REMC_FundedResearchApplication,
                        evaluation => evaluation.fra_Id,
                        researchApp => researchApp.fra_Id,
                        (evaluation, researchApp) => new ViewEvaluationVM
                        {
                            dts_No = researchApp.dts_No,
                            research_Title = researchApp.research_Title,
                            field_of_Study = researchApp.field_of_Study,
                            application_Status =  researchApp.application_Status,
                            evaluation_deadline =  evaluation.evaluation_Deadline,
                            fra_Id =  researchApp.fra_Id
                        })
                    .OrderBy(e => e.evaluation_deadline)
                    .ToListAsync();

                return View(pendingEvaluations);
            }
            return View(new List<ViewEvaluationVM>());
        }

        [Authorize(Roles = "REMC Evaluator")]
        public async Task<IActionResult> EvaluatorMissed() // List of Missed Evaluation
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound("User not found!");
            }
            var evaluator = _context.REMC_Evaluator.Where(e => e.UserId == user.Id).FirstOrDefault();

            if (evaluator != null)
            {
                var missedEvaluations = await _context.REMC_Evaluations
                    .Where(e => e.evaluator_Id == evaluator.evaluator_Id && e.evaluation_Status == "Missed")
                    .Join(_context.REMC_FundedResearchApplication,
                        evaluation => evaluation.fra_Id,
                        researchApp => researchApp.fra_Id,
                        (evaluation, researchApp) => new ViewEvaluationVM
                        {
                            dts_No = researchApp.dts_No,
                            research_Title = researchApp.research_Title,
                            field_of_Study = researchApp.field_of_Study,
                            application_Status = researchApp.application_Status,
                            evaluation_deadline = evaluation.evaluation_Deadline,
                            fra_Id = researchApp.fra_Id
                        })
                    .OrderBy(e => e.evaluation_deadline)
                    .ToListAsync();

                return View(missedEvaluations);
            }
            return View(new List<ViewEvaluationVM>());
        }

        [Authorize(Roles = "REMC Evaluator")]
        public async Task<IActionResult> EvaluatorEvaluated() // List of Done Evaluation
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound("User not found!");
            }
            var evaluator = _context.REMC_Evaluator.Where(e => e.UserId == user.Id).FirstOrDefault();

            if (evaluator != null)
            {
                var doneEvaluations = await _context.REMC_Evaluations
                    .Where(e => e.evaluator_Id == evaluator.evaluator_Id && (e.evaluation_Status == "Approved" || e.evaluation_Status == "Rejected"))
                    .Join(_context.REMC_FundedResearchApplication,
                        evaluation => evaluation.fra_Id,
                        researchApp => researchApp.fra_Id,
                        (evaluation, researchApp) => new ViewEvaluationVM
                        {
                            dts_No = researchApp.dts_No,
                            research_Title = researchApp.research_Title,
                            field_of_Study = researchApp.field_of_Study,
                            application_Status = evaluation.evaluation_Status,
                            evaluation_deadline = evaluation.evaluation_Date,
                            fra_Id = researchApp.fra_Id
                        })
                    .OrderBy(e => e.evaluation_deadline)
                    .ToListAsync();

                return View(doneEvaluations);
            }
            return View(new List<ViewEvaluationVM>());
        }

        [Authorize(Roles = "REMC Evaluator")]
        public IActionResult EvaluationForm(string id) // Evaluation Form
        {
            var guidelines = _context.REMC_Guidelines.Where(g => g.document_Type == "UFREvalsForm" && g.file_Type == ".pdf")
                .OrderBy(g => g.file_Name).ToList();
            ViewBag.exec = guidelines;

            var criteria = _context.REMC_Criterias.Include(c => c.subCategory).OrderBy(c => c.Id).ToList();
            // File Requirement
            var fileRequirement = _context.REMC_FileRequirement.Where(f => f.fra_Id == id && f.document_Type == "Forms")
                .OrderBy(f => f.file_Name)
                .ToList();
            ViewBag.Id = id;

            var model = new Tuple<IEnumerable<FileRequirement>, IEnumerable<Criteria>>(fileRequirement, criteria);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitEvaluation(double aqScore, string aqComment, double reScore, string reComment, double riScore, string riComment,
            double lcScore, string lcComment, double rdScore, string rdComment, double ffScore, string ffComment, string genComment, string fraId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found!");
            }
            var evaluator = await _context.REMC_Evaluator.Where(e => e.UserId == user.Id).FirstOrDefaultAsync();
            if(evaluator == null)
            {
                return NotFound("Evaluator not found!");
            }
            var fra = await _context.REMC_FundedResearchApplication.Where(f => f.fra_Id == fraId).FirstOrDefaultAsync();
            if(fra == null)
            {
                return NotFound("Funded Research Application not found!");
            }

            double tot1 = aqScore + reScore;
            double d1 = tot1 / 20;
            double p1 = d1 * 10;

            double tot2 = riScore + lcScore + rdScore;
            double d2 = tot2 / 30;
            double p2 = d2 * 60;

            double d3 = ffScore / 10;
            double p3 = d3 * 30;

            double g1 = p1 + p2 + p3;

            var templates = await _context.REMC_Guidelines
                .Where(g => g.document_Type == "UFREvalsForm" && g.file_Type == ".docx")
                .ToListAsync();

            string filledFolder = Path.Combine(_hostingEnvironment.WebRootPath, "content", "evaluation_forms");
            Directory.CreateDirectory(filledFolder);

            foreach (var template in templates)
            {
                using (var templateStream = new MemoryStream(template.data))
                {
                    string filledDocumentPath = Path.Combine(filledFolder, $"{evaluator.evaluator_Name}_{template.file_Name}");

                    var teamMembers = fra.team_Members.Contains("N/A") ?
                        string.Empty : string.Join(Environment.NewLine, fra.team_Members);

                    var college = fra.college == null ? string.Empty : fra.college;

                    var branch = fra.branch == null ? string.Empty : fra.branch;

                    using (DocX doc = DocX.Load(templateStream))
                    {
                        doc.ReplaceText("{{Title}}", fra.research_Title);
                        doc.ReplaceText("{{Lead}}", fra.applicant_Name);
                        doc.ReplaceText("{{Staff}}", teamMembers);
                        doc.ReplaceText("{{College}}", college);
                        doc.ReplaceText("{{Branch}}", branch);
                        doc.ReplaceText("{{AQComment}}", aqComment);
                        doc.ReplaceText("{{REComment}}", reComment);
                        doc.ReplaceText("{{RIComment}}", riComment);
                        doc.ReplaceText("{{LCComment}}", lcComment);
                        doc.ReplaceText("{{RDComment}}", rdComment);
                        doc.ReplaceText("{{FFComment}}", ffComment);
                        doc.ReplaceText("{{GenComment}}", genComment);
                        doc.ReplaceText("{{EvaluatorName}}", evaluator.evaluator_Name.ToUpper());
                        doc.ReplaceText("{{Date}}", DateTime.Now.ToString("MMMM d, yyyy"));
                        doc.ReplaceText("{{S1}}", aqScore.ToString());
                        doc.ReplaceText("{{S2}}", reScore.ToString());
                        doc.ReplaceText("{{T1}}", tot1.ToString());
                        doc.ReplaceText("{{P1}}", p1.ToString());
                        doc.ReplaceText("{{S3}}", riScore.ToString());
                        doc.ReplaceText("{{S4}}", lcScore.ToString());
                        doc.ReplaceText("{{S5}}", rdScore.ToString());
                        doc.ReplaceText("{{T2}}", tot2.ToString());
                        doc.ReplaceText("{{P2}}", p2.ToString());
                        doc.ReplaceText("{{S6}}", ffScore.ToString());
                        doc.ReplaceText("{{P3}}", p3.ToString());
                        doc.ReplaceText("{{G1}}", g1.ToString());

                        doc.SaveAs(filledDocumentPath);
                    }

                    byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filledDocumentPath);

                    var fileReq = new FileRequirement
                    {
                        fr_Id = Guid.NewGuid().ToString(),
                        file_Name = $"{evaluator.evaluator_Name}_{template.file_Name}",
                        file_Type = ".docx",
                        data = fileBytes,
                        file_Status = "Evaluated",
                        document_Type = "EvaluationForms",
                        file_Feedback = null,
                        file_Uploaded = DateTime.Now,
                        fra_Id = fraId
                    };

                    _context.REMC_FileRequirement.Add(fileReq);
                }
            }

            var evals = _context.REMC_Evaluations.Where(e => e.fra_Id == fraId && e.evaluator_Id == evaluator.evaluator_Id).FirstOrDefault();
            if(evals == null)
            {
                return NotFound("Evaluations not found!");
            }
            evals.evaluation_Grade = g1;
            evals.evaluation_Status = g1 >= 70 ? "Approved" : "Rejected";
            evals.evaluation_Date = DateTime.Now;

            await _context.SaveChangesAsync();

            Directory.Delete(filledFolder, true);

            await _actionLogger.LogActionAsync(fra.applicant_Name, fra.fra_Type,
                $"{evaluator.evaluator_Name} has evaluated {fra.research_Title}.",
                false, true, false, fra.fra_Id);

            return RedirectToAction("EvaluatorEvaluated", "Evaluator");
        }

        [Authorize(Roles = "REMC Evaluator")]
        public async Task<IActionResult> GenerateEvalsForm(string id) // List of Generated Evaluation Form
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound("User not found!");
            }
            var evaluator = await _context.REMC_Evaluator.Where(e => e.UserId == user.Id).FirstOrDefaultAsync();
            if(evaluator == null)
            {
                return NotFound("Evaluator not found!");
            }
            var evaluations = await _context.REMC_Evaluations.Where(e => e.fra_Id == id && e.evaluator_Id == evaluator.evaluator_Id)
                .FirstOrDefaultAsync();
            if(evaluations == null)
            {
                return NotFound("Evaluation not found!");
            }
            var fr = await _context.REMC_FileRequirement.Where(f => f.fra_Id == evaluations.fra_Id && f.document_Type == "EvaluationForms"
                && f.file_Name.Contains(evaluator.evaluator_Name))
                .OrderBy(f => f.file_Name)
                .ToListAsync();
            ViewBag.Grade = evaluations.evaluation_Grade;
            ViewBag.Remarks = evaluations.evaluation_Status;
            return View(fr);
        }

        public async Task<IActionResult> Download(string id) // Download Non-PDF file
        {
            var document = await _context.REMC_FileRequirement.FindAsync(id);

            if (document == null)
            {
                return NotFound("File not found!");
            }
            return File(document.data, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", document.file_Name);
        }

        public IActionResult PreviewFile(string id) // Preview PDF file
        {
            var file = _context.REMC_FileRequirement.FirstOrDefault(f => f.fr_Id == id);
            if(file != null)
            {
                return File(file.data, "application/pdf");
            }

            var guidelines = _context.REMC_Guidelines.FirstOrDefault(f => f.Id == id);
            if (guidelines != null)
            {
                if (guidelines.file_Type == ".pdf")
                {
                    return File(guidelines.data, "application/pdf");
                }
            }

            return BadRequest("Only PDF files can be previewed.");
        }

        [HttpPost]
        public async Task<IActionResult> AddEvent(string eventTitle, DateTime startDate, DateTime endDate) // Adding Event
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound("User not found!");
            }
            if (ModelState.IsValid)
            {
                var addEvent = new CalendarEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = eventTitle,
                    Start = startDate,
                    End = endDate.AddDays(1),
                    Visibility = "JustYou",
                    UserId = user.Id
                };

                _context.REMC_CalendarEvents.Add(addEvent);
                await _context.SaveChangesAsync();

                return RedirectToAction("EvaluatorDashboard");
            }
            return RedirectToAction("EvaluatorDashboard");
        }

        [HttpGet]
        public async Task<IActionResult> GetUserEvents() // Getting Event
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound("User not found!");
            }
            var events = _context.REMC_CalendarEvents
                .Where(e => e.Visibility == "Broadcasted" || (e.Visibility == "JustYou" && e.UserId == user.Id))
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Start,
                    e.End,
                    e.Visibility,
                })
                .ToList();

            return Json(events);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEvent(string id) // Delete an event
        {
            var events = await _context.REMC_CalendarEvents.FindAsync(id);
            if (events != null && events.Visibility == "JustYou")
            {
                _context.REMC_CalendarEvents.Remove(events);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public async Task<IActionResult> EvaluatorSpecialization()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound("User not found!");
            }
            var evaluator = await _context.REMC_Evaluator.FirstOrDefaultAsync(f => f.UserId == user.Id);
            if (evaluator == null)
            {
                return NotFound("Evaluator not found!");
            }
            return View(evaluator);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSpecialization([FromForm] List<string> field_of_Interest)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("User not found!");
                }
                var evaluator = await _context.REMC_Evaluator.FirstOrDefaultAsync(e => e.UserId == user.Id);
                if (evaluator == null)
                {
                    return NotFound("Evaluator not found!");
                }
                evaluator.field_of_Interest = field_of_Interest;
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch
            {
                return Json(new {success = false});
            }
        }
    }
}
