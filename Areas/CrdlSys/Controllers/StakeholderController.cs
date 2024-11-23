using CrdlSys.ViewModels;
using CrdlSys.Data;
using CrdlSys.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity;
using ResearchManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace CrdlSys.Controllers
{
    [Area("CrdlSys")]
    public class StakeholderController : Controller
    {
        private readonly CrdlDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StakeholderController(CrdlDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles ="Stakeholder")]
        public IActionResult HomeStake()
        {
            return View();
        }

        [Authorize(Roles = "Stakeholder")]
        [HttpGet]
        public IActionResult UploadDocument()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument(UploadDocumentViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "You need to be logged in to upload documents.";
                return RedirectToAction("Login", "Account"); 
            }

            if (!ModelState.IsValid)
            {

                if (model.TypeOfDocument != "MOA")
                {
                    ModelState.Remove("TypeOfMOA");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }
            }
            
            /*var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["Error"] = "Invalid user ID.";
                return View(model);
            }*/

            /*var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == 2);

            if (userRole == null)
            {
                TempData["Error"] = "You do not have permission to upload documents.";
                return View(model);
            }*/

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    var user = await _userManager.GetUserAsync(User);
                    await model.DocumentFile.CopyToAsync(memoryStream);
                    byte[] documentBytes = memoryStream.ToArray();

                    var stakeholderUpload = new StakeholderUpload
                    {
                        DocumentId = StakeholderUpload.GenerateDocumentId(),
                        StakeholderId = user.Id,
                        StakeholderName = $"{user.FirstName} {user.LastName}",
                        StakeholdeEmail = user.Email,
                        NameOfDocument = model.NameOfDocument,
                        TypeOfDocument = model.TypeOfDocument,
                        TypeOfMOA = model.TypeOfDocument == "MOA" ? model.TypeOfMOA : null,
                        DocumentFile = documentBytes,
                        DocumentDescription = model.DocumentDescription,
                        Status = "Pending",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        IsArchived = false,
                        LastNotificationSent = DateTime.Now
                    };

                    _context.StakeholderUpload.Add(stakeholderUpload);
                    await _context.SaveChangesAsync();
                     
                    if (model.TypeOfDocument != "MOA" || model.TypeOfMOA == "Others")
                    {
                        var documentTracking = new DocumentTracking
                        {
                            TrackingId = DocumentTracking.GenerateTrackingId(),
                            DocumentId = stakeholderUpload.DocumentId,
                            IsReceivedByRMO = true,
                            UpdatedAt = DateTime.Now,
                            IsReceivedByRMOUpdatedAt = DateTime.Now
                        };

                        _context.DocumentTracking.Add(documentTracking);
                        await _context.SaveChangesAsync();
                    }
                    string stakeholderName = $"{user.FirstName} {user.LastName}";

                    await SendEmailNotification(stakeholderUpload, stakeholderName);

                    TempData["Success"] = "Document uploaded successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error occurred while uploading document: {ex.Message}";
            }

            return RedirectToAction("UploadDocument");
        }

        private async Task SendEmailNotification(StakeholderUpload stakeholderUpload, string stakeholderName)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("rmocrdlnotification@gmail.com", "uhflpdedbetywhxu"),
                EnableSsl = true,
            };

            var message = new MailMessage
            {
                From = new MailAddress("rmocrdlnotification@gmail.com"),
                Subject = "Document Uploaded",
                Body = $"A new document has been uploaded by stakeholder: {stakeholderName}.\n\n" +
                       $"Document Name: {stakeholderUpload.NameOfDocument}\n" +
                       $"Type of Document: {stakeholderUpload.TypeOfDocument}\n" +
                       $"Description: {stakeholderUpload.DocumentDescription}\n\n" +
                       $"Uploaded At: {stakeholderUpload.CreatedAt:yyyy-MM-dd HH:mm:ss}\n\n" +
                       "Please review it at your earliest convenience.",
                IsBodyHtml = false,
            };
            message.To.Add("rmocrdl@gmail.com");

            await smtpClient.SendMailAsync(message);
        }

        [Authorize(Roles = "Stakeholder")]
        public async Task<IActionResult> ViewDocuments()
        {

            var user = await _userManager.GetUserAsync(User);
            /*var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int stakeholderId))
            {
                TempData["Error"] = "Invalid stakeholder ID.";
                return RedirectToAction("Login", "Account"); 
            }
*/
            var documents = _context.StakeholderUpload
                .Where(d => d.StakeholderId == user.Id)
                .ToList();

        
            var documentViewModels = documents.Select(doc => new StakeholderUploadViewModel
            {
                DocumentId = doc.DocumentId,
                StakeholderId = doc.StakeholderId,
                NameOfDocument = doc.NameOfDocument,
                TypeOfDocument = doc.TypeOfDocument,
                TypeOfMOA = doc.TypeOfMOA,
                DocumentFile = doc.DocumentFile,
                DocumentDescription = doc.DocumentDescription,
                CreatedAt = doc.CreatedAt,
                UpdatedAt = doc.UpdatedAt,
                ContractStartDate = doc.ContractStartDate,
                ContractEndDate = doc.ContractEndDate,
                Status = doc.Status,
                Comment = doc.Comment,
                ContractStatus = doc.ContractStatus,
                IsArchived = false
            }).ToList();

            return View(documentViewModels);
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
                TempData["Error"] = "Document not found.";
                return NotFound();
            }

            return File(document.DocumentFile, "application/pdf", document.NameOfDocument + ".pdf");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStakeholderUpload(string DocumentId, IFormFile DocumentFile)
        {
            var document = await _context.StakeholderUpload.FindAsync(DocumentId);
            if (document == null)
            {
                TempData["ErrorMessage"] = "Document not found.";
                return RedirectToAction("ViewDocuments");
            }

            // Check if tracking information is needed based on the document type
            var requiresTracking = document.TypeOfDocument == "MOU" ||
                                   (document.TypeOfDocument == "MOA" && document.TypeOfMOA == "Others");

            if (requiresTracking)
            {
                var tracking = await _context.DocumentTracking.FirstOrDefaultAsync(t => t.DocumentId == DocumentId);
                if (tracking == null)
                {
                    TempData["ErrorMessage"] = "Tracking information not found.";
                    return RedirectToAction("ViewDocuments");
                }

                // Update tracking fields
                tracking.IsReceivedByRMO = true;
                tracking.IsReceivedByRMOUpdatedAt = DateTime.Now;
                tracking.IsSubmittedToOVPRED = false;
                tracking.IsSubmittedToOVPREDUpdatedAt = null;
                tracking.IsSubmittedToLegalOffice = false;
                tracking.IsSubmittedToLegalOfficeUpdatedAt = null;
                tracking.IsReceivedByOVPRED = false;
                tracking.IsReceivedByOVPREDUpdatedAt = null;
                tracking.IsReceivedByRMOAfterOVPRED = false;
                tracking.IsReceivedByRMOAfterOVPREDUpdatedAt = null;
                tracking.IsSubmittedToOfficeOfThePresident = false;
                tracking.IsSubmittedToOfficeOfThePresidentUpdatedAt = null;
                tracking.IsReceivedByRMOAfterOfficeOfThePresident = false;
                tracking.IsReceivedByRMOAfterOfficeOfThePresidentUpdatedAt = null;
                tracking.UpdatedAt = DateTime.Now;

                _context.Update(tracking);
            }

            if (DocumentFile != null && DocumentFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await DocumentFile.CopyToAsync(memoryStream);
                    document.DocumentFile = memoryStream.ToArray();
                }

                document.Status = "Pending";
                document.Comment = null;
                document.UpdatedAt = DateTime.Now;

                _context.Update(document);
                await _context.SaveChangesAsync();

                //var user = await _context.Users.FindAsync(document.StakeholderId);
                var user = await _userManager.GetUserAsync(User);
                string stakeholderName = $"{user.FirstName} {user.LastName}";

                await SendChiefNotification(document, stakeholderName);

                TempData["SuccessMessage"] = "Document updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "No file was uploaded.";
            }

            return RedirectToAction("ViewDocuments");
        }

        private async Task SendChiefNotification(StakeholderUpload document, string stakeholderName)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("rmocrdlnotification@gmail.com", "uhflpdedbetywhxu"),
                EnableSsl = true,
            };

            var message = new MailMessage
            {
                From = new MailAddress("rmocrdlnotification@gmail.com"),
                Subject = "Document Re-uploaded by Stakeholder",
                Body = $"A new document has been uploaded by stakeholder: {stakeholderName}.\n\n" +
                       $"Document Name: {document.NameOfDocument}\n" +
                       $"Type of Document: {document.TypeOfDocument}\n" +
                       $"Description: {document.DocumentDescription}\n\n" +
                       $"Uploaded At: {document.UpdatedAt:yyyy-MM-dd HH:mm:ss}\n\n" +
                       "Please review it at your earliest convenience.",
                IsBodyHtml = false,
            };

            message.To.Add("rmocrdl@gmail.com");

            await smtpClient.SendMailAsync(message);
        }

        [Authorize(Roles = "Stakeholder")]
        public IActionResult TrackStatus(string documentId)
        {
            var trackingHistory = _context.DocumentTracking
                .Where(t => t.DocumentId == documentId)
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
                            IsReceivedByRMOUpdatedAt = x.IsReceivedByRMOUpdatedAt,
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
                            UpdatedAt = x.UpdatedAt
                        })
                        .OrderByDescending(x => x.UpdatedAt)
                        .ToList()
                })
                .ToList();

            if (!trackingHistory.Any())
            {
                TempData["ErrorMessage"] = "No tracking history found for this document.";
                return RedirectToAction("ViewDocuments");
            }

            return View(trackingHistory);
        }

    }
}
