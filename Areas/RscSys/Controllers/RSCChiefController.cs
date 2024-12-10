using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Org.BouncyCastle.Asn1.Ocsp;
using rscSys_final.Data;
using rscSys_final.Models;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Geom;
using iText.Kernel.Colors;
using System.IO;
using iText.Layout.Properties;
using iText.StyledXmlParser.Jsoup.Select;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Xceed.Words.NET;
using Microsoft.AspNetCore.Http.HttpResults;
using MimeKit;
using MailKit.Net.Smtp;
using ContentType = MimeKit.ContentType;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using Path = System.IO.Path;
using OfficeOpenXml.Style;
using DocumentFormat.OpenXml.Spreadsheet;
using ResearchManagementSystem.Models;
using Table = iText.Layout.Element.Table;
using Tuple = System.Tuple;
using CrdlSys.Data;
using MailKit.Search;


namespace rscSys_final.Controllers
{
    [Area("RscSys")]
    [Authorize(Roles = "RSC Chief,Director")]
    public class RSCChiefController : Controller
    {
        private readonly rscSysfinalDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly ILogger<RSCResearcherController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly CrdlDbContext _crdlDbContext;
        public RSCChiefController(rscSysfinalDbContext context, UserManager<ApplicationUser> userManager, /*ILogger<RSCResearcherController> logger,*/ IWebHostEnvironment environment, CrdlDbContext crdlDbContext)
        {
            _context = context;
            _userManager = userManager;
            //_logger = logger;
            _environment = environment;
            _crdlDbContext = crdlDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> UploadTemplate(string TemplateName, IFormFile FileUpload)
        {
            if (FileUpload != null && FileUpload.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await FileUpload.CopyToAsync(memoryStream);

                    var template = new Template
                    {
                        TemplateId = Guid.NewGuid().ToString(),
                        TemplateName = TemplateName,
                        FileData = memoryStream.ToArray(),
                        FileType = FileUpload.ContentType,
                        FileUploaded = DateTime.Now
                    };

                    _context.RSC_Templates.Add(template);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("EvaluationForms");
        }

        public IActionResult Endorse(int requestId)
        {
            // Fetch the request data from the database
            var request = _context.RSC_Requests
                .FirstOrDefault(r => r.RequestId == requestId);

            if (request == null)
            {
                return NotFound();
            }

            // Check if a final document already exists for this request
            var existingFinalDocuments = _context.RSC_FinalDocuments
                .Where(d => d.RequestId == requestId)
                .ToList();

            // If a final document already exists, skip the generation process
            if (existingFinalDocuments.Any())
            {
                // Pass the existing final documents to the view
                return View("Endorse", new EndorsementViewModel { Request = request, FinalDocuments = existingFinalDocuments });
            }

            // Fetch the template from the database
            var template = _context.RSC_Templates.FirstOrDefault(t => t.TemplateName == "endorse-template");
            if (template == null)
            {
                return NotFound("Template not found in the database.");
            }

            // Load the .docx template from the FileData byte array
            using (var memoryStream = new MemoryStream(template.FileData))
            using (var doc = DocX.Load(memoryStream))
            {
                string formattedDate = DateTime.Now.ToString("MM-dd-yyyy");

                // Replace placeholders with request details
                doc.ReplaceText("{date}", formattedDate);
                doc.ReplaceText("{fullname}", request.Name);
                doc.ReplaceText("{college}", request.College ?? "N/A");
                doc.ReplaceText("{name}", request.Name);
                doc.ReplaceText("{title}", request.ResearchTitle);
                doc.ReplaceText("{dtsno}", request.DtsNo);
                /* doc.ReplaceText("{ApplicationType}", request.ApplicationType);
                   doc.ReplaceText("{FieldOfStudy}", request.FieldOfStudy); */

                // Save the modified document to a new MemoryStream
                using (var outputStream = new MemoryStream())
                {
                    doc.SaveAs(outputStream);

                    // Convert the MemoryStream to a byte array
                    var fileData = outputStream.ToArray();

                    // Save the document directly to the database
                    var finalDocument = new FinalDocument
                    {
                        FinalDocuName = $"Endorsement_Letter_{request.DtsNo}.docx",
                        FileData = fileData,
                        FileType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                        RequestId = request.RequestId
                    };

                    _context.RSC_FinalDocuments.Add(finalDocument);
                    _context.SaveChanges();
                }
            }

            // Redirect to the same action to refresh the page with updated data
            return RedirectToAction("Endorse", new { requestId });
        }



        public IActionResult DownloadFinalDocument(int id)
        {
            var document = _context.RSC_FinalDocuments.FirstOrDefault(d => d.FinalDocuID == id);

            if (document == null)
            {
                return NotFound();
            }

            return File(document.FileData, document.FileType, document.FinalDocuName);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEndorsement(int finaldocuId)
        {
            // Find the requirement record in the database by its ID
            var finaldocu = await _context.RSC_FinalDocuments.FindAsync(finaldocuId);

            if (finaldocuId == null)
            {
                return Json(new { success = false, message = "File not found" }); // If file doesn't exist in the database
            }

            // Remove the file record from the database
            _context.RSC_FinalDocuments.Remove(finaldocu);
            await _context.SaveChangesAsync();

            // Redirect back or return success
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UploadFinalDocu(int requestId, IFormFile endorsementLetter)
        {
            if (endorsementLetter == null || endorsementLetter.Length == 0)
            {
                return Json(new { success = false, message = "No file uploaded." });
            }

            // Check if there's an existing document for this request
            var existingDocument = _context.RSC_FinalDocuments.FirstOrDefault(d => d.RequestId == requestId);

            if (existingDocument != null)
            {
                // Remove the existing document record from the database
                _context.RSC_FinalDocuments.Remove(existingDocument);
            }

            // Read the uploaded file into a byte array
            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                await endorsementLetter.CopyToAsync(memoryStream);
                fileData = memoryStream.ToArray();
            }

            // Save the new document record in the database
            var newDocument = new FinalDocument
            {
                FinalDocuName = endorsementLetter.FileName,
                FileData = fileData,
                FileType = endorsementLetter.ContentType,
                RequestId = requestId
            };

            _context.RSC_FinalDocuments.Add(newDocument);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "File uploaded successfully." });
        }

        public async Task<IActionResult> SendEndorsementLetter(EndorsementViewModel model, string comments)
        {
            if (model == null || model.Request == null)
            {
                return BadRequest("Invalid request data.");
            }

            var request = await _context.RSC_Requests.FindAsync(model.Request.RequestId);
            if (request == null)
            {
                return NotFound("Request not found.");
            }

            // Update the request status
            request.Status = "Endorsed by RMO";
            _context.RSC_Requests.Update(request);

            // Create a new status history entry for the endorsement
            var statusHistory = new StatusHistory
            {
                RequestId = request.RequestId,
                Status = "Endorsed by RMO",
                Comments = comments
            };

            await _context.RSC_StatusHistories.AddAsync(statusHistory);

            // Create a new notification
            var notification = new Notifications
            {
                UserId = model.Request.UserId,
                NotificationTitle = "Endorsement Letter Sent",
                NotificationMessage = comments,
                NotificationCreationDate = DateTime.Now,
                NotificationStatus = false,
                Role = "Researcher",
                PerformedBy = "Chief"
            };

            await _context.RSC_Notifications.AddAsync(notification);

            // Add a new document history entry indicating the endorsement letter is ready
            var documentHistory = new DocumentHistory
            {
                RequestId = request.RequestId,
                Comments = "The endorsement letter is ready to download",
                PerformedBy = "Chief"
            };

            await _context.RSC_DocumentHistories.AddAsync(documentHistory);

            // Retrieve the FinalDocument from the database
            var finalDocument = await _context.RSC_FinalDocuments
                                .FirstOrDefaultAsync(d => d.RequestId == request.RequestId);

            if (finalDocument != null)
            {
                // Send an email with the document attached
                await SendEmailWithAttachmentAsync(model.Request.UserId, comments, finalDocument);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction("Applications"); // Adjust as needed
        }

        private async Task SendEmailWithAttachmentAsync(string userId, string comments, FinalDocument document)
        {
            // Retrieve user's email and name
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                return; // User not found or email not provided
            }

            // Retrieve the DtsNo from the associated request
            var request = await _context.RSC_Requests.FindAsync(document.RequestId);
            if (request == null)
            {
                return; // Request not found
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("PUP Research Support Center", "rsc.rmo3@gmail.com"));
            message.To.Add(new MailboxAddress($"{user.FirstName} {user.LastName}", user.Email));
            message.Subject = "Endorsement Letter Sent";

            var builder = new BodyBuilder
            {
                TextBody = $"Dear {user.FirstName} {user.LastName},\n\nWe are pleased to inform you that your application request with DTS No. {request.DtsNo} has been approved. Attached, you will find your Endorsement Letter, which formally recognizes the successful approval of your application. Thank you!\n\nYours Truly,\nPUP Research Support Center"
            };

            // Add the document as an attachment
            builder.Attachments.Add(document.FinalDocuName, document.FileData, ContentType.Parse(document.FileType));

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("rsc.rmo3@gmail.com", "ervg ojdk oeom rfyk");

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        // GET: Reports
        public IActionResult Reports()
        {
            var model = new ReportFilterViewModel
            {
                GeneratedReports = _context.RSC_GeneratedReports.ToList() // Retrieve generated reports for the view
            };
            ViewBag.UnreadNotificationsCount = _context.RSC_Notifications
            .Count(n => !n.NotificationStatus && n.Role == "Chief");

            return View(model);
        }

        // Controller Action to Generate Report
        [HttpPost]
        public async Task<IActionResult> GenerateReport(ReportFilterViewModel model, string fileType)
        {
            List<rscSys_final.Models.Request> requests;

            // Fetch requests based on the selected criteria
            if (model.ReportType == "Summary Report" && string.IsNullOrEmpty(model.College) && model.StartDate == null && model.EndDate == null && string.IsNullOrEmpty(model.Branch))
            {
                requests = await _context.RSC_Requests
                    .Where(r => r.Status == "Approved" || r.Status == "Endorsed by RMO")
                    .ToListAsync();
            }
            else
            {
                requests = await _context.RSC_Requests
                 .Where(r => (r.Status == "Approved" || r.Status == "Endorsed by RMO") &&
                             (model.StartDate == null || r.CreatedDate >= model.StartDate) &&
                             (model.EndDate == null || r.CreatedDate <= model.EndDate) &&
                             (string.IsNullOrEmpty(model.College) || r.College == model.College) &&
                             (string.IsNullOrEmpty(model.Branch) || r.Branch == model.Branch) &&
                             (model.ReportType != "Actual Number of Approved Thesis And Dissertation Grant" ||
                              r.ApplicationType == "Master's Thesis" || r.ApplicationType == "Dissertation") &&
                             (model.ReportType != "Actual Number of Approved National Paper Presentation" ||
                              r.ApplicationType.Contains("National Paper Presentation")) &&
                             (model.ReportType != "Actual Number of Approved International Paper Presentation" ||
                              r.ApplicationType.Contains("International Paper Presentation")) &&
                             (model.ReportType != "Actual Number of Approved Publication and Citation Incentives" ||
                              r.ApplicationType.Contains("Citation")) &&
                             (model.ReportType != "Actual Number of Approved Industrial Design, Utility Model, and Patent Incentive" ||
                              r.ApplicationType.Contains("Industrial Design, Utility Model, and Patent")) &&
                             (model.ReportType != "Actual Number of Approved University Publication Assistance" ||
                              r.ApplicationType.Contains("University Publication Assistance"))
                              )
                .ToListAsync();
            }

            // Group and prepare report data by year
            var groupedRequests = requests.GroupBy(r => r.CreatedDate.Year)
                                          .OrderBy(g => g.Key)
                                          .ToList();

            var reportData = groupedRequests.Select(g => new ReportData
            {
                Year = g.Key,
                TotalRequests = g.Count(),
                TotalSpent = g.Sum(r => r.RequestSpent),
                Requests = g.ToList()
            }).ToList();

            byte[] fileData;
            string mimeType;
            string fileExtension;

            // Generate the report based on the selected file type
            if (fileType == "Pdf")
            {
                fileData = GeneratePdfReport(reportData, reportData.Sum(r => r.TotalSpent), requests.Count, model.ReportType);
                mimeType = "application/pdf";
                fileExtension = "pdf";
            }
            else if (fileType == "Excel")
            {
                fileData = GenerateExcelReport(reportData, model.ReportType);
                mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                fileExtension = "xlsx";
            }
            else
            {
                return BadRequest("Invalid file type selected.");
            }

            // Save the generated report to the database
            var report = new GeneratedReport
            {
                ReportName = $"{model.ReportType}_{DateTime.Now:yyyyMMdd_HHmmss}",
                FileData = fileData,
                FileType = mimeType,
                GeneratedDate = DateTime.Now,
                GeneratedBy = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            _context.RSC_GeneratedReports.Add(report);
            await _context.SaveChangesAsync();

            // Return the file for download
            return File(fileData, mimeType, $"{model.ReportType}_{DateTime.Now:yyyyMMdd_HHmmss}.{fileExtension}");
        }

        public IActionResult Notifications()
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch notifications for the current user
            var notifications = _context.RSC_Notifications
                .Where(n => n.Role == "Chief")
                .OrderByDescending(n => n.NotificationCreationDate)
                .ToList();

            // Count notifications where NotificationStatus is false for this user
            ViewBag.UnreadNotificationsCount = _context.RSC_Notifications
                .Count(n => !n.NotificationStatus && n.Role == "Chief");

            // Pass the notifications to the view
            return View(notifications);
        }

        [HttpPost]
        public IActionResult MarkAllAsRead()
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find all unread notifications for the current user
            var notifications = _context.RSC_Notifications
                .Where(n => n.Role == "Chief" && !n.NotificationStatus)
                .ToList();

            // Mark each notification as read
            foreach (var notification in notifications)
            {
                notification.NotificationStatus = true;
            }

            // Save changes to the database
            _context.SaveChanges();

            return RedirectToAction("Notifications");
        }

        // Generate PDF Report as byte array
        private byte[] GeneratePdfReport(IEnumerable<dynamic> reportData, double overallTotalSpent, int overallApprovedCount, string reportType)
        {
            using (var ms = new MemoryStream())
            {
                // Use A4 landscape size
                var pdfWriter = new PdfWriter(ms);
                var pdfDocument = new PdfDocument(pdfWriter);
                var document = new Document(pdfDocument, PageSize.A4.Rotate()); // Set to landscape

                // Title
                document.Add(new Paragraph($"{reportType} - Research Support Center")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(18)
                    .SetBold());

                document.Add(new Paragraph($"Generated on: {DateTime.Now}")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(10));

                document.Add(new Paragraph("\n")); // Add some space

                // Add overall total spent
                document.Add(new Paragraph($"Overall Total Spent: PHP {overallTotalSpent:N2}")
                    .SetFontSize(12)
                    .SetBold());

                // Add overall count of approved requests
                document.Add(new Paragraph($"Overall Approved Requests: {overallApprovedCount}")
                    .SetFontSize(12)
                    /*                    .SetMarginTop(5)*/
                    .SetBold());

                // Iterate through each year group
                foreach (var group in reportData)
                {
                    // Add year header
                    document.Add(new Paragraph($"Year: {group.Year}")
                        .SetFontSize(16)
                        .SetBold()
                        .SetMarginTop(10));

                    // Add total spent for the year
                    document.Add(new Paragraph($"Total Spent: PHP {group.TotalSpent:N2}") // Format as currency
                        .SetFontSize(14)
                        .SetMarginTop(5));

                    // Create a table for requests of that year
                    var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1, 1 })).UseAllAvailableWidth();
                    // Set header cells with background color and bold font
                    var headerCells = new[]
                    {
                new Paragraph("DTS No").SetBold().SetBackgroundColor(new DeviceRgb(221, 217, 195)),
                new Paragraph("Applicant Name").SetBold().SetBackgroundColor(new DeviceRgb(221, 217, 195)),
                new Paragraph("Application Type").SetBold().SetBackgroundColor(new DeviceRgb(221, 217, 195)),
                new Paragraph("Field of Study").SetBold().SetBackgroundColor(new DeviceRgb(221, 217, 195)),
                new Paragraph("Research Title").SetBold().SetBackgroundColor(new DeviceRgb(221, 217, 195)),
                new Paragraph("College").SetBold().SetBackgroundColor(new DeviceRgb(221, 217, 195))
            };

                    foreach (var cell in headerCells)
                    {
                        table.AddHeaderCell(cell);
                    }

                    // Add rows to the table for this year
                    foreach (var request in group.Requests) // Use the included requests from the group
                    {
                        table.AddCell(request.DtsNo);
                        table.AddCell(request.Name ?? "N/A"); // Add applicant name
                        table.AddCell(request.ApplicationType);
                        table.AddCell(request.FieldOfStudy);
                        table.AddCell(request.ResearchTitle ?? "N/A");
                        table.AddCell(request.College ?? "N/A");
                    }

                    // Add the table to the document
                    document.Add(table);
                    document.Add(new Paragraph("\n")); // Add space after each year's table
                }

                // Close the document
                document.Close();

                // Return the PDF as a byte array
                return ms.ToArray();
            }
        }

        // Sample GenerateExcelReport method (same as provided earlier)
        private byte[] GenerateExcelReport(List<ReportData> reportData, string reportType)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Report");

                // Set title with the provided reportType
                worksheet.Cells["A1"].Value = $"{reportType} - Research Support Center";
                worksheet.Cells["A1"].Style.Font.Size = 18;
                worksheet.Cells["A1"].Style.Font.Bold = true;

                // Generated date
                worksheet.Cells["A2"].Value = $"Generated on: {DateTime.Now}";
                worksheet.Cells["A2"].Style.Font.Size = 10;

                // Overall totals
                var overallTotalSpent = reportData.Sum(r => r.TotalSpent);
                var overallApprovedCount = reportData.Sum(r => r.TotalRequests);
                worksheet.Cells["A4"].Value = $"Overall Total Spent: PHP {overallTotalSpent:N2}";
                worksheet.Cells["A4"].Style.Font.Size = 12;
                worksheet.Cells["A4"].Style.Font.Bold = true;

                worksheet.Cells["A5"].Value = $"Overall Approved Requests: {overallApprovedCount}";
                worksheet.Cells["A5"].Style.Font.Size = 12;
                worksheet.Cells["A5"].Style.Font.Bold = true;

                // Start populating data rows
                int currentRow = 7;

                foreach (var group in reportData)
                {
                    // Year header
                    worksheet.Cells[currentRow, 1].Value = $"Year: {group.Year}";
                    worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
                    worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                    currentRow++;

                    // Total spent for the year
                    worksheet.Cells[currentRow, 1].Value = $"Total Spent: PHP {group.TotalSpent:N2}";
                    worksheet.Cells[currentRow, 1].Style.Font.Size = 14;
                    currentRow++;

                    // Table headers
                    worksheet.Cells[currentRow, 1].Value = "DTS No";
                    worksheet.Cells[currentRow, 2].Value = "Applicant Name";
                    worksheet.Cells[currentRow, 3].Value = "Application Type";
                    worksheet.Cells[currentRow, 4].Value = "Field of Study";
                    worksheet.Cells[currentRow, 5].Value = "Research Title";
                    worksheet.Cells[currentRow, 6].Value = "College";

                    using (var range = worksheet.Cells[currentRow, 1, currentRow, 6])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    currentRow++;

                    // Add data rows for each request
                    foreach (var request in group.Requests)
                    {
                        worksheet.Cells[currentRow, 1].Value = request.DtsNo;
                        worksheet.Cells[currentRow, 2].Value = request.Name ?? "N/A";
                        worksheet.Cells[currentRow, 3].Value = request.ApplicationType;
                        worksheet.Cells[currentRow, 4].Value = request.FieldOfStudy;
                        worksheet.Cells[currentRow, 5].Value = request.ResearchTitle ?? "N/A";
                        worksheet.Cells[currentRow, 6].Value = request.College ?? "N/A";
                        currentRow++;
                    }

                    // Add space after each year's data
                    currentRow += 2;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }


        // Method to delete the report
        [HttpPost]
        public IActionResult DeleteReport(int id)
        {
            var report = _context.RSC_GeneratedReports.Find(id);
            if (report == null)
            {
                return NotFound();
            }

            // Remove the report from the database
            _context.RSC_GeneratedReports.Remove(report);
            _context.SaveChanges();

            // Redirect or return a success message
            return RedirectToAction("Reports"); // Adjust as needed
        }

        public IActionResult EvaluationForms()
        {
            // Fetch EvaluationForms and include related Criteria
            var evaluationForms = _context.RSC_EvaluationForms
                .Include(e => e.Criteria)
                .ToList();

            // Fetch Template data
            var templates = _context.RSC_Templates.ToList();

            // Pass both EvaluationForms and Templates as a Tuple to the view
            var model = Tuple.Create<IEnumerable<EvaluationForm>, IEnumerable<Template>>(evaluationForms, templates);

            ViewBag.UnreadNotificationsCount = _context.RSC_Notifications
            .Count(n => !n.NotificationStatus && n.Role == "Chief");

            return View(model);
        }

        public IActionResult CreateForms()
        {

            var model = new EvaluationFormViewModel
            {
                ApplicationTypes = _context.RSC_ApplicationTypes.ToList() // Fetch Application Types
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EvaluationFormViewModel evaluationFormViewModel)
        {
            if (ModelState.IsValid)
            {
                var applicationType = await _context.RSC_ApplicationTypes.FindAsync(evaluationFormViewModel.SelectedApplicationTypeId);
                if (applicationType != null)
                {
                    var evaluationForm = new EvaluationForm
                    {
                        Title = applicationType.ApplicationTypeName, // Use the ApplicationTypeName as the title
                        Criteria = evaluationFormViewModel.Criteria.Select(c => new Criterion
                        {
                            Name = c.Name,
                            Percentage = c.Percentage
                        }).ToList()
                    };

                    _context.RSC_EvaluationForms.Add(evaluationForm);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("EvaluationForms");
                }
            }

            // Repopulate Application Types in case of error
            evaluationFormViewModel.ApplicationTypes = _context.RSC_ApplicationTypes.ToList();
            return View(evaluationFormViewModel);
        }

        [HttpGet]
        public IActionResult Template(string id)
        {
            var template = _context.RSC_Templates.FirstOrDefault(t => t.TemplateId == id);
            if (template == null)
            {
                return NotFound();
            }

            // Return a JSON result with the memo data
            return Json(template);
        }

        [HttpPost]
        public async Task<IActionResult> EditTemplate(string id, string templateName, IFormFile file)
        {
            var template = await _context.RSC_Templates.FindAsync(id);
            if (template == null)
            {
                return NotFound();
            }

            // Update memorandum name
            template.TemplateName = templateName;

            // If a new file is uploaded, update file data
            if (file != null && file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    template.FileData = ms.ToArray();
                }
                template.FileType = file.ContentType;
                template.FileUploaded = DateTime.Now;
            }

            _context.RSC_Templates.Update(template);
            await _context.SaveChangesAsync();

            return RedirectToAction("EvaluationForms"); // Redirect to the view that lists the memorandums
        }

        [HttpPost]
        public async Task<IActionResult> EditSubcategory(int CategoryId, string CategoryName, decimal SubAmount)
        {
            var subcategory = await _context.RSC_ApplicationSubCategories.FindAsync(CategoryId);

            if (subcategory == null)
            {
                return NotFound();
            }

            subcategory.CategoryName = CategoryName;
            subcategory.SubAmount = SubAmount;

            _context.RSC_ApplicationSubCategories.Update(subcategory);
            await _context.SaveChangesAsync();

            return RedirectToAction("Checklist"); // Replace with your actual view name
        }

        public IActionResult ViewEvaluator()
        {

            return View();
        }

        public IActionResult Evaluators()
        {
            // Fetch all evaluators from the database
            var evaluators = _context.RSC_Evaluators
                .ToList();

            return View(evaluators);
        }

        // GET: Evaluation/Details/{id}
        public async Task<IActionResult> EvaluationDetails(int requestId)
        {
            // Fetch the Request based on the id
            var request = await _context.RSC_Requests
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null)
            {
                return NotFound();
            }

            // Fetch EvaluatorAssignments related to the request
            var evaluatorAssignments = await _context.RSC_EvaluatorAssignments
                .Include(ea => ea.Evaluators) // Include Evaluator (ApplicationUser)
                .Where(ea => ea.RequestId == request.RequestId)
                .ToListAsync();

            // Get the evaluator assignment IDs
            var evaluatorAssignmentIds = evaluatorAssignments.Select(ea => ea.AssignmentId).ToList();

            // Fetch evaluation documents related to the evaluator assignments
            var evaluationDocuments = await _context.RSC_EvaluationDocuments
                .Where(ed => evaluatorAssignmentIds.Contains(ed.EvaluatorAssignmentId))
                .ToListAsync();

            // Fetch general comments related to the evaluator assignments
            var generalComments = await _context.RSC_EvaluationGeneralComments
                .Where(gc => evaluatorAssignmentIds.Contains(gc.EvaluatorAssignmentId))
                .ToListAsync();

            // Fetch UserPercentage for the evaluations related to this request
            var userPercentages = await _context.RSC_EvaluationFormResponses
                .Where(response => evaluatorAssignmentIds.Contains(response.EvaluatorAssignmentId))
                .ToListAsync();

            // Group the responses by Evaluator
            var evaluatorScores = userPercentages
                .GroupBy(up => up.EvaluatorAssignmentId)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.UserPercentage)); // Sum the percentages per evaluator assignment

            // Determine how many evaluators have provided responses
            int completedEvaluatorsCount = evaluatorScores.Count;

            decimal averageUserPercentage;

            if (completedEvaluatorsCount == 0)
            {
                averageUserPercentage = 0; // No evaluations yet
            }
            else if (completedEvaluatorsCount == 1)
            {
                // Only one evaluator has submitted; take the sum as the total score
                averageUserPercentage = evaluatorScores.Values.First(); // Take the only score available
            }
            else
            {
                // More than one evaluator has provided responses; average their scores
                averageUserPercentage = evaluatorScores.Values.Average(); // Average the scores
            }

            // Determine the decision based on the average UserPercentage
            string decision = averageUserPercentage >= 60 ? "Approve" : "Reject";
            string evaluationMessage = averageUserPercentage >= 60
                ? "The application has passed the evaluation."
                : "The application has been rejected.";

            // Create ViewModel and pass the average percentage and decision along with other data
            var viewModel = new EvaluationStatusViewModel
            {
                Request = request,
                EvaluatorAssignments = evaluatorAssignments,
                EvaluationDocuments = evaluationDocuments,
                GeneralComments = generalComments,
                AverageUserPercentage = averageUserPercentage,
                Decision = decision,
                EvaluationMessage = evaluationMessage
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveApplication(int requestId)
        {
            // Find the request in the database
            var request = await _context.RSC_Requests
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null)
            {
                return NotFound();
            }

            // Update the request status to "Approved"
            request.Status = "Approved";
            request.SubmittedDate = DateTime.Now;

            // Add entry to StatusHistory
            var statusHistory = new StatusHistory
            {
                RequestId = requestId,
                Status = "Approved",
                StatusDate = DateTime.Now,
                Comments = "The application has been approved."
            };
            _context.RSC_StatusHistories.Add(statusHistory);

            // Add entry to DocumentHistory
            var documentHistory = new DocumentHistory
            {
                RequestId = requestId,
                CreateDate = DateTime.Now,
                Comments = "Application is approved. Awaiting the submission of the endorsement letter.",
                PerformedBy = "Chief"
            };
            _context.RSC_DocumentHistories.Add(documentHistory);

            // Prepare the notification message
            string notificationMessage = $"We are pleased to inform you that your application request with DTS Number {request.DtsNo} has been approved. Once we receive the hard copy of your documentary requirements, we will send the endorsement letter that formally recognizes the successful approval of your application. Thank you!";

            // Create a new notification entry
            var notification = new Notifications
            {
                UserId = request.UserId,
                NotificationTitle = "Application Approved",
                NotificationMessage = notificationMessage,
                NotificationCreationDate = DateTime.Now,
                NotificationStatus = false, // Set as unread
                Role = "Applicant",
                PerformedBy = "System",
                EvaluatorAssignmentId = null // Not linked to any evaluator assignment in this case
            };
            _context.RSC_Notifications.Add(notification);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Return a success response with the notification message
            return Ok();
        }

        public IActionResult GrantsOverview()
        {
            var approvedGrants = _context.RSC_Requests
              .Where(r => r.Status == "Endorsed by RMO" && r.ApplicationType == "Master's Thesis" || r.ApplicationType == "Dissertation")
              .GroupBy(r => r.College)
              .Select(g => new GrantsOverviewViewModel
              {
                  College = g.Key,
                  NumberOfApprovedGrants = g.Count()
              })
              .ToList();

            return View(approvedGrants);
        }

        public IActionResult IncentivesOverview()
        {
            var incentivesData = _context.RSC_Requests
               .Where(r => r.Status == "Endorsed by RMO")
               .Where(r => r.ApplicationType.Contains("Incentive") || r.ApplicationType.Contains("Literary"))
               .GroupBy(r => r.College)
               .Select(g => new IncentivesOverviewViewModel
               {
                   College = g.Key,
                   NumberOfApprovedIncentives = g.Count()
               })
               .ToList();

            return View(incentivesData);
        }

        public IActionResult AssistanceOverview()
        {
            var incentivesData = _context.RSC_Requests
               .Where(r => r.Status == "Endorsed by RMO")
               .Where(r => r.ApplicationType.Contains("Assistance") || r.ApplicationType.Contains("Presentation"))
               .GroupBy(r => r.College)
               .Select(g => new AssistanceOverviewViewModel
               {
                   College = g.Key,
                   NumberOfApprovedAssistance = g.Count()
               })
               .ToList();

            return View(incentivesData);
        }

        public ActionResult Forecasting()
        {
            var requestData = _context.RSC_Requests
                .Where(r => r.Status == "Approved" || r.Status == "Endorsed by RMO")
                .ToList()
                .GroupBy(r => r.CreatedDate.Year)
                .Select(g => new RequestData
                {
                    CreatedDate = new DateTime(g.Key, 1, 1),
                    RequestSpent = g.Sum(r => (float)r.RequestSpent)
                })
                .OrderBy(d => d.CreatedDate)
                .ToList();

            if (requestData.Count < 8)
            {
                ViewBag.Error = "Insufficient data for accurate forecasting. At least 8 years of data are required.";
                return View();
            }

            var mlContext = new MLContext();
            int seriesLength = requestData.Count;
            int windowSize = Math.Max(3, seriesLength / 4);
            int horizon = 2;

            if (seriesLength <= 2 * windowSize)
            {
                ViewBag.Error = $"The input size for training should be greater than twice the window size. " +
                                $"Series Length: {seriesLength}, Window Size: {windowSize}";
                return View();
            }

            int testSetSize = Math.Max(2, seriesLength / 10);
            var trainData = requestData.Take(seriesLength - testSetSize).ToList();
            var testData = requestData.Skip(seriesLength - testSetSize).ToList();

            var dataView = mlContext.Data.LoadFromEnumerable(trainData);

            var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedSpent",
                inputColumnName: "RequestSpent",
                windowSize: windowSize,
                seriesLength: seriesLength,
                trainSize: trainData.Count,
                horizon: horizon);

            var mlModel = forecastingPipeline.Fit(dataView);
            var forecastEngine = mlModel.CreateTimeSeriesEngine<RequestData, RequestForecast>(mlContext);
            var forecast = forecastEngine.Predict();

            var actualValues = testData.Select(t => t.RequestSpent).ToList();
            var predictedValues = forecast.ForecastedSpent.Take(testSetSize).ToList();

            // Calculate MAE, MAPE, RMSE
            double mae = actualValues.Zip(predictedValues, (actual, predicted) => Math.Abs(actual - predicted)).Average();
            double mape = actualValues.Zip(predictedValues, (actual, predicted) => Math.Abs((actual - predicted) / actual)).Average() * 100;
            double rmse = Math.Sqrt(actualValues.Zip(predictedValues, (actual, predicted) => Math.Pow(actual - predicted, 2)).Average());

            // Calculate R-squared
            double meanActual = actualValues.Average();
            double ssTot = actualValues.Sum(actual => Math.Pow(actual - meanActual, 2));
            double ssRes = actualValues.Zip(predictedValues, (actual, predicted) => Math.Pow(actual - predicted, 2)).Sum();
            double rSquared = 1 - (ssRes / ssTot);

            // Calculate Signal-to-Noise Ratio (SNR)
            double residualStdDev = Math.Sqrt(ssRes / actualValues.Count);
            double snr = meanActual / residualStdDev;

            ViewBag.MAE = mae;
            ViewBag.MAPE = mape;
            ViewBag.RMSE = rmse;
            ViewBag.RSquared = rSquared;
            ViewBag.SNR = snr;

            var historicalYears = requestData.Select(d => d.CreatedDate.Year).ToList();
            var historicalSpent = requestData.Select(d => d.RequestSpent).ToList();

            var lastYear = requestData.Last().CreatedDate.Year;
            for (int i = 0; i < horizon; i++)
            {
                historicalYears.Add(lastYear + (i + 1));
                historicalSpent.Add(forecast.ForecastedSpent[i]);
            }

            ViewBag.Years = historicalYears;
            ViewBag.Spent = historicalSpent;

            var viewModel = new BudgetForecastViewModel
            {
                Year = lastYear + horizon,
                ForecastedBudget = forecast.ForecastedSpent.Last(),
                HistoricalData = requestData.Select(d => new HistoricalSpending
                {
                    Year = d.CreatedDate.Year,
                    TotalSpent = d.RequestSpent
                }).ToList()
            };

            return View(viewModel);
        }



        public async Task<IActionResult> Checklist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the logged-in user's ID

            if (userId == null)
            {
                return Unauthorized();  // If not authenticated
            }

            var applicationTypes = await _context.RSC_ApplicationTypes
           .Include(at => at.ApplicationSubCategories)
               .ThenInclude(asc => asc.Checklists)
           .Include(at => at.Checklists) // Include checklists directly linked to the ApplicationType
           .ToListAsync();

            ViewBag.ApplicationTypes = applicationTypes;

            // Populate subcategories in a ViewBag
            ViewBag.Subcategories = applicationTypes
                .SelectMany(at => at.ApplicationSubCategories)
                .ToList();

            ViewBag.UnreadNotificationsCount = _context.RSC_Notifications
            .Count(n => !n.NotificationStatus && n.Role == "Chief");

            return View(applicationTypes);
        }

        [HttpPost]
        public async Task<IActionResult> AddApplicationType(string applicationTypeName, decimal amount)
        {
            if (string.IsNullOrEmpty(applicationTypeName))
            {
                ModelState.AddModelError("", "Application Type Name is required.");
                return RedirectToAction("Checklist"); // Redirect back to the Checklist view
            }

            var applicationType = new ApplicationType
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                ApplicationTypeName = applicationTypeName,
                Amount = amount,
                ApplicationTypeCreated = DateTime.Now,
                ApplicationTypeUpdated = DateTime.Now
            };

            _context.RSC_ApplicationTypes.Add(applicationType);
            await _context.SaveChangesAsync();

            return RedirectToAction("Checklist"); // Redirect to the Checklist view after adding
        }

        [HttpPost]
        public async Task<IActionResult> AddSubcategory(int applicationTypeId, string categoryName, decimal amount)
        {
            if (ModelState.IsValid)
            {
                var subcategory = new ApplicationSubCategory
                {
                    ApplicationTypeId = applicationTypeId,
                    CategoryName = categoryName,
                    SubAmount = amount
                };

                _context.RSC_ApplicationSubCategories.Add(subcategory);
                await _context.SaveChangesAsync();

                // Optionally, you could redirect or return a JSON result
                return RedirectToAction("Checklist"); // Adjust as needed
            }

            // If ModelState is invalid, return the view with the current data
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddChecklist(int? applicationTypeId, int? applicationSubCategoryId, List<string> checklistNames)
        {
            // Validate inputs
            if (checklistNames == null || checklistNames.Count == 0 ||
                (applicationTypeId == null && applicationSubCategoryId == null) ||
                (applicationTypeId.HasValue && applicationSubCategoryId.HasValue))
            {
                ModelState.AddModelError(string.Empty, "Please provide either an Application Type or a Subcategory, but not both.");
                return View(); // Return to the view with error
            }

            // Check if the application type has subcategories
            if (applicationTypeId.HasValue)
            {
                var applicationType = await _context.RSC_ApplicationTypes
                    .Include(at => at.ApplicationSubCategories)
                    .FirstOrDefaultAsync(at => at.ApplicationTypeId == applicationTypeId.Value);

                if (applicationType != null && applicationType.ApplicationSubCategories.Any())
                {
                    if (applicationSubCategoryId == null ||
                        !applicationType.ApplicationSubCategories.Any(s => s.CategoryId == applicationSubCategoryId.Value))
                    {
                        ModelState.AddModelError(string.Empty, "Please select a valid subcategory.");
                        return View();
                    }
                }
            }

            var addedChecklistDetails = new List<string>();

            // Loop through checklist names and create checklists
            foreach (var checklistName in checklistNames)
            {
                if (!string.IsNullOrWhiteSpace(checklistName))
                {
                    var checklist = new Checklist
                    {
                        ChecklistName = checklistName,
                        ApplicationSubCategoryId = applicationSubCategoryId,
                        ApplicationTypeId = applicationTypeId
                    };

                    _context.RSC_Checklists.Add(checklist);
                    addedChecklistDetails.Add(checklistName); // Store added checklist names
                }
            }

            // Attempt to save changes and catch any exceptions
            try
            {
                await _context.SaveChangesAsync();

                // Create notification message
                var applicationTypeName = applicationTypeId.HasValue
                    ? (await _context.RSC_ApplicationTypes.FindAsync(applicationTypeId)).ApplicationTypeName
                    : "N/A";

                var subcategoryName = applicationSubCategoryId.HasValue
                    ? (await _context.RSC_ApplicationSubCategories.FindAsync(applicationSubCategoryId)).CategoryName
                    : "N/A";

                var checklistMessage = $"The following checklists have been added: {string.Join(", ", addedChecklistDetails)}. " +
                                       $"Application Type: {applicationTypeName}, Subcategory: {subcategoryName}";

                // Notify all users about the new checklist
                await NotifyAllUsers("New Checklist Added", checklistMessage);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving data: " + ex.Message);
                return View(); // Return to the view with error
            }

            // Redirect to the checklist view or another appropriate action
            return RedirectToAction("Checklist");
        }

        // Method to notify all users
        private async Task NotifyAllUsers(string notificationTitle, string notificationMessage)
        {
            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                var notification = new Notifications
                {
                    UserId = user.Id,
                    NotificationTitle = notificationTitle,
                    NotificationMessage = notificationMessage,
                    NotificationCreationDate = DateTime.Now,
                    NotificationStatus = false,  // unread
                    Role = "All",
                    PerformedBy = "System"
                };

                _context.RSC_Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteChecklist(int id)
        {
            // Find the ApplicationType by id
            var applicationType = await _context.RSC_ApplicationTypes
                .Include(at => at.ApplicationSubCategories)
                .Include(at => at.Checklists)
                    .ThenInclude(c => c.Requirements) // Include Requirements
                .FirstOrDefaultAsync(at => at.ApplicationTypeId == id);

            if (applicationType == null)
            {
                return NotFound();
            }

            // Remove all related Requirements for each Checklist
            foreach (var checklist in applicationType.Checklists)
            {
                if (checklist.Requirements != null)
                {
                    _context.RSC_Requirements.RemoveRange(checklist.Requirements);
                }
            }

            // Remove the related Checklists
            _context.RSC_Checklists.RemoveRange(applicationType.Checklists);

            // Remove the related SubCategories
            _context.RSC_ApplicationSubCategories.RemoveRange(applicationType.ApplicationSubCategories);

            // Finally, remove the ApplicationType itself
            _context.RSC_ApplicationTypes.Remove(applicationType);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect or return a success message
            return RedirectToAction("Checklist"); // Adjust to your desired action
        }

        [HttpPost]
        public IActionResult DeleteCheck(int id)
        {
            // Retrieve the checklist and include related requirements
            var checklist = _context.RSC_Checklists
                .Include(c => c.Requirements) // Include the related requirements
                .FirstOrDefault(c => c.ChecklistId == id);

            if (checklist == null)
            {
                // Optionally handle the case where the checklist is not found
                return NotFound(new { message = "Checklist not found." });
            }

            try
            {
                // Remove all related requirements first
                if (checklist.Requirements != null && checklist.Requirements.Any())
                {
                    _context.RSC_Requirements.RemoveRange(checklist.Requirements);
                }

                // Remove the checklist itself
                _context.RSC_Checklists.Remove(checklist);

                // Save changes to the database
                _context.SaveChanges();

                // Optionally add a success message or log the action
                TempData["SuccessMessage"] = "Checklist and associated requirements deleted successfully.";
            }
            catch (Exception ex)
            {
                // Log the exception if logging is implemented
                // Example: _logger.LogError(ex, "Error deleting Checklist with ID: {id}", id);

                // Optionally handle the error and return a meaningful response
                TempData["ErrorMessage"] = "An error occurred while deleting the checklist.";
                return RedirectToAction("Checklist");
            }

            // Redirect to the checklist view or another appropriate view after successful deletion
            return RedirectToAction("Checklist");
        }

        [HttpPost]
        public IActionResult DeleteForm(int id)
        {
            var forms = _context.RSC_EvaluationForms.Find(id);
            if (forms != null)
            {
                _context.RSC_EvaluationForms.Remove(forms);
                _context.SaveChanges();
                // Optionally add a success message
            }
            return RedirectToAction("EvaluationForms"); // Redirect to the appropriate view after deletion
        }

        [HttpPost]
        public IActionResult DeleteTemplate(string id) // Change 'int' to 'string'
        {
            var temp = _context.RSC_Templates.Find(id); // Use 'id' to find the template by its string ID
            if (temp != null)
            {
                _context.RSC_Templates.Remove(temp);
                _context.SaveChanges();
                // Optionally add a success message or notification here
            }
            return RedirectToAction("EvaluationForms"); // Redirect to the appropriate view after deletion
        }

        [HttpPost]
        public IActionResult DeleteApplicationType(int id)
        {
            // Find the ApplicationType by id
            var applicationType = _context.RSC_ApplicationTypes
                                           .Include(at => at.ApplicationSubCategories) // Include related subcategories if needed
                                           .Include(at => at.Checklists) // Include related checklists if needed
                                           .FirstOrDefault(at => at.ApplicationTypeId == id);

            if (applicationType != null)
            {
                // Optionally, handle cascading deletes or remove related entities
                _context.RSC_ApplicationTypes.Remove(applicationType);
                _context.SaveChanges();

                // Optionally add a success message here
                TempData["SuccessMessage"] = "Application type deleted successfully.";
            }
            else
            {
                // Handle case where the application type was not found
                TempData["ErrorMessage"] = "Application type not found.";
            }

            // Redirect to the index or list view
            return RedirectToAction("Checklist");
        }

        public IActionResult DeleteSubcategory(int id)
        {
            // Find the ApplicationType by id
            var subcategory = _context.RSC_ApplicationSubCategories
                                           .Include(at => at.Checklists) // Include related checklists if needed
                                           .FirstOrDefault(at => at.CategoryId == id);

            if (subcategory != null)
            {
                // Optionally, handle cascading deletes or remove related entities
                _context.RSC_ApplicationSubCategories.Remove(subcategory);
                _context.SaveChanges();

                // Optionally add a success message here
                TempData["SuccessMessage"] = "Application subcategory deleted successfully.";
            }
            else
            {
                // Handle case where the application type was not found
                TempData["ErrorMessage"] = "Application subcategory not found.";
            }

            // Redirect to the index or list view
            return RedirectToAction("Checklist");
        }

        [HttpPost]
        public async Task<IActionResult> EditForm(int id, string criteriaName, decimal criteriaPercentage)
        {
            var criteria = await _context.RSC_Criteria.FindAsync(id);
            if (criteria == null)
            {
                return NotFound();
            }

            // Update memorandum name
            criteria.Name = criteriaName;
            criteria.Percentage = criteriaPercentage;

            _context.RSC_Criteria.Update(criteria);
            await _context.SaveChangesAsync();

            return RedirectToAction("EvaluationForms"); // Redirect to the view that lists the memorandums
        }

        [HttpPost]
        public async Task<IActionResult> EditAppType(int id, string applicationtypeName, decimal applicationtypeAmount)
        {
            var appType = await _context.RSC_ApplicationTypes.FindAsync(id);
            if (appType == null)
            {
                return NotFound();
            }

            // Update memorandum name
            appType.ApplicationTypeName = applicationtypeName;
            appType.Amount = applicationtypeAmount;
            appType.ApplicationTypeUpdated = DateTime.Now;

            _context.RSC_ApplicationTypes.Update(appType);
            await _context.SaveChangesAsync();

            return RedirectToAction("Checklist"); // Redirect to the view that lists the memorandums
        }

        // POST: EditChecklist
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditChecklist(int id, string checklistName)
        {
            if (id == 0 || string.IsNullOrEmpty(checklistName))
            {
                TempData["ErrorMessage"] = "Invalid input data.";
                return RedirectToAction("Checklist"); // Replace "Index" with your current page/action
            }

            // Find the checklist item in the database
            var checklist = await _context.RSC_Checklists.FirstOrDefaultAsync(c => c.ChecklistId == id);
            if (checklist == null)
            {
                TempData["ErrorMessage"] = "Checklist item not found.";
                return RedirectToAction("Checklist"); // Replace "Index" with your current page/action
            }

            // Update the checklist's properties
            checklist.ChecklistName = checklistName;

            try
            {
                // Save changes to the database
                _context.Update(checklist);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Checklist updated successfully.";
                return RedirectToAction("Checklist"); // Replace "Index" with your current page/action
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Error updating the checklist. Please try again.";
                return RedirectToAction("Checklist"); // Replace "Index" with your current page/action
            }
        }

        public IActionResult Index()
        {
            // Filter data for the year 2024
            var yearFilter = 2024;

            // Total spent on all approved or endorsed services
            var totalSpent = _context.RSC_Requests
                .Where(r => (r.Status == "Approved" || r.Status == "Endorsed by RMO") && r.CreatedDate.Year == yearFilter)
                .Sum(r => r.RequestSpent);

            // Count for grants (Master's Thesis, Dissertation)
            var totalGrants = _context.RSC_Requests
                .Where(r => (r.ApplicationType == "Master's Thesis" || r.ApplicationType == "Dissertation" || r.ApplicationType.Contains("Grants")) &&
                            (r.Status == "Approved" || r.Status == "Endorsed by RMO"))
                .Count();
            var totalGrantSpent = _context.RSC_Requests
                .Where(r => (r.ApplicationType == "Master's Thesis" || r.ApplicationType == "Dissertation" || r.ApplicationType.Contains("Grants")) &&
                            (r.Status == "Approved" || r.Status == "Endorsed by RMO") && r.CreatedDate.Year == yearFilter)
                .Sum(r => r.RequestSpent);

            // Count other categories
            var totalIncentives = _context.RSC_Requests
                .Count(r => r.ApplicationType.Contains("Incentive") || r.ApplicationType.Contains("Literary"));
            var totalIncentiveSpent = _context.RSC_Requests
                .Where(r => r.ApplicationType.Contains("Incentive") || r.ApplicationType.Contains("Literary") &&
                            (r.Status == "Approved" || r.Status == "Endorsed by RMO") && r.CreatedDate.Year == yearFilter)
                .Sum(r => r.RequestSpent);

            var totalAssistance = _context.RSC_Requests
                .Count(r => r.ApplicationType.Contains("Presentation") || r.ApplicationType.Contains("Assistance"));
            var totalAssistanceSpent = _context.RSC_Requests
                .Where(r => r.ApplicationType.Contains("Presentation") || r.ApplicationType.Contains("Assistance") &&
                            (r.Status == "Approved" || r.Status == "Endorsed by RMO") && r.CreatedDate.Year == yearFilter)
                .Sum(r => r.RequestSpent);

            // Fetch application request summary
            var pending = _context.RSC_Requests
                .Count(r => r.Status == "For Evaluation" || r.Status == "For Compliance");
            var approved = _context.RSC_Requests
                .Count(r => r.Status == "Approved" || r.Status == "Endorsed by RMO");
            var rejected = _context.RSC_Requests.Count(r => r.Status == "Rejected");

            var memorandums = _context.RSC_Memorandums.ToList();

            // Pass data to the view
            ViewBag.TotalSpent = totalSpent;
            ViewBag.TotalGrants = totalGrants;
            ViewBag.TotalIncentives = totalIncentives;
            ViewBag.TotalAssistance = totalAssistance;

            ViewBag.Pending = pending;
            ViewBag.Approved = approved;
            ViewBag.Rejected = rejected;

            ViewBag.UnreadNotificationsCount = _context.RSC_Notifications
            .Count(n => !n.NotificationStatus && n.Role == "Chief");

            // Calculate the total number of pending evaluations
            ViewBag.TotalPendingEvaluations = _context.RSC_Requests.Count(r => r.Status == "For Evaluation" || r.Status == "For Compliance");

            return View(memorandums);
        }

        public async Task<IActionResult> TechEval(int requestId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the request data and associated documents (Requirements) from the database
            var request = await _context.RSC_Requests
                .Include(r => r.Requirements)  // Include the associated Requirements (documents)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null)
            {
                return NotFound();  // Handle if the request doesn't exist
            }

            // Get the applicant's name formatted as "LastName, FirstName"
            var applicantName = $"{request.Name}";
            ViewData["ApplicantName"] = applicantName;

            // Retrieve ApplicationSubCategories for the draft based on criteria
            var applicationSubCategories = await _context.RSC_ApplicationSubCategories
                .Where(sc => sc.ApplicationType.ApplicationTypeName == request.ApplicationType
                           || sc.CategoryName == request.ApplicationType) // Adjusted condition
                .Include(sc => sc.Checklists) // Include Checklists for each ApplicationSubCategory
                .ToListAsync();

            var checklists = applicationSubCategories
                .SelectMany(sc => sc.Checklists)
                .ToList();

            // Retrieve document histories for the current request
            var documentHistories = await _context.RSC_DocumentHistories
                .Where(dh => dh.RequestId == requestId)
                .OrderByDescending(dh => dh.CreateDate) // Order by the creation date
                .ToListAsync();

            // Determine if the decision should be disabled based on comments in document history
            var isDecisionDisabled = documentHistories
                .Any(dh => dh.Comments.Contains("Notice to Proceed", StringComparison.OrdinalIgnoreCase) ||
                           dh.Comments.Contains("Approved", StringComparison.OrdinalIgnoreCase) ||
                           dh.Comments.Contains("Rejected", StringComparison.OrdinalIgnoreCase));

            var model = new TechEvalViewModel
            {
                Request = request,
                Requirements = request.Requirements.ToList(),
                Checklists = checklists, // Set the Checklists here
                DocumentHistories = documentHistories,
                IsDecisionDisabled = isDecisionDisabled
            };

            // Check if "Requirements Under Review" status already exists in the StatusHistory for this request
            var existingStatus = await _context.RSC_StatusHistories
                .FirstOrDefaultAsync(sh => sh.RequestId == requestId && sh.Status == "Requirements Under Review");

            // If the status does not already exist, add it to the StatusHistory
            if (existingStatus == null)
            {
                var statusHistory = new StatusHistory
                {
                    RequestId = requestId,
                    Status = "Requirements Under Review",
                    StatusDate = DateTime.Now,
                    Comments = "Status updated upon viewing documents."
                };

                _context.RSC_StatusHistories.Add(statusHistory);
            }

            // Get the owner of the request
            var requestOwnerId = request.UserId; // Assuming UserId is the foreign key in Requests table that points to the owner

            // Check if a notification for this request already exists for the owner
            var existingNotification = await _context.RSC_Notifications
                .Where(n => n.UserId == requestOwnerId && n.NotificationMessage.Contains(request.DtsNo))
                .FirstOrDefaultAsync();

            // If the notification does not already exist for the request owner, create it
            if (existingNotification == null)
            {
                var notification = new Notifications
                {
                    UserId = requestOwnerId,  // Set the owner of the request as the notification recipient
                    NotificationTitle = "Application Under Review",
                    NotificationMessage = $"Your application (DTS No: {request.DtsNo}) is now under review.",
                    NotificationCreationDate = DateTime.Now,
                    NotificationStatus = false // Set as unread
                };

                _context.RSC_Notifications.Add(notification);
            }

            await _context.SaveChangesAsync(); // Save changes asynchronously

            return View(model);
        }

        // Action to view the file
        public IActionResult ViewFile(int requirementId)
        {
            var requirement = _context.RSC_Requirements.FirstOrDefault(r => r.RequirementId == requirementId);

            if (requirement == null)
            {
                return NotFound();
            }

            Response.Headers.Add("Content-Disposition", "inline; filename=\"" + requirement.FileName + "\"");
            return File(requirement.FileData, requirement.FileType);
        }

        public IActionResult ViewMemo(int memorandumId)
        {
            var memo = _context.RSC_Memorandums.FirstOrDefault(r => r.memorandumId == memorandumId);

            if (memo == null)
            {
                return NotFound();
            }

            Response.Headers.Add("Content-Disposition", "inline; filename=\"" + memo.memorandumName + "\"");
            return File(memo.memorandumData, memo.filetype);
        }

        public IActionResult ViewReport(int ReportId)
        {
            var report = _context.RSC_GeneratedReports.FirstOrDefault(r => r.ReportId == ReportId);

            if (report == null)
            {
                return NotFound();
            }

            Response.Headers.Add("Content-Disposition", "inline; filename=\"" + report.ReportName + "\"");
            return File(report.FileData, report.FileType);
        }

        [HttpGet]
        public IActionResult EditMemo(int id)
        {
            var memo = _context.RSC_Memorandums.FirstOrDefault(m => m.memorandumId == id);
            if (memo == null)
            {
                return NotFound();
            }

            // Return a JSON result with the memo data
            return Json(memo);
        }

        [HttpPost]
        public async Task<IActionResult> EditMemo(int id, string memorandumName, IFormFile file)
        {
            var memo = await _context.RSC_Memorandums.FindAsync(id);
            if (memo == null)
            {
                return NotFound();
            }

            // Update memorandum name
            memo.memorandumName = memorandumName;

            // If a new file is uploaded, update file data
            if (file != null && file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    memo.memorandumData = ms.ToArray();
                }
                memo.filetype = file.ContentType;
                memo.memorandumUpdateDate = DateOnly.FromDateTime(DateTime.Now);
            }

            _context.RSC_Memorandums.Update(memo);
            await _context.SaveChangesAsync();

            return RedirectToAction("Memorandums"); // Redirect to the view that lists the memorandums
        }

        public IActionResult ViewEvaluationSheet(int EvaluationDocuId)
        {
            var document = _context.RSC_EvaluationDocuments.Find(EvaluationDocuId);
            if (document == null)
            {
                return NotFound();
            }

            Response.Headers.Add("Content-Disposition", "inline; filename=\"" + document.FileName + "\"");
            // Return the document data as a file
            return File(document.FileData, document.FileType);
        }

        public async Task<IActionResult> AssignEvaluator(int requestId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var request = _context.RSC_Requests
                .Include(r => r.Requirements)
                .Include(r => r.EvaluatorAssignments)
                .FirstOrDefault(r => r.RequestId == requestId);

            if (request == null)
            {
                return NotFound();
            }

            var ownerId = request.UserId;
            var roleName = "Evaluator";

            // Check if evaluators are already assigned
            if (!request.EvaluatorAssignments.Any())
            {
                // Fetch evaluators with matching specialization first (using contains logic)
                var evaluatorsWithSpecialization = await _context.RSC_Evaluators
                    .Where(u => u.EvaluatorId != ownerId &&
                                (u.Specialization.Contains(request.FieldOfStudy) ||
                                 request.FieldOfStudy.Contains(u.Specialization)) &&
                                 _context.RSC_EvaluatorAssignments.Count(ea => ea.EvaluatorId == u.EvaluatorId && ea.EvaluationStatus == "Pending") < 5)
                    .OrderBy(u => u.PendingCount)
                    .Take(3)
                    .ToListAsync();

                // If not enough evaluators with specialization, get the remaining evaluators with least pending workload
                var requiredEvaluatorsCount = 3 - evaluatorsWithSpecialization.Count;

                if (requiredEvaluatorsCount > 0)
                {
                    var additionalEvaluators = await _context.RSC_Evaluators
                        .Where(u => u.EvaluatorId != ownerId &&
                                    !evaluatorsWithSpecialization.Select(e => e.EvaluatorId).Contains(u.EvaluatorId) &&
                                    _context.RSC_EvaluatorAssignments.Count(ea => ea.EvaluatorId == u.EvaluatorId && ea.EvaluationStatus == "Pending") < 5)
                        .OrderBy(u => u.PendingCount)
                        .Take(requiredEvaluatorsCount)
                        .ToListAsync();

                    evaluatorsWithSpecialization.AddRange(additionalEvaluators);
                }

                // If no evaluators are available, do not assign and log this information (or handle it as needed)
                if (evaluatorsWithSpecialization.Count == 0)
                {
                    // Log or handle the case when no evaluators are available
                    TempData["Message"] = "No evaluators available with less than 5 pending evaluations.";
                    return RedirectToAction("Applications", "RSCChief", new { area = "RscSys" }); // Show a view indicating no evaluators are available
                }

                // Assign the evaluators
                var assignments = new List<EvaluatorAssignment>();

                foreach (var evaluator in evaluatorsWithSpecialization)
                {
                    var assignment = new EvaluatorAssignment
                    {
                        RequestId = requestId,
                        EvaluatorId = evaluator.EvaluatorId,
                        EvaluationStatus = "Pending",
                        AssignedDate = DateTime.Now,
                        EvaluationDeadline = DateTime.Now.AddDays(3)
                    };
                    assignments.Add(assignment);
                }

                _context.RSC_EvaluatorAssignments.AddRange(assignments);
                await _context.SaveChangesAsync(); // Save the evaluator assignments first

                // Now create and save notifications for the evaluators
                foreach (var assignment in assignments)
                {
                    var notification = new Notifications
                    {
                        UserId = assignment.EvaluatorId,
                        NotificationTitle = "New Evaluation Assignment",
                        NotificationMessage = $"You have been assigned to evaluate: <br/>DTS Number: {request.DtsNo}<br/>Application Type: {request.ApplicationType}<br/>Applicant Name: {request.Name} <br/>Deadline: {assignment.EvaluationDeadline.ToShortDateString()}<br/>Please respond as soon as possible. Thank you!",
                        NotificationCreationDate = DateTime.Now,
                        NotificationStatus = false, // Unread notification
                        Role = "Evaluator",
                        PerformedBy = "RSC Chief",
                        EvaluatorAssignmentId = assignment.AssignmentId
                    };
                    _context.RSC_Notifications.Add(notification);
                }

                await _context.SaveChangesAsync(); // Save changes for the notifications

                // Add an entry to the StatusHistory table
                var statusHistory = new StatusHistory
                {
                    RequestId = requestId,
                    Status = "Application Under Evaluation",
                    StatusDate = DateTime.Now,
                    Comments = "Evaluators have been assigned to the application."
                };
                _context.RSC_StatusHistories.Add(statusHistory);

                await _context.SaveChangesAsync(); // Save changes for the StatusHistory entry
            }

            // Fetching assigned evaluators for the request
            var assignedEvaluators = await _context.RSC_EvaluatorAssignments
                .Where(ea => ea.RequestId == requestId)
                .Join(_context.RSC_Evaluators,
                      ea => ea.EvaluatorId,
                      user => user.EvaluatorId,
                      (ea, user) => new EvaluatorAssignmentViewModel
                      {
                          AssignmentId = ea.AssignmentId,
                          RequestId = ea.RequestId,
                          EvaluatorId = ea.EvaluatorId,
                          EvaluationStatus = ea.EvaluationStatus,
                          AssignedDate = ea.AssignedDate,
                          EvaluationDeadline = ea.EvaluationDeadline,
                          Feedback = ea.Feedback,
                          Request = ea.Request,
                          EvaluatorName = $"{user.Name}"
                      })
                .ToListAsync();

            var assignedEvaluatorIds = assignedEvaluators.Select(ea => ea.EvaluatorId).ToList();

            // Fetching all evaluators
            var evaluatorsList = await _context.RSC_Evaluators
               .Where(u => u.EvaluatorId != ownerId)
               .Select(u => new
               {
                   User = u,
                   PendingCount = _context.RSC_EvaluatorAssignments.Count(ea => ea.EvaluatorId == u.EvaluatorId && ea.EvaluationStatus == "Pending" || ea.EvaluationStatus == "Accepted"),
                   CompletedCount = _context.RSC_EvaluatorAssignments.Count(ea => ea.EvaluatorId == u.EvaluatorId && ea.EvaluationStatus == "Approved"),
                   DeclinedCount = _context.RSC_EvaluatorAssignments.Count(ea => ea.EvaluatorId == u.EvaluatorId && ea.EvaluationStatus == "Rejected")
               })
               .ToListAsync();

            var viewModel = new AssignEvaluatorViewModel
            {
                Request = request,
                Requirements = await _context.RSC_Requirements.Where(r => r.RequestId == request.RequestId).ToListAsync(),
                EvaluatorAssignments = assignedEvaluators,
                EvaluatorProfile = evaluatorsList.Select(e => new rscSys_final.Models.Evaluator
                {
                    EvaluatorId = e.User.EvaluatorId,
                    Name = e.User.Name,
                    Specialization = e.User.Specialization,
                    PendingCount = e.PendingCount,
                    CompletedCount = e.CompletedCount,
                    DeclinedCount = e.DeclinedCount
                }).ToList()
            };

            ViewData["AssignedEvaluatorIds"] = assignedEvaluatorIds;

            var user = await _userManager.FindByIdAsync(userId);

            ViewData["ApplicantName"] = $"{user.LastName}, {user.FirstName}";

            var evaluationDeadlineOptions = Enumerable.Range(3, 8)
                .Select(day => new SelectListItem
                {
                    Text = day.ToString(),
                    Value = day.ToString(),
                    Selected = day == 3
                }).ToList();

            ViewBag.EvaluationDeadlineOptions = evaluationDeadlineOptions;

            // Count notifications where NotificationStatus is false for this user
            ViewBag.UnreadNotificationsCount = _context.RSC_Notifications
                .Count(n => !n.NotificationStatus && n.Role == "Chief");

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ManualAssignEvaluator(int requestId, string evaluatorId)
        {
            var request = await _context.RSC_Requests
                .Include(r => r.EvaluatorAssignments)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null) return NotFound("Request not found.");
            if (request.EvaluatorAssignments.Count >= 3) return BadRequest("Cannot assign more than 3 evaluators.");

            // Check if the evaluator exists
            var evaluator = await _userManager.FindByIdAsync(evaluatorId);
            if (evaluator == null) return NotFound("Evaluator not found.");

            // Check if the evaluator has 5 or more pending evaluations
            var pendingEvaluationsCount = await _context.RSC_EvaluatorAssignments
                .CountAsync(ea => ea.EvaluatorId == evaluatorId && ea.EvaluationStatus == "Pending");

            if (pendingEvaluationsCount >= 5)
            {
                return BadRequest("Evaluator cannot be assigned because they already have 5 pending evaluations.");
            }

            var assignment = new EvaluatorAssignment
            {
                RequestId = requestId,
                EvaluatorId = evaluatorId,
                EvaluationStatus = "Pending",
                AssignedDate = DateTime.Now,
                EvaluationDeadline = DateTime.Now.AddDays(3)
            };

            _context.RSC_EvaluatorAssignments.Add(assignment);
            await _context.SaveChangesAsync(); // Save to generate the AssignmentId

            var notification = new Notifications
            {
                UserId = evaluatorId,
                NotificationTitle = "New Evaluation Assignment",
                NotificationMessage = $"You have been assigned to evaluate: <br/>DTS Number: {request.DtsNo}<br/>Application Type: {request.ApplicationType}<br/>Applicant Name: {request.Name}<br/>Deadline: {assignment.EvaluationDeadline.ToShortDateString()}<br/>Please respond as soon as possible. Thank you!",
                NotificationCreationDate = DateTime.Now,
                NotificationStatus = false, // Unread notification
                Role = "Evaluator",
                PerformedBy = "RSC Chief",
                EvaluatorAssignmentId = assignment.AssignmentId
            };
            _context.RSC_Notifications.Add(notification);

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveEvaluator(int requestId, string evaluatorId)
        {
            var request = await _context.RSC_Requests
                .Include(r => r.EvaluatorAssignments)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null) return NotFound("Request not found.");
            if (request.EvaluatorAssignments.Count <= 1) return BadRequest("At least one evaluator is required.");

            var assignment = request.EvaluatorAssignments.FirstOrDefault(ea => ea.EvaluatorId == evaluatorId);
            if (assignment == null) return NotFound("Assignment not found.");

            _context.RSC_EvaluatorAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateAllEvaluatorsDeadline(int evaluationDeadline, int requestId)
        {
            // Fetch all assignments related to the current request ID
            var assignments = _context.RSC_EvaluatorAssignments.Where(a => a.RequestId == requestId).ToList();

            if (assignments.Any())
            {
                foreach (var assignment in assignments)
                {
                    // Update the EvaluationDeadline based on the assigned date
                    assignment.EvaluationDeadline = assignment.AssignedDate.AddDays(evaluationDeadline);
                }

                // Save changes to the database
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Evaluation deadlines updated successfully for all evaluators!";
            }
            else
            {
                TempData["SuccessMessage"] = "No evaluators found to update the deadline.";
            }

            return RedirectToAction("AssignEvaluator", new { requestId = requestId });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitDecision(int requestId, string decision, string comments)
        {
            // Fetch the request from the database
            var request = await _context.RSC_Requests.FindAsync(requestId);
            if (request == null)
            {
                return NotFound();  // Handle if the request doesn't exist
            }

            // Reset IsResubmitted to false for all associated requirements
            var requirements = _context.RSC_Requirements.Where(r => r.RequestId == requestId).ToList();
            foreach (var requirement in requirements)
            {
                requirement.IsResubmitted = false;
            }

            // Handle decision logic
            if (decision == "Notice to Proceed")
            {

                // Set the request status to "For Evaluation"
                request.Status = "For Evaluation"; // Ensure the status is set to "For Evaluation"

                // Add to Document History for Notice to Proceed
                var documentHistory = new DocumentHistory
                {
                    RequestId = requestId,
                    CreateDate = DateTime.Now,
                    Comments = "Notice to Proceed: " + comments,
                    PerformedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) // Assuming you want to log the user who performed the action
                };

                _context.RSC_DocumentHistories.Add(documentHistory);
            }
            else if (decision == "Approved" || decision == "Rejected" || decision == "For Compliance")
            {
                // Update request status
                request.Status = decision; // Assuming there's a Status property in your Request model

                // Add to Document History for other decisions
                var documentHistory = new DocumentHistory
                {
                    RequestId = requestId,
                    CreateDate = DateTime.Now,
                    Comments = $"{decision}: " + comments,
                    PerformedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) // Log the user who performed the action
                };

                _context.RSC_DocumentHistories.Add(documentHistory);

                var statusHistory = new StatusHistory
                {
                    RequestId = requestId,
                    Status = "Approved",
                    StatusDate = DateTime.Now,
                    Comments = "Your application is approved"
                };

                _context.RSC_StatusHistories.Add(statusHistory);
            }

            await _context.SaveChangesAsync(); // Save changes to the database

            // Redirect back to the evaluation view or wherever you need
            return RedirectToAction("Applications"/*, new { requestId }*/);
        }

        // View all requests with pagination
        public IActionResult Applications(int? yearFilter, string searchTerm = "", string collegeFilter = "", string applicationTypeFilter = "", int page = 1, int pageSize = 10)
        {
            // Base query: Include related data
            IQueryable<rscSys_final.Models.Request> requestsQuery = _context.RSC_Requests
                .Include(r => r.DocumentHistories)
                .Include(r => r.EvaluatorAssignments)
                    .ThenInclude(ea => ea.Evaluators);

            // Apply filters conditionally
            if (!string.IsNullOrEmpty(collegeFilter))
            {
                requestsQuery = requestsQuery.Where(r => r.College == collegeFilter);
            }

            if (!string.IsNullOrEmpty(applicationTypeFilter))
            {
                var selectedApplicationType = _context.RSC_ApplicationTypes
                    .Include(at => at.ApplicationSubCategories)
                    .FirstOrDefault(at => at.ApplicationTypeName == applicationTypeFilter);

                if (selectedApplicationType != null)
                {
                    var subCategoryNames = selectedApplicationType.ApplicationSubCategories.Select(sc => sc.CategoryName).ToList();

                    requestsQuery = requestsQuery.Where(r =>
                        r.ApplicationType == applicationTypeFilter ||
                        subCategoryNames.Contains(r.ApplicationType));
                }
            }

            if (yearFilter.HasValue)
            {
                requestsQuery = requestsQuery.Where(r => r.CreatedDate.Year == yearFilter.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                requestsQuery = requestsQuery.Where(r =>
                    r.DtsNo.Contains(searchTerm) ||  // Replace with actual DTS No. field
                    r.Name.Contains(searchTerm) ||  // Replace with actual Applicant Name field
                    r.ResearchTitle.Contains(searchTerm)); // Replace with actual Research Title field
            }

            // Get total requests count after filters
            int totalRequests = requestsQuery.Count();

            // Apply sorting and pagination
            var requests = requestsQuery
                .OrderByDescending(r => r.CreatedDate) // Sort by date, adjust if needed
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Dropdown data for filters
            var applicationTypes = _context.RSC_ApplicationTypes
                .Select(at => at.ApplicationTypeName)
                .Distinct()
                .ToList();

            var colleges = _context.RSC_Requests
                .Select(r => r.College)
                .Distinct()
                .ToList();

            var years = _context.RSC_Requests
                .Select(r => r.CreatedDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            // Pass data to the view using ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRequests / pageSize);
            ViewBag.ApplicationTypes = applicationTypes;
            ViewBag.Years = years;
            ViewBag.Colleges = colleges;
            ViewBag.SelectedCollege = collegeFilter;
            ViewBag.SelectedApplicationType = applicationTypeFilter;
            ViewBag.SelectedYear = yearFilter;
            ViewBag.SearchTerm = searchTerm;

            // Count notifications where NotificationStatus is false for this user
            ViewBag.UnreadNotificationsCount = _context.RSC_Notifications
                .Count(n => !n.NotificationStatus && n.Role == "Chief");

            return View(requests);
        }


        public async Task<IActionResult> Memorandums()
        {
            var memorandums = await _context.RSC_Memorandums.ToListAsync();
            ViewBag.UnreadNotificationsCount = _context.RSC_Notifications
            .Count(n => !n.NotificationStatus && n.Role == "Chief");

            return View(memorandums);
        }

        [HttpPost]
        public async Task<IActionResult> UploadMemorandum(IFormFile file, string memorandumName)
        {
            if (file != null && file.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                var memorandum = new Memorandum
                {
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier), // Get current user ID
                    memorandumName = memorandumName,
                    memorandumData = memoryStream.ToArray(), // Store file as byte array
                    filetype = file.ContentType, // Get file type
                    memorandumUploadDate = DateOnly.FromDateTime(DateTime.Now) // Set upload date
                };

                _context.RSC_Memorandums.Add(memorandum);

                try
                {
                    await _context.SaveChangesAsync();

                    // Create notification message
                    var notificationMessage = $"A new memorandum has been uploaded: {memorandumName}. ";

                    // Notify all users about the new memorandum
                    await NotifyAllUsers("New Memorandum Uploaded", notificationMessage);

                    return RedirectToAction("Memorandums"); // Redirect to your desired action after upload
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the memorandum: " + ex.Message);
                    return View("Memorandums");
                }
            }

            ModelState.AddModelError("", "Please select a file to upload.");
            return View("Memorandums");
        }

        // Method to delete a memorandum by ID
        [HttpPost]
        public async Task<IActionResult> DeleteMemorandum(int id)
        {
            var memorandum = await _context.RSC_Memorandums.FindAsync(id);
            if (memorandum == null)
            {
                return NotFound();
            }

            _context.RSC_Memorandums.Remove(memorandum);
            await _context.SaveChangesAsync();

            return RedirectToAction("Memorandums");
        }

        [HttpPost]
        public IActionResult MarkHardCopyAsReceived(int requestId)
        {
            var request = _context.RSC_Requests.Find(requestId);
            if (request != null)
            {
                request.IsHardCopyReceived = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Applications");
        }
        public async Task<IActionResult> GetUploadedFiles(int requestId)
        {
            var uploadedFiles = await _context.RSC_FinalDocuments
                .Where(f => f.RequestId == requestId)
                .ToListAsync();

            return Json(uploadedFiles);
        }

        [HttpGet]
        public async Task<JsonResult> GetCalendarEvents()
        {
            var events = await _crdlDbContext.ResearchEvent
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

        // GET: Reports
        public IActionResult GenerateReportDirector()
        {
            var model = new ReportFilterViewModel
            {
                GeneratedReports = _context.RSC_GeneratedReports.ToList() // Retrieve generated reports for the view
            };
            ViewBag.UnreadNotificationsCount = _context.RSC_Notifications
            .Count(n => !n.NotificationStatus && n.Role == "Chief , Director");

            return View(model);
        }


        // Controller Action to Generate Report for director
        [HttpPost]
        public async Task<IActionResult> GenerateReportDirector(ReportFilterViewModel model, string fileType)
        {
            List<rscSys_final.Models.Request> requests;

            // Fetch requests based on the selected criteria
            if (model.ReportType == "Summary Report" && string.IsNullOrEmpty(model.College) && model.StartDate == null && model.EndDate == null && string.IsNullOrEmpty(model.Branch))
            {
                requests = await _context.RSC_Requests
                    .Where(r => r.Status == "Approved" || r.Status == "Endorsed by RMO")
                    .ToListAsync();
            }
            else
            {
                requests = await _context.RSC_Requests
                 .Where(r => (r.Status == "Approved" || r.Status == "Endorsed by RMO") &&
                             (model.StartDate == null || r.CreatedDate >= model.StartDate) &&
                             (model.EndDate == null || r.CreatedDate <= model.EndDate) &&
                             (string.IsNullOrEmpty(model.College) || r.College == model.College) &&
                             (string.IsNullOrEmpty(model.Branch) || r.Branch == model.Branch) &&
                             (model.ReportType != "Actual Number of Approved Thesis And Dissertation Grant" ||
                              r.ApplicationType == "Master's Thesis" || r.ApplicationType == "Dissertation") &&
                             (model.ReportType != "Actual Number of Approved National Paper Presentation" ||
                              r.ApplicationType.Contains("National Paper Presentation")) &&
                             (model.ReportType != "Actual Number of Approved International Paper Presentation" ||
                              r.ApplicationType.Contains("International Paper Presentation")) &&
                             (model.ReportType != "Actual Number of Approved Publication and Citation Incentives" ||
                              r.ApplicationType.Contains("Citation")) &&
                             (model.ReportType != "Actual Number of Approved Industrial Design, Utility Model, and Patent Incentive" ||
                              r.ApplicationType.Contains("Industrial Design, Utility Model, and Patent")) &&
                             (model.ReportType != "Actual Number of Approved University Publication Assistance" ||
                              r.ApplicationType.Contains("University Publication Assistance"))
                              )
                .ToListAsync();
            }

            // Group and prepare report data by year
            var groupedRequests = requests.GroupBy(r => r.CreatedDate.Year)
                                          .OrderBy(g => g.Key)
                                          .ToList();

            var reportData = groupedRequests.Select(g => new ReportData
            {
                Year = g.Key,
                TotalRequests = g.Count(),
                TotalSpent = g.Sum(r => r.RequestSpent),
                Requests = g.ToList()
            }).ToList();

            byte[] fileData;
            string mimeType;
            string fileExtension;

            // Generate the report based on the selected file type
            if (fileType == "Pdf")
            {
                fileData = GeneratePdfReport(reportData, reportData.Sum(r => r.TotalSpent), requests.Count, model.ReportType);
                mimeType = "application/pdf";
                fileExtension = "pdf";
            }
            else if (fileType == "Excel")
            {
                fileData = GenerateExcelReport(reportData, model.ReportType);
                mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                fileExtension = "xlsx";
            }
            else
            {
                return BadRequest("Invalid file type selected.");
            }

            // Save the generated report to the database
            var report = new GeneratedReport
            {
                ReportName = $"{model.ReportType}_{DateTime.Now:yyyyMMdd_HHmmss}",
                FileData = fileData,
                FileType = mimeType,
                GeneratedDate = DateTime.Now,
                GeneratedBy = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            _context.RSC_GeneratedReports.Add(report);
            await _context.SaveChangesAsync();

            // Return the file for download
            return File(fileData, mimeType, $"{model.ReportType}_{DateTime.Now:yyyyMMdd_HHmmss}.{fileExtension}");
        }
    }
}
