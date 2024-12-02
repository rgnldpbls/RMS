using CrdlSys.Models;
using System.Net.Mail;
using System.Net;
using CrdlSys.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ResearchManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Xceed.Words.NET;

namespace CrdlSys.Services
{
    public class NotificationService
    {
        private readonly CrdlDbContext _context;
        private readonly ILogger<NotificationService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationService(CrdlDbContext context, ILogger<NotificationService> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task CheckAndSendPendingStakeholderReminders()
        {
            var cutoffDate = DateTime.Now.AddDays(-3);

            var pendingUploads = await _context.StakeholderUpload
                .Where(upload => upload.Status == "Pending" &&
                                 upload.CreatedAt <= cutoffDate &&
                                 (!upload.LastNotificationSent.HasValue ||
                                  upload.LastNotificationSent.Value <= cutoffDate))
                .ToListAsync();

            if (pendingUploads.Count > 0)
            {
                foreach (var upload in pendingUploads)
                {
                    string body = $"Dear Chief,<br/><br/>" +
                                  $"You have a pending document from a stakeholder that needs review:<br/>" +
                                  $"<strong>Document:</strong> {upload.NameOfDocument}<br/>" +
                                  $"Please follow up with the relevant office regarding the document's status.<br/><br/>" +
                                  "Thank you.";
                    SendEmail("rmocrdl@gmail.com", body, "Pending Document Reminder");
                    upload.LastNotificationSent = DateTime.Now;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task CheckAndSendEventReminders()
        {
            var today = DateTime.Today;
            var events = await _context.ResearchEvent
                .Where(e => e.EventDate >= today && e.EventStatus != "Cancelled")
                .ToListAsync();

            foreach (var researchEvent in events)
            {
                var daysUntilEvent = (researchEvent.EventDate - today).Days;

                if (daysUntilEvent == 7 || daysUntilEvent == 1 || daysUntilEvent == 0)
                {
                    var registrations = await _context.ResearchEventRegistration
                        .Where(r => r.ResearchEventId == researchEvent.ResearchEventId)
                        .ToListAsync();

                    foreach (var registration in registrations)
                    {
                        var user = registration.UserId;
                        if (user != null)
                        {
                            if (registration.LastNotificationSent == null || registration.LastNotificationSent.Value.Date != today)
                            {
                                string emailBody = GenerateEmailBody(researchEvent, daysUntilEvent);
                                SendEmail(registration.UserEmail, emailBody, $"Reminder for Upcoming Event: {researchEvent.EventName}");

                                registration.LastNotificationSent = DateTime.Now;
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
        }

        private static string GenerateEmailBody(ResearchEvent researchEvent, int daysUntilEvent)
        {
            string timing = daysUntilEvent switch
            {
                7 => "one week before",
                1 => "the day before",
                0 => "today",
                _ => string.Empty
            };

            return $"Dear Researcher,<br/><br/>" +
                   $"This is a reminder that the event <strong>{researchEvent.EventName}</strong> will take place {timing}.<br/>" +
                   $"<strong>Date:</strong> {researchEvent.EventDate:MM/dd/yyyy hh:mm tt}<br/>" +
                   $"<strong>Location:</strong> {researchEvent.EventLocation}<br/><br/>" +
                   "We look forward to your participation!";
        }

        private void SendEmail(string recipientEmail, string body, string subject)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("rmocrdlnotification@gmail.com", "uhfl pded bety whxu"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("rmocrdlnotification@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(recipientEmail);

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
            }
        }

        public async void CheckAndSendNotification(StakeholderUpload upload)
        {
            if (upload.ContractStartDate.HasValue && upload.ContractEndDate.HasValue)
            {
                var today = DateTime.Today;
                var sixMonthsFromNow = today.AddMonths(6);

                if (upload.ContractEndDate.Value.ToDateTime(new TimeOnly(0, 0)) < sixMonthsFromNow)
                {
                    if (!upload.LastNotificationSent.HasValue ||
                        (today - upload.LastNotificationSent.Value.Date).TotalDays >= 7)
                    {
                        var stakeholders = await _userManager.GetUsersInRoleAsync("Stakeholder");
                        foreach (var stakeholder in stakeholders)
                        {
                            SendEmail(stakeholder.Email, upload);
                        }

                        upload.LastNotificationSent = DateTime.Now;
                        _context.SaveChanges();
                    }
                }
            }
        }

        public void CheckAndSendChiefNotification(ChiefUpload doc)
        {
            if (doc.ContractStartDate.HasValue && doc.ContractEndDate.HasValue)
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                var sixMonthsFromNow = today.AddMonths(6);

                if (doc.ContractEndDate.Value <= sixMonthsFromNow)
                {
                    if (!doc.LastNotificationSent.HasValue ||
                        (DateTime.Now - doc.LastNotificationSent.Value).TotalDays >= 7)
                    {
                        SendChiefEmail("rmocrdl@gmail.com", doc);

                        doc.LastNotificationSent = DateTime.Now;
                        _context.SaveChanges();
                    }
                }
            }
        }

        /*private List<User> GetStakeholders()
        {
            return _context.UserRoles
                .Where(ur => ur.RoleId == 2)
                .Select(ur => ur.User)
                .ToList();
        }
*/
        private static void SendEmail(string recipientEmail, StakeholderUpload upload)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("rmocrdlnotification@gmail.com", "uhfl pded bety whxu"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("rmocrdlnotification@gmail.com"),
                Subject = "Contract Expiration Notification",
                Body = $"Dear Stakeholder,<br/><br/>" +
                       $"The contract for <strong>{upload.NameOfDocument}</strong> is expiring soon.<br/>" +
                       $"<strong>Start Date:</strong> {upload.ContractStartDate}<br/>" +
                       $"<strong>End Date:</strong> {upload.ContractEndDate}<br/><br/>" +
                       "Please take necessary action.",

                IsBodyHtml = true,
            };

            mailMessage.To.Add(recipientEmail);

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        private static void SendChiefEmail(string recipientEmail, ChiefUpload doc)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("rmocrdlnotification@gmail.com", "uhfl pded bety whxu"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("rmocrdlnotification@gmail.com"),
                Subject = "Contract Expiration Notification",
                Body = $"Dear Chief,<br/><br/>" +
                       $"The contract for <strong>{doc.NameOfDocument}</strong> is expiring soon.<br/>" +
                       $"<strong>Start Date:</strong> {doc.ContractStartDate}<br/>" +
                       $"<strong>End Date:</strong> {doc.ContractEndDate}<br/><br/>" +
                       "Please take necessary action.<br/><br/>",
                IsBodyHtml = true,
            };

            mailMessage.To.Add("rmocrdl@gmail.com");

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        private static void SendChiefExpiredEmail(string recipientEmail, ChiefUpload doc)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("rmocrdlnotification@gmail.com", "uhfl pded bety whxu"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("rmocrdlnotification@gmail.com"),
                Subject = "Contract Expiration Notification",
                Body = $"Dear Chief,<br/><br/>" +
                       $"The contract for <strong>{doc.NameOfDocument}</strong> is expired.<br/>" +
                       "Kindly contact your stakeholder for necessary action.",
                IsBodyHtml = true,
            };

            mailMessage.To.Add("rmocrdl@gmail.com");

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        private static void SendStakeholderExpiredEmail(string recipientEmail, StakeholderUpload upload)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("rmocrdlnotification@gmail.com", "uhfl pded bety whxu"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("rmocrdlnotification@gmail.com"),
                Subject = "Contract Expiration Notification",
                Body = $"Dear Stakeholder,<br/><br/>" +
                       $"The contract for <strong>{upload.NameOfDocument}</strong> is expired.<br/>" +
                       "Kindly contact the chief for necessary action.",

                IsBodyHtml = true,
            };

            mailMessage.To.Add(recipientEmail);

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        public void UpdateDocumentChiefExpirationStatus()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var uploads = _context.ChiefUpload.ToList();

            foreach (var doc in uploads)
            {
                if (doc.ContractEndDate.HasValue)
                {
                    if (doc.ContractEndDate.Value <= today)
                    {
                        if (doc.TypeOfDocument == "MOA")
                        {
                            if (doc.IsManuallyUnarchived)
                            {
                                doc.ContractStatus = "Expired";
                                doc.IsArchived = false;
                            }
                            else
                            {
                                doc.ContractStatus = "Expired";
                                doc.IsArchived = true;

                                if (!doc.LastNotificationSent.HasValue || doc.LastNotificationSent.Value.Date != DateTime.Today)
                                {
                                    SendChiefExpiredEmail("rmocrdl@gmail.com", doc);
                                    doc.LastNotificationSent = DateTime.Now;
                                }
                            }
                        }
                        else
                        {
                            doc.ContractStatus = "Expired";
                        }
                    }
                    else
                    {
                        if (doc.ContractStatus != "Expired" && doc.ContractStatus != "Terminated")
                        {
                            doc.ContractStatus = "Active";
                            doc.IsArchived = false;
                        }
                    }
                }
            }
            _context.SaveChanges();
        }

        public void UpdateDocumentExpirationStatus()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var uploads = _context.StakeholderUpload.ToList();

            foreach (var upload in uploads)
            {
                if (upload.ContractEndDate.HasValue)
                {
                    if (upload.ContractEndDate.Value <= today)
                    {
                        if (upload.TypeOfDocument == "MOA")
                        {
                            if (upload.IsManuallyUnarchived)
                            {
                                upload.ContractStatus = "Expired";
                                upload.IsArchived = false;
                            }
                            else
                            {
                                upload.ContractStatus = "Expired";
                                upload.IsArchived = true;

                                if (!upload.LastNotificationSent.HasValue || upload.LastNotificationSent.Value.Date != DateTime.Today)
                                {
                                    SendStakeholderExpiredEmail(upload.StakeholdeEmail, upload);
                                    upload.LastNotificationSent = DateTime.Now;
                                }
                            }
                        }
                        else
                        {
                            upload.ContractStatus = "Expired";
                        }
                    }
                    else
                    {
                        if (upload.ContractStatus != "Expired" && upload.ContractStatus != "Terminated")
                        {
                            upload.ContractStatus = "Active";
                            upload.IsArchived = false;
                        }
                    }
                }
            }
            _context.SaveChanges();
        }
    }
}
