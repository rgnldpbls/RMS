using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rscSys_final.Data;
using rscSys_final.Models;
using System.Security.Claims;
using Microsoft.Extensions.Hosting;
using Xceed.Words.NET;
using Azure;
using OfficeOpenXml;
using System.Text;
using iText.Kernel.Pdf;
using iText.Layout.Properties;
using iText.Layout;
using iText.Layout.Element;
using ResearchManagementSystem.Models;

namespace rscSys_final.Controllers
{
    [Area("RscSys")]
    [Authorize(Roles = "RSC Evaluator")]
    public class RSCEvaluatorController : Controller
    {
        private readonly rscSysfinalDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RSCResearcherController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public RSCEvaluatorController(rscSysfinalDbContext context, UserManager<ApplicationUser> userManager, ILogger<RSCResearcherController> logger, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _hostingEnvironment = environment;
        }

        public async Task<IActionResult> EvaluateDocuments(int id)
        {
            // Fetch the evaluator assignment by AssignmentId
            var evaluatorAssignment = await _context.EvaluatorAssignments
                .AsNoTracking()
                .Include(e => e.Request) // Include the Request to get application details
                .Include(e => e.Request.Requirements)
                .FirstOrDefaultAsync(e => e.AssignmentId == id);

            // Check if the evaluator assignment was found
            if (evaluatorAssignment == null)
            {
                return NotFound("Evaluator assignment not found.");
            }

            // Try to fetch the subcategory based on the application type in the request
            var subCategory = await _context.ApplicationSubCategories
                .AsNoTracking()
                .Include(sc => sc.ApplicationType) // Include the associated ApplicationType
                .FirstOrDefaultAsync(sc => sc.CategoryName == evaluatorAssignment.Request.ApplicationType);

            // Determine the application type name
            string applicationTypeName;
            if (subCategory != null)
            {
                // If subcategory is found, use its associated application type
                applicationTypeName = subCategory.ApplicationType.ApplicationTypeName;
            }
            else
            {
                // Fallback: Use the application type from the request if subcategory is not found
                applicationTypeName = evaluatorAssignment.Request.ApplicationType;
            }

            // Fetch the related evaluation forms based on the resolved ApplicationTypeName
            var evaluationForms = await _context.EvaluationForms
                .Include(e => e.Criteria) // Include criteria for each form
                .Where(form => form.Title == applicationTypeName) // Match Title with ApplicationTypeName
                .ToListAsync();

            // Create the ViewModel if necessary
            var viewModel = new EvaluationViewModel
            {
                EvaluatorAssignmentId = evaluatorAssignment.AssignmentId,
                DtsNo = evaluatorAssignment.Request.DtsNo,
                ApplicantName = $"{evaluatorAssignment.Request.Name}",
                ApplicationType = evaluatorAssignment.Request.ApplicationType,
                FieldOfStudy = evaluatorAssignment.Request.FieldOfStudy,
                FiledDate = evaluatorAssignment.AssignedDate,
                Status = evaluatorAssignment.EvaluationStatus,
                SubmittedDocuments = evaluatorAssignment.Request.Requirements.ToList(), // Assuming you have a Requirements collection
                EvaluationForms = evaluationForms
            };

            // Return the view with the populated ViewModel
            return View(viewModel);
        }

        public async Task<IActionResult> Applications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var toBeEvaluated = await _context.EvaluatorAssignments
                .Include(ea => ea.Request)
                .Where(ea => ea.EvaluatorId == userId &&
                             (ea.EvaluationStatus == "Accepted" || ea.EvaluationStatus == "Under Evaluation"))
                .ToListAsync();

            var evaluated = await _context.EvaluatorAssignments
                .Include(ea => ea.Request)
                .Where(ea => ea.EvaluatorId == userId &&
                             ea.EvaluationStatus == "Completed")
                .ToListAsync();

            var viewModel = new EvaluationListViewModel
            {
                ToBeEvaluated = toBeEvaluated,
                Evaluated = evaluated
            };

            return View(viewModel);

        }

        public async Task<IActionResult> Index()
        {
            var evaluatorId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in evaluator's ID

            // Fetch evaluator assignments with related request data
            var evaluatorAssignments = _context.EvaluatorAssignments
                .Where(ea => ea.EvaluatorId == evaluatorId &&
                             (ea.EvaluationStatus == "Accepted" || ea.EvaluationStatus == "Under Evaluation" || ea.EvaluationStatus == "Completed"))
                .Include(ea => ea.Request) // Include the related Request data
                .ToList();

            // Count the number of pending and completed assignments
            int pendingCount = evaluatorAssignments.Count(ea => ea.EvaluationStatus == "Under Evaluation" || ea.EvaluationStatus == "Accepted");
            int completedCount = evaluatorAssignments.Count(ea => ea.EvaluationStatus == "Completed");

            var memorandums = _context.Memorandums.ToList();

            // Prepare the view model
            var viewModel = new EvaluatorDashboardViewModel
            {
                PendingCount = pendingCount,
                CompletedCount = completedCount,
                EvaluatorAssignments = evaluatorAssignments,
                Memorandums = memorandums
            };

            var user = await _userManager.GetUserAsync(User);
            var haveEvaluator = await _context.Evaluators.AnyAsync(e => e.EvaluatorId == user.Id);
            if (!haveEvaluator)
            {
                var evals = new Evaluator
                {
                    EvaluatorId = user.Id,
                    Name = $"{user.FirstName} {user.LastName}",
                    Specialization = "Computer Science and Information System Technology"
                };

                _context.Evaluators.Add(evals);
                await _context.SaveChangesAsync();

                return View(viewModel);
            }

            return View(viewModel);
        }
        public IActionResult Notifications()
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch notifications for the current user
            var notifications = _context.Notifications
                .Where(n => n.UserId == userId && n.Role == "Evaluator" || n.UserId == userId && !n.NotificationStatus && n.Role == "All")
                .OrderByDescending(n => n.NotificationCreationDate)
                .ToList();

            // Pass the notifications to the view
            return View(notifications);
        }

        [HttpPost]
        public IActionResult UpdateNotificationStatus(int notificationId)
        {
            var notification = _context.Notifications.Find(notificationId);

            if (notification != null)
            {
                notification.NotificationStatus = true; // Set the status to true
                _context.SaveChanges(); // Save the changes to the database
            }

            return Json(new { success = true }); // Return a JSON response
        }

        [HttpPost]
        public async Task<IActionResult> GetNotificationDetails(int notificationId)
        {
            var notification = await _context.Notifications
                .Include(n => n.EvaluatorAssignment) // Include if needed
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

            if (notification == null)
            {
                return Json(new { success = false, message = "Notification not found." });
            }

            return Json(new
            {
                success = true,
                details = notification.NotificationMessage,
                assignmentId = notification.EvaluatorAssignmentId // Adjust if needed
            });
        }

        [HttpPost]
        public async Task<IActionResult> AcceptEvaluation(int assignmentId)
        {
            var assignment = await _context.EvaluatorAssignments.FindAsync(assignmentId);

            if (assignment == null)
            {
                return Json(new { success = false, message = "Assignment not found." });
            }

            // Update the evaluation status
            assignment.EvaluationStatus = "Accepted";

            // Remove the associated notification
            var notification = await _context.Notifications
                                              .FirstOrDefaultAsync(n => n.EvaluatorAssignmentId == assignmentId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RejectEvaluation(int assignmentId)
        {
            var assignment = await _context.EvaluatorAssignments.FindAsync(assignmentId);

            if (assignment == null)
            {
                return Json(new { success = false, message = "Assignment not found." });
            }

            // Update the evaluation status
            assignment.EvaluationStatus = "Rejected";

            // Remove the associated notification
            var notification = await _context.Notifications
                                              .FirstOrDefaultAsync(n => n.EvaluatorAssignmentId == assignmentId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
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

        public async Task<IActionResult> GetDocument(int requirementId)
        {
            var requirement = _context.Requirements.FirstOrDefault(r => r.RequirementId == requirementId);

            if (requirement == null)
            {
                return NotFound();
            }

            Response.Headers.Add("Content-Disposition", "inline; filename=\"" + requirement.FileName + "\"");
            return File(requirement.FileData, requirement.FileType);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitEvaluationResponse(List<EvaluationFormResponse> criteriaResponses, string comments, string evaluatorId, int evaluatorAssignmentId, string userEmail)
        {
            if (criteriaResponses == null || !criteriaResponses.Any())
            {
                return BadRequest("No criteria responses provided.");
            }

            // Validate EvaluatorAssignmentId exists
            var assignment = await _context.EvaluatorAssignments
                .Include(a => a.Evaluators) // Include evaluator if needed
                .FirstOrDefaultAsync(e => e.AssignmentId == evaluatorAssignmentId);

            if (assignment == null)
            {
                return BadRequest("Invalid Evaluator Assignment ID.");
            }

            // Prepare to save the evaluation responses
            foreach (var response in criteriaResponses)
            {
                response.EvaluatorId = evaluatorId; // Set the evaluator ID
                response.EvaluatorAssignmentId = evaluatorAssignmentId; // Link to the specific evaluator assignment
                _context.EvaluationFormResponses.Add(response); // Add the response to the context
            }

            // Save all responses to get the ResponseIds
            await _context.SaveChangesAsync();

            // Save the general comments if provided
            if (!string.IsNullOrEmpty(comments))
            {
                var generalComment = new EvaluationGeneralComment
                {
                    CommentText = comments,
                    CreatedDate = DateTime.Now,
                    EvaluatorAssignmentId = evaluatorAssignmentId // Link the comment to the evaluator assignment
                };

                _context.EvaluationGeneralComments.Add(generalComment); // Add the comment
            }

            await _context.SaveChangesAsync(); // Save all changes to the database

            // Generate the evaluation document
            await GenerateEvaluationDocument(evaluatorAssignmentId);

            // Update the status of the evaluator assignment to "Completed"
            assignment.EvaluationStatus = "Completed"; // Assuming there is a Status property
            await _context.SaveChangesAsync(); // Save the updated status

            // Add notifications for the Chief role and the owner of the file
            var ownerUserId = assignment.Request.UserId;
            var ownerName = $"{assignment.Request.Name}";

            // Find users with the Chief role

            var chiefUsers = await _userManager.GetUsersInRoleAsync("RSC Chief");

            foreach (var chiefUserId in chiefUsers)
            {
                var chiefNotification = new Notifications
                {
                    UserId = chiefUserId.Id,
                    NotificationTitle = "Evaluation Completed",
                    NotificationMessage = $"The evaluation for DTS No. {assignment.Request.DtsNo} has been completed.",
                    Role = "Chief",
                    PerformedBy = ownerName
                };
                _context.Notifications.Add(chiefNotification);
            }

            // Notification for the owner of the file
            var ownerNotification = new Notifications
            {
                UserId = ownerUserId,
                NotificationTitle = "Evaluation Completed",
                NotificationMessage = $"Your application with DTS No. {assignment.Request.DtsNo} has been successfully evaluated.",
                Role = "Researcher",
                PerformedBy = ownerName
            };
            _context.Notifications.Add(ownerNotification);

            // Add an entry to DocumentHistory
            var documentHistory = new DocumentHistory
            {
                RequestId = assignment.Request.RequestId, // Assuming RequestId is the primary key in Request table
                Comments = "Your application has been successfully evaluated.",
                PerformedBy = ownerName
            };
            _context.DocumentHistories.Add(documentHistory);

            await _context.SaveChangesAsync(); // Save notifications

            return RedirectToAction("Applications"); // Redirect to a suitable action/view after submission
        }

        private async Task GenerateEvaluationDocument(int evaluatorAssignmentId)
        {
            var assignment = await _context.EvaluatorAssignments
                .Include(a => a.Evaluators)
                .Include(a => a.Request)
                .FirstOrDefaultAsync(a => a.AssignmentId == evaluatorAssignmentId);

            if (assignment == null)
            {
                throw new Exception("Evaluator Assignment not found.");
            }

            var applicantName = $"{assignment.Request.Name}";
            var evaluatorName = $"{assignment.Evaluators.Name}";
            var currentDate = DateTime.Now.ToString("MMMM dd, yyyy");
            var fileName = $"{assignment.Request.DtsNo}_EvaluationResponse.pdf"; // Set the desired file name

            // Retrieve the template from the database
            var template = await _context.Templates
                .FirstOrDefaultAsync(t => t.TemplateName == "evaluation-form-template");

            if (template == null || template.FileData == null)
            {
                throw new Exception("Template not found or contains no data.");
            }

            using (var templateStream = new MemoryStream(template.FileData))
            {
                using (DocX document = DocX.Load(templateStream))
                {
                    // Replace placeholders
                    document.ReplaceText("{DTS_Number}", assignment.Request.DtsNo);
                    document.ReplaceText("{Application_Type}", assignment.Request.ApplicationType);
                    document.ReplaceText("{Applicant_Name}", applicantName);
                    document.ReplaceText("{Field_of_Study}", assignment.Request.FieldOfStudy);
                    document.ReplaceText("{College_Branch}", assignment.Request.Branch);

                    // Fetch evaluation responses
                    var responses = await _context.EvaluationFormResponses
                        .Include(r => r.Criterion)
                        .Where(r => r.EvaluatorAssignmentId == evaluatorAssignmentId)
                        .ToListAsync();

                    // Find the table and check the second row for the placeholder
                    var table = document.Tables.FirstOrDefault(t => t.Rows.Count > 1 &&
                        t.Rows[1].Cells[0].Paragraphs[0].Text.Contains("{Responses_Table}"));

                    if (table != null)
                    {
                        // Remove the placeholder row
                        table.RemoveRow(1);

                        decimal grandTotalScore = 0;

                        // Insert each response into the table
                        foreach (var response in responses)
                        {
                            var row = table.InsertRow();
                            row.Cells[0].Paragraphs[0].Append($"{response.Criterion.Percentage}%").Font("Century Gothic");
                            row.Cells[1].Paragraphs[0].Append(response.Criterion.Name).Font("Century Gothic");
                            row.Cells[2].Paragraphs[0].Append($"{response.UserPercentage}%").Font("Century Gothic");
                            row.Cells[3].Paragraphs[0].Append(response.Comment).Font("Century Gothic");

                            grandTotalScore += response.UserPercentage; // Calculate grand total
                        }

                        // Add grand total row
                        var totalRow = table.InsertRow();
                        totalRow.Cells[1].Paragraphs[0].Append("Total").Bold().Font("Century Gothic");
                        totalRow.Cells[2].Paragraphs[0].Append($"{grandTotalScore}%").Bold().Font("Century Gothic");
                    }

                    // Fetch general comments
                    var generalComments = await _context.EvaluationGeneralComments
                        .Where(c => c.EvaluatorAssignmentId == evaluatorAssignmentId)
                        .Select(c => c.CommentText)
                        .ToListAsync();

                    // Join comments into a single string
                    string commentsText = string.Join("\n", generalComments);
                    document.ReplaceText("{General_Comments}", commentsText);

                    document.ReplaceText("{Evaluator_Name}", evaluatorName);
                    document.ReplaceText("{Date}", currentDate);

                    // Save the document to a MemoryStream instead of a file
                    using (var memoryStream = new MemoryStream())
                    {
                        document.SaveAs(memoryStream);
                        memoryStream.Position = 0; // Reset stream position to the beginning

                        // Convert DOCX to PDF using Aspose.Words
                        var doc = new Aspose.Words.Document(memoryStream);

                        using (var pdfStream = new MemoryStream())
                        {
                            // Save to PDF format
                            doc.Save(pdfStream, Aspose.Words.SaveFormat.Pdf);

                            // Convert PDF stream to byte array
                            var pdfBytes = pdfStream.ToArray();

                            // Create the EvaluationDocument object with the new model structure
                            var evaluationDocument = new EvaluationDocument
                            {
                                EvaluatorAssignmentId = evaluatorAssignmentId,
                                FileName = fileName, // Set the file name
                                FileData = pdfBytes,  // Store the byte array in the database
                                FileType = "application/pdf", // Set the file type
                                CreatedAt = DateTime.Now
                            };

                            // Add the new EvaluationDocument entry to the context
                            _context.EvaluationDocuments.Add(evaluationDocument);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
        }

    }
}
